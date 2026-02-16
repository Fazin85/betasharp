namespace BetaSharp.Client.Input;

public class MouseFilter : java.lang.Object
{
    private float field_22388_a;
    private float field_22387_b;
    private float field_22389_c;

    public float Smooth(float var1, float var2)
    {
        field_22388_a += var1;
        var1 = (field_22388_a - field_22387_b) * var2;
        field_22389_c += (var1 - field_22389_c) * 0.5F;
        if (var1 > 0.0F && var1 > field_22389_c || var1 < 0.0F && var1 < field_22389_c)
        {
            var1 = field_22389_c;
        }

        field_22387_b += var1;
        return var1;
    }
}