namespace BetaSharp;

public class StepSoundStone : BlockSoundGroup
{
    public StepSoundStone(string var1, float var2, float var3) : base(var1, var2, var3)
    {
    }

    public override string stepSoundDir()
    {
        return "random.glass";
    }
}