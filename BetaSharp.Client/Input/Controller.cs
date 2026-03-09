using Silk.NET.GLFW;
using System;
using System.Collections.Generic;

namespace BetaSharp.Client.Input;

public static class Controller
{
    private static bool created;
    private static Glfw glfw;
    private static unsafe WindowHandle* window;

    public static int GamepadJoystickIndex = -1;

    // We'll track the standard 15 gamepad buttons + 6 axes
    private static readonly bool[] buttons = new bool[15];
    private static readonly float[] axes = new float[6];

    // Previous state (to detect presses)
    private static readonly bool[] lastButtons = new bool[15];

    // Event queue for button presses
    private static readonly Queue<ControllerEvent> eventQueue = new();
    private static ControllerEvent current_event = new();

    private static int _debugFrames = 0;

    public static unsafe void create(Glfw glfwApi, WindowHandle* windowHandle)
    {
        if (created) return;
        glfw = glfwApi;
        window = windowHandle;
        created = true;

        Console.WriteLine("Controller.create called. Checking joysticks...");
        bool found = false;
        for (int i = 0; i < 16; i++)
        {
            if (glfw.JoystickPresent(i))
            {
                bool isGamepad = glfw.JoystickIsGamepad(i);
                string name = glfw.GetJoystickName(i);
                Console.WriteLine($"Joystick {i} present. Name: {name}, IsGamepad: {isGamepad}");
                found = true;

                if (isGamepad)
                {
                    // Prefer the first one we find, but override if we find "Xbox"
                    if (GamepadJoystickIndex == -1 || name.Contains("Xbox", StringComparison.OrdinalIgnoreCase))
                    {
                        GamepadJoystickIndex = i;
                    }
                }
            }
        }
        if (!found) Console.WriteLine("No joysticks present at all.");
        Console.WriteLine($"Selected Gamepad Index: {GamepadJoystickIndex}");
    }

    public static bool IsGamepadConnected()
    {
        if (!created || GamepadJoystickIndex == -1) return false;
        return glfw.JoystickIsGamepad(GamepadJoystickIndex);
    }

    // Call this every frame in the main loop to poll state and generate events
    public static unsafe void PollEvents()
    {
        if (!created) return;
        if (!IsGamepadConnected())
        {
            // Optional: periodically check if it got connected? Actually GLFW handles this, but let's just return.
            return;
        }

        bool success = glfw.GetGamepadState(GamepadJoystickIndex, out GamepadState state);
        if (!success)
        {
            Console.WriteLine($"Failed to get GamepadState for index {GamepadJoystickIndex}");
            return;
        }

        // Update Axes
        for (int i = 0; i < 6; i++)
        {
            axes[i] = state.Axes[i];
        }

        // Update Buttons and generate events
        for (int i = 0; i < 15; i++)
        {
            lastButtons[i] = buttons[i];
            bool isDown = state.Buttons[i] == 1; // 1 represents GLFW_PRESS
            buttons[i] = isDown;

            if (isDown != lastButtons[i])
            {
                eventQueue.Enqueue(new ControllerEvent
                {
                    Button = i,
                    State = isDown,
                    Nanos = GetNanos()
                });
            }
        }
    }

    public static bool Next()
    {
        if (!created) return false;

        if (eventQueue.Count > 0)
        {
            current_event = eventQueue.Dequeue();
            Console.WriteLine(current_event);
            return true;
        }

        return false;
    }

    public static int GetEventButton() => current_event.Button;
    public static bool GetEventButtonState() => current_event.State;
    public static long GetEventNanoseconds() => current_event.Nanos;

    public static bool IsButtonDown(GamepadButton button)
    {
        int btnIdx = (int)button;
        if (!created || btnIdx < 0 || btnIdx >= 15) return false;
        return buttons[btnIdx];
    }

    public static float GetAxis(int axisIdx)
    {
        if (!created || axisIdx < 0 || axisIdx >= 6) return 0f;
        return axes[axisIdx];
    }

    // Helper properties for common sticks
    public static float LeftStickX => GetAxis(0);
    public static float LeftStickY => GetAxis(1);
    public static float RightStickX => GetAxis(2);
    public static float RightStickY => GetAxis(3);

    // Helper to detect if controller is actively being used (any stick beyond deadzone or any button pressed)
    public static bool IsActive()
    {
        if (!IsGamepadConnected()) return false;

        float deadzone = 0.2f;
        if (Math.Abs(LeftStickX) > deadzone || Math.Abs(LeftStickY) > deadzone ||
            Math.Abs(RightStickX) > deadzone || Math.Abs(RightStickY) > deadzone)
        {
            return true;
        }

        for (int i = 0; i < 15; i++)
        {
            if (buttons[i]) return true;
        }

        return false;
    }

    public static bool IsCreated() => created;

    public static void Destroy()
    {
        if (!created) return;
        created = false;
        eventQueue.Clear();
    }

    private static long GetNanos()
    {
        return DateTime.UtcNow.Ticks * 100;
    }

    private struct ControllerEvent
    {
        public int Button;
        public bool State;
        public long Nanos;
    }
}
