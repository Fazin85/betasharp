using java.lang;
using java.net;
using java.util;

namespace BetaSharp.Client.Sound;

public class SoundPool : java.lang.Object
{
    private readonly java.util.Random rand = new();
    private readonly Map weightedSoundSet = new HashMap();
    private readonly List loadedSounds = new ArrayList();
    public int loadedSoundCount = 0;
    public bool isRandom = true;

    public SoundPoolEntry addSound(string var1, java.io.File var2)
    {
        try
        {
            string var3 = var1;
            var1 = var1[..var1.IndexOf('.')];
            if (isRandom)
            {
                while (Character.isDigit(var1[var1.Length - 1]))
                {
                    var1 = var1[..^1];
                }
            }

            var1 = var1.Replace('/', '.');
            if (!weightedSoundSet.containsKey(var1))
            {
                weightedSoundSet.put(var1, new ArrayList());
            }

            SoundPoolEntry var4 = new(var3, var2.toURI().toURL());
            ((List)weightedSoundSet.get(var1)).add(var4);
            loadedSounds.add(var4);
            ++loadedSoundCount;
            return var4;
        }
        catch (MalformedURLException var5)
        {
            var5.printStackTrace();
            throw new RuntimeException(var5);
        }
    }

    public SoundPoolEntry getRandomSoundFromSoundPool(string var1)
    {
        List var2 = (List)weightedSoundSet.get(var1);
        return var2 == null ? null : (SoundPoolEntry)var2.get(rand.nextInt(var2.size()));
    }

    public SoundPoolEntry getRandomSound()
    {
        return loadedSounds.size() == 0 ? null : (SoundPoolEntry)loadedSounds.get(rand.nextInt(loadedSounds.size()));
    }
}