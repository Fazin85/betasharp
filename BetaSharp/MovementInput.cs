using BetaSharp.Entities;

namespace BetaSharp;

public class MovementInput
{
    public float moveStrafe = 0.0F;
    public float moveForward = 0.0F;
    public bool field_1177_c = false;
    public bool jump = false;
    public bool sneak = false;

    public virtual void updatePlayerMoveState(EntityPlayer var1)
    {
    }

    public virtual void resetKeyState()
    {
    }

    public virtual void checkKeyForMovementInput(int var1, bool var2)
    {
    }
}