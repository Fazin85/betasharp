namespace BetaSharp;

public class BlockSoundGroup : java.lang.Object
{
    public readonly string field_1678_a;
    public readonly float field_1677_b;
    public readonly float field_1679_c;

    public BlockSoundGroup(string var1, float var2, float var3)
    {
        field_1678_a = var1;
        field_1677_b = var2;
        field_1679_c = var3;
    }

    public float getVolume()
    {
        return field_1677_b;
    }

    public float getPitch()
    {
        return field_1679_c;
    }

    public virtual string stepSoundDir()
    {
        return "step." + field_1678_a;
    }

    public string func_1145_d()
    {
        return "step." + field_1678_a;
    }
}