namespace BetaSharp;

public class BlockSoundGroup : java.lang.Object
{
    public readonly string name;
    public readonly float volume;
    public readonly float pitch;

    public BlockSoundGroup(string name, float volume, float pitch)
    {
        this.name = name;
        this.volume = volume;
        this.pitch = pitch;
    }

    public float getVolume()
    {
        return volume;
    }

    public float getPitch()
    {
        return pitch;
    }

    public virtual string stepSoundDir()
    {
        return "step." + name;
    }

    public string getName()
    {
        return "step." + name;
    }
}