using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using BetaSharp.Client.Input;
using java.io;
using Microsoft.Extensions.Logging;
using File = System.IO.File;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace BetaSharp.Client.Options;

public class GameOptions
{
    private readonly ILogger<GameOptions> _logger = Log.Instance.For<GameOptions>();

    private static readonly string[] RENDER_DISTANCES =
    [
        "options.renderDistance.far",
        "options.renderDistance.normal",
        "options.renderDistance.short",
        "options.renderDistance.tiny",
    ];
    private static readonly string[] Difficulties =
    [
        "options.difficulty.peaceful",
        "options.difficulty.easy",
        "options.difficulty.normal",
        "options.difficulty.hard",
    ];
    private static readonly string[] GuiScales =
    [
        "options.guiScale.auto",
        "options.guiScale.small",
        "options.guiScale.normal",
        "options.guiScale.large",
    ];

    private static readonly string[] AnisoLeves = ["options.off", "2x", "4x", "8x", "16x"];
    private static readonly string[] MSAALeves = ["options.off", "2x", "4x", "8x"];

    public static float MaxAnisotropy = 1.0f;
    public float MusicVolume = 1.0F;
    public float SoundVolume = 1.0F;
    public float MouseSensitivity = 0.5F;
    public float Brightness = 0.5F;
    public bool VSync = false;
    public bool InvertMouse;
    public int renderDistance;
    public bool ViewBobbing = true;
    public float LimitFramerate = 0.42857143f; // 0.428... = 120, 1.0 = 240, 0.0 = 30
    public float Fov = 0.44444445F; // (70 - 30) / 90
    public string Skin = "Default";

    public KeyBinding KeyBindForward = new("key.forward", 17);
    public KeyBinding KeyBindLeft = new("key.left", 30);
    public KeyBinding KeyBindBack = new("key.back", 31);
    public KeyBinding KeyBindRight = new("key.right", 32);
    public KeyBinding KeyBindJump = new("key.jump", 57);
    public KeyBinding KeyBindInventory = new("key.inventory", 18);
    public KeyBinding KeyBindDrop = new("key.drop", 16);
    public KeyBinding KeyBindChat = new("key.chat", 20);
    public KeyBinding KeyBindCommand = new("key.command", Keyboard.KEY_SLASH);
    public KeyBinding KeyBindToggleFog = new("key.fog", 33);
    public KeyBinding KeyBindSneak = new("key.sneak", 42);
    public KeyBinding[] KeyBindings;

    protected Minecraft _mc;
    private readonly string _optionsPath;
    public int Difficulty = 2;
    public bool HideGUI = false;
    public EnumCameraMode CameraMode = EnumCameraMode.FirstPerson;
    public bool ShowDebugInfo = false;
    public string LastServer = "";
    public bool InvertScrolling = false;
    public bool SmoothCamera = false;
    public bool DebugCamera = false;
    public float AmountScrolled = 1.0F;
    public float field_22271_G = 1.0F;
    public int GuiScale;
    public int AnisotropicLevel;
    public int MSAALevel;
    public int INITIAL_MSAA;
    public bool UseMipmaps = true;
    public bool DebugMode;
    public bool EnvironmentAnimation = true;

    public GameOptions(Minecraft mc, string mcDataDir)
    {
        KeyBindings =
        [
            KeyBindForward,
            KeyBindLeft,
            KeyBindBack,
            KeyBindRight,
            KeyBindJump,
            KeyBindSneak,
            KeyBindDrop,
            KeyBindInventory,
            KeyBindChat,
            KeyBindToggleFog,
        ];
        _mc = mc;
        _optionsPath = System.IO.Path.Combine(mcDataDir, "options.txt");
        LoadOptions();
        INITIAL_MSAA = MSAALevel;
    }

    public GameOptions() { }

    public string GetKeyBindingDescription(int keyBindingIndex)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        return translations.TranslateKey(KeyBindings[keyBindingIndex].keyDescription);
    }

    public string GetOptionDisplayString(int keyBindingIndex)
    {
        return Keyboard.getKeyName(KeyBindings[keyBindingIndex].keyCode);
    }

    public void SetKeyBinding(int keyBindingIndex, int keyCode)
    {
        KeyBindings[keyBindingIndex].keyCode = keyCode;
        SaveOptions();
    }

    public void SetOptionFloatValue(EnumOptions option, float value)
    {
        if (option == EnumOptions.MUSIC)
        {
            MusicVolume = value;
            _mc.sndManager.OnSoundOptionsChanged();
        }
        else if (option == EnumOptions.SOUND)
        {
            SoundVolume = value;
            _mc.sndManager.OnSoundOptionsChanged();
        }
        else if (option == EnumOptions.SENSITIVITY)
        {
            MouseSensitivity = value;
        }
        else if (option == EnumOptions.FRAMERATE_LIMIT)
        {
            LimitFramerate = value;
        }
        else if (option == EnumOptions.FOV)
        {
            Fov = value;
        }

        SaveOptions();
    }

    public void SetOptionValue(EnumOptions option, int increment)
    {
        if (option == EnumOptions.INVERT_MOUSE)
        {
            InvertMouse = !InvertMouse;
        }
        else if (option == EnumOptions.RENDER_DISTANCE)
        {
            renderDistance = renderDistance + increment & 3;
        }
        else if (option == EnumOptions.GUI_SCALE)
        {
            GuiScale = GuiScale + increment & 3;
        }
        else if (option == EnumOptions.VIEW_BOBBING)
        {
            ViewBobbing = !ViewBobbing;
        }
        else if (option == EnumOptions.VSYNC)
        {
            VSync = !VSync;
            Display.getGlfw().SwapInterval(VSync ? 1 : 0);
        }
        else if (option == EnumOptions.DIFFICULTY)
        {
            Difficulty = Difficulty + increment & 3;
        }
        else if (option == EnumOptions.ANISOTROPIC)
        {
            AnisotropicLevel = (AnisotropicLevel + increment) % 5;
            int anisoValue = AnisotropicLevel == 0 ? 0 : (int)System.Math.Pow(2, AnisotropicLevel);
            if (anisoValue > MaxAnisotropy)
            {
                AnisotropicLevel = 0;
            }
            if (Minecraft.INSTANCE?.textureManager != null)
            {
                Minecraft.INSTANCE.textureManager.Reload();
            }
        }
        else if (option == EnumOptions.MIPMAPS)
        {
            UseMipmaps = !UseMipmaps;
            if (Minecraft.INSTANCE?.textureManager != null)
            {
                Minecraft.INSTANCE.textureManager.Reload();
            }
        }
        else if (option == EnumOptions.MSAA)
        {
            MSAALevel = (MSAALevel + increment) % 4;
        }
        else if (option == EnumOptions.DEBUG_MODE)
        {
            DebugMode = !DebugMode;
            Profiling.Profiler.Enabled = DebugMode;
        }
        else if (option == EnumOptions.ENVIRONMENT_ANIMATION)
        {
            EnvironmentAnimation = !EnvironmentAnimation;
        }

        SaveOptions();
    }

    public float GetOptionFloatValue(EnumOptions option)
    {
        if (option == EnumOptions.MUSIC) return MusicVolume;
        if (option == EnumOptions.SOUND) return SoundVolume;
        if (option == EnumOptions.SENSITIVITY) return MouseSensitivity;
        if (option == EnumOptions.FRAMERATE_LIMIT) return LimitFramerate;
        if (option == EnumOptions.FOV) return Fov;
        return 0.0F;
    }

    public bool GetOptionOrdinalValue(EnumOptions option)
    {
        int mappedValue = EnumOptionsMappingHelper.enumOptionsMappingHelperArray[option.ordinal()];
        return mappedValue switch
        {
            1 => InvertMouse,
            2 => ViewBobbing,
            3 => UseMipmaps,
            4 => DebugMode,
            5 => EnvironmentAnimation,
            6 => VSync,
            _ => false
        };
    }

    public string GetKeyBinding(EnumOptions option)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        string label = GetOptionLabel(option, translations) + ": ";

        if (option.getEnumFloat())
        {
            return FormatFloatValue(option, label, translations);
        }
        else if (option.getEnumBoolean())
        {
            bool isEnabled = GetOptionOrdinalValue(option);
            return label + (isEnabled ? translations.TranslateKey("options.on") : translations.TranslateKey("options.off"));
        }
        else if (option == EnumOptions.MSAA)
        {
            return FormatMsaaValue(label, translations);
        }
        else
        {
            return FormatEnumValue(option, label, translations);
        }
    }

    private string GetOptionLabel(EnumOptions option, TranslationStorage translations)
    {
        if (option == EnumOptions.FRAMERATE_LIMIT) return "Max FPS";
        if (option == EnumOptions.BRIGHTNESS) return "Brightness";
        if (option == EnumOptions.VSYNC) return "VSync";
        if (option == EnumOptions.FOV) return "FOV";
        return translations.TranslateKey(option.getEnumString());
    }

    private string FormatFloatValue(EnumOptions option, string label, TranslationStorage translations)
    {
        float value = GetOptionFloatValue(option);

        if (option == EnumOptions.SENSITIVITY)
        {
            return value == 0.0F ? label + translations.TranslateKey("options.sensitivity.min") : (value == 1.0F ? label + translations.TranslateKey("options.sensitivity.max") : label + (int)(value * 200.0F) + "%");
        }
        else if (option == EnumOptions.FRAMERATE_LIMIT)
        {
            return FormatFramerateValue(label, value);
        }
        else if (option == EnumOptions.FOV)
        {
            return label + (30 + (int)(value * 90.0f));
        }
        else
        {
            return value == 0.0F
                ? label + translations.TranslateKey("options.off")
                : label + $"{(int)(value * 100.0F)}%";
        }
    }

    private string FormatFramerateValue(string label, float value)
    {
        int fps = 30 + (int)(value * 210.0f);
        return label + (fps == 240 ? "Unlimited" : fps + " FPS");
    }

    private string FormatMsaaValue(string label, TranslationStorage translations)
    {
        string result = label + (MSAALevel == 0 ? translations.TranslateKey("options.off") : MSAALeves[MSAALevel]);
        if (MSAALevel != INITIAL_MSAA)
        {
            result += " (Reload required)";
        }
        return result;
    }

    private string FormatEnumValue(EnumOptions option, string label, TranslationStorage translations)
    {
        if (option == EnumOptions.RENDER_DISTANCE) return label + translations.TranslateKey(RENDER_DISTANCES[renderDistance]);
        if (option == EnumOptions.DIFFICULTY) return label + translations.TranslateKey(Difficulties[Difficulty]);
        if (option == EnumOptions.GUI_SCALE) return label + translations.TranslateKey(GuiScales[GuiScale]);
        if (option == EnumOptions.ANISOTROPIC) return label + (AnisotropicLevel == 0 ? translations.TranslateKey("options.off") : AnisoLeves[AnisotropicLevel]);
        return label;
    }

    public void LoadOptions()
    {
        try
        {
            if (!File.Exists(_optionsPath)) throw new FileNotFoundException($"Options file not found at {_optionsPath}");
            using StreamReader reader = new StreamReader(_optionsPath);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    string[] parts = line.Split(':');
                    if (parts.Length >= 2) LoadOptionFromParts(parts);
                }
                catch (Exception)
                {
                    _logger.LogError($"Skipping bad option: {line}");
                }
            }
        }
        catch (Exception)
        {
            _logger.LogError("Failed to load options");
        }
    }

    private void LoadOptionFromParts(string[] parts)
    {
        if (parts.Length < 2) return;

        string key = parts[0];
        string value = parts[1];

        switch (key)
        {
            case "music": MusicVolume = ParseFloat(value); break;
            case "sound": SoundVolume = ParseFloat(value); break;
            case "mouseSensitivity": MouseSensitivity = ParseFloat(value); break;
            case "invertYMouse": InvertMouse = value == "true"; break; // Simplified boolean parsing
            case "viewDistance": renderDistance = int.Parse(value); break;
            case "guiScale": GuiScale = int.Parse(value); break;
            case "bobView": ViewBobbing = value == "true"; break;
            case "fpsLimit": LimitFramerate = ParseFloat(value); break;
            case "vsync": VSync = bool.Parse(value); break;
            case "fov": Fov = ParseFloat(value); break;
            case "difficulty": Difficulty = int.Parse(value); break;
            case "skin": Skin = value; break;
            case "lastServer": LastServer = value; break; // Safe now because of the global length check
            case "anisotropicLevel": AnisotropicLevel = int.Parse(value); break;
            case "msaaLevel":
                MSAALevel = int.Parse(value);
                if (MSAALevel > 3) MSAALevel = 3;
                break;
            case "useMipmaps": UseMipmaps = value == "true"; break;
            case "debugMode": DebugMode = value == "true"; break;
            case "envAnimation": EnvironmentAnimation = value == "true"; break;
            case "cameraMode": CameraMode = (EnumCameraMode)int.Parse(value); break;
            case "thirdPersonView": // backward compatibility
                CameraMode = value == "true" ? EnumCameraMode.ThirdPerson : EnumCameraMode.FirstPerson;
                break;

            default:
                if (key.StartsWith("key_"))
                {
                    string bindName = key[4..];

                    for (int i = 0; i < KeyBindings.Length; ++i)
                    {
                        if (KeyBindings[i].keyDescription == bindName)
                        {
                            KeyBindings[i].keyCode = int.Parse(value);
                            break;
                        }
                    }
                }
                break;
        }
    }

    private float ParseFloat(string value)
    {
        return value switch
        {
            "true" => 1.0F,
            "false" => 0.0F,
            _ => float.Parse(value)
        };
    }

    public void SaveOptions()
    {
        try
        {
            using var writer = new StreamWriter(_optionsPath);
            writer.WriteLine($"music:{MusicVolume}");
            writer.WriteLine($"sound:{SoundVolume}");
            writer.WriteLine($"invertYMouse:{InvertMouse.ToString().ToLower()}");
            writer.WriteLine($"mouseSensitivity:{MouseSensitivity}");
            writer.WriteLine($"viewDistance:{renderDistance}");
            writer.WriteLine($"guiScale:{GuiScale}");
            writer.WriteLine($"bobView:{ViewBobbing.ToString().ToLower()}");
            writer.WriteLine($"fpsLimit:{LimitFramerate}");
            writer.WriteLine($"vsync:{VSync}");
            writer.WriteLine($"fov:{Fov}");
            writer.WriteLine($"difficulty:{Difficulty}");
            writer.WriteLine($"skin:{Skin}");
            writer.WriteLine($"lastServer:{LastServer}");
            writer.WriteLine($"anisotropicLevel:{AnisotropicLevel}");
            writer.WriteLine($"msaaLevel:{MSAALevel}");
            writer.WriteLine($"useMipmaps:{UseMipmaps.ToString().ToLower()}");
            writer.WriteLine($"debugMode:{DebugMode.ToString().ToLower()}");
            writer.WriteLine($"envAnimation:{EnvironmentAnimation.ToString().ToLower()}");
            writer.WriteLine($"cameraMode:{(int)CameraMode}");

            foreach (var bind in KeyBindings)
            {
                writer.WriteLine($"key_{bind.keyDescription}:{bind.keyCode}");
            }

            writer.Close();
        }
        catch (Exception exception)
        {
            _logger.LogError($"Failed to save options: {exception.Message}");
        }
    }
}
