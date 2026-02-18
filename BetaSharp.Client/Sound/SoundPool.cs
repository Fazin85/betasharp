using BetaSharp.Util.Maths;
using java.util;

namespace BetaSharp.Client.Sound;

public class SoundPool
{
    private readonly JavaRandom _rand = new();
    private readonly Map weightedSoundSet = new HashMap();
    private readonly List loadedSounds = new ArrayList();
    public int loadedSoundCount = 0;
    public bool isRandom = true;

    private readonly Dictionary<string, List<SoundPoolEntry>> _weightedSoundSet = new();
    private readonly List<SoundPoolEntry> _allLoadedSounds = new();

    public int LoadedSoundCount = 0;
    public bool IsRandom = true;

    public SoundPoolEntry AddSound(string soundPath, FileInfo fileInfo)
    {
        string originalFileName = soundPath;

        string soundKey = soundPath;
        int dotIndex = soundKey.IndexOf('.');
        if (dotIndex != -1)
        {
            soundKey = soundKey[..dotIndex];
        }

        if (IsRandom)
        {
            while (soundKey.Length > 0 && char.IsDigit(soundKey[^1]))
            {
                soundKey = soundKey[..^1];
            }
        }

        soundKey = soundKey.Replace('/', '.');
        if (!_weightedSoundSet.TryGetValue(soundKey, out List<SoundPoolEntry>? variations))
        {
            variations = new List<SoundPoolEntry>();
            _weightedSoundSet[soundKey] = variations;
        }

        SoundPoolEntry entry = new(originalFileName, new Uri(fileInfo.FullName));

        variations.Add(entry);
        _allLoadedSounds.Add(entry);
        LoadedSoundCount++;

        return entry;
    }

    public SoundPoolEntry? GetRandomSoundFromSoundPool(string soundKey)
    {
        if (_weightedSoundSet.TryGetValue(soundKey, out List<SoundPoolEntry>? variations))
        {
            return variations[_rand.NextInt(variations.Count)];
        }
        return null;
    }

    public SoundPoolEntry? GetRandomSound()
    {
        return _allLoadedSounds.Count == 0 ? null : _allLoadedSounds[_rand.NextInt(_allLoadedSounds.Count)];
    }
}