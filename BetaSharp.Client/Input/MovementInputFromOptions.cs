using BetaSharp.Client.Options;
using BetaSharp.Entities;

namespace BetaSharp.Client.Input;

public class MovementInputFromOptions : MovementInput
{
    private const int SprintDoubleTapTicks = 7;

    private readonly bool[] movementKeyStates = new bool[10];
    private readonly GameOptions gameSettings;
    private int sprintTapTimer;

    public MovementInputFromOptions(GameOptions var1)
    {
        gameSettings = var1;
    }

    public override void checkKeyForMovementInput(int var1, bool var2)
    {
        int var3 = -1;
        if (var1 == gameSettings.KeyBindForward.keyCode)
        {
            var3 = 0;

            if (var2)
            {
                if (sprintTapTimer > 0)
                {
                    sprinting = true;
                }
                else
                {
                    sprintTapTimer = SprintDoubleTapTicks;
                }
            }
        }

        if (var1 == gameSettings.KeyBindBack.keyCode)
        {
            var3 = 1;
        }

        if (var1 == gameSettings.KeyBindLeft.keyCode)
        {
            var3 = 2;
        }

        if (var1 == gameSettings.KeyBindRight.keyCode)
        {
            var3 = 3;
        }

        if (var1 == gameSettings.KeyBindJump.keyCode)
        {
            var3 = 4;
        }

        if (var1 == gameSettings.KeyBindSneak.keyCode)
        {
            var3 = 5;
        }

        if (var3 >= 0)
        {
            movementKeyStates[var3] = var2;
        }

    }

    public override void resetKeyState()
    {
        for (int var1 = 0; var1 < 10; ++var1)
        {
            movementKeyStates[var1] = false;
        }

        sprinting = false;
        sprintTapTimer = 0;
    }

    public override void updatePlayerMoveState(EntityPlayer var1)
    {
        moveStrafe = 0.0F;
        moveForward = 0.0F;
        if (movementKeyStates[0])
        {
            ++moveForward;
        }

        if (movementKeyStates[1])
        {
            --moveForward;
        }

        if (movementKeyStates[2])
        {
            ++moveStrafe;
        }

        if (movementKeyStates[3])
        {
            --moveStrafe;
        }

        jump = movementKeyStates[4];
        sneak = movementKeyStates[5];

        if (sprinting && (moveForward <= 0.0F || sneak))
        {
            sprinting = false;
        }

        if (sprintTapTimer > 0)
        {
            --sprintTapTimer;
        }

        if (sneak)
        {
            moveStrafe = (float)((double)moveStrafe * 0.3D);
            moveForward = (float)((double)moveForward * 0.3D);
        }

    }
}
