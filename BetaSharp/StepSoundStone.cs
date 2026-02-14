namespace BetaSharp;

public class StepSoundStone : BlockSoundGroup
{
    public StepSoundStone(string name, float volume, float pitch) : base(name, volume, pitch)
    {
    }

    public override string stepSoundDir()
    {
        return "random.glass";
    }
}