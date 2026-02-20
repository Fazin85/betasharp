namespace BetaSharp;

public record BlockSoundGroup(string GroupName, float Volume, float Pitch, string? CustomBreakSound = null)
{
    public readonly string StepSound = $"step.{GroupName}";
    public string BreakSound => CustomBreakSound ?? StepSound;
}
