namespace BetaSharp;

public class StepSoundSand : BlockSoundGroup
{
    public StepSoundSand(string name, float var2, float pitch) : base(name, var2, pitch)
    {
    }

    public override string stepSoundDir()
    {
        return "step.gravel";
    }
}