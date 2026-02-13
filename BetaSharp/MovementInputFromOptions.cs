using BetaSharp.Entities;

namespace BetaSharp;

public class MovementInputFromOptions : MovementInput
{

    private bool[] movementKeyStates = new bool[10];
    private GameOptions gameSettings;

    public MovementInputFromOptions(GameOptions var1)
    {
        gameSettings = var1;
    }

    public override void checkKeyForMovementInput(int var1, bool var2)
    {
        int var3 = -1;
        if (var1 == gameSettings.keyBindForward.keyCode)
        {
            var3 = 0;
        }

        if (var1 == gameSettings.keyBindBack.keyCode)
        {
            var3 = 1;
        }

        if (var1 == gameSettings.keyBindLeft.keyCode)
        {
            var3 = 2;
        }

        if (var1 == gameSettings.keyBindRight.keyCode)
        {
            var3 = 3;
        }

        if (var1 == gameSettings.keyBindJump.keyCode)
        {
            var3 = 4;
        }

        if (var1 == gameSettings.keyBindSneak.keyCode)
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
        if (sneak)
        {
            moveStrafe = (float)((double)moveStrafe * 0.3D);
            moveForward = (float)((double)moveForward * 0.3D);
        }

    }
}