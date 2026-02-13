using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using SFML.Audio;
using SFML.System;

namespace BetaSharp.Client.Sound;

public class SoundManager : java.lang.Object
{
    private readonly SoundPool soundPoolSounds = new();
    private readonly SoundPool soundPoolStreaming = new();
    private readonly SoundPool soundPoolMusic = new();

    private readonly Dictionary<string, List<SoundBuffer>> soundBuffers = [];

    private const int MAX_CHANNELS = 32;
    private readonly SFML.Audio.Sound[] soundChannels = new SFML.Audio.Sound[MAX_CHANNELS];

    private int soundSourceSuffix = 0;
    private GameOptions options;
    private static bool started = false;
    private readonly java.util.Random rand = new();
    private int ticksBeforeMusic = 0;
    private Music currentMusic = null;
    private Music currentStreaming = null;

    public SoundManager()
    {
        ticksBeforeMusic = rand.nextInt(12000);
    }

    public void loadSoundSettings(GameOptions var1)
    {
        soundPoolStreaming.isRandom = false;
        options = var1;
        if (!started && (var1 == null || var1.soundVolume != 0.0F || var1.musicVolume != 0.0F))
        {
            tryToSetLibraryAndCodecs();
        }
    }

    private static string sanitizePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        if (path.StartsWith('/') && path.Length >= 3 && path[2] == ':')
        {
            path = path[1..];
        }

        return path.Replace("/", "\\");
    }

    private void tryToSetLibraryAndCodecs()
    {
        try
        {
            float var1 = options.soundVolume;
            float var2 = options.musicVolume;
            options.soundVolume = 0.0F;
            options.musicVolume = 0.0F;
            options.saveOptions();

            options.soundVolume = var1;
            options.musicVolume = var2;
            options.saveOptions();
        }
        catch (java.lang.Throwable var3)
        {
            var3.printStackTrace();
            java.lang.System.err.println("error initializing audio system");
        }

        started = true;
    }

    public void onSoundOptionsChanged()
    {
        if (!started && (options.soundVolume != 0.0F || options.musicVolume != 0.0F))
        {
            tryToSetLibraryAndCodecs();
        }

        if (started)
        {
            if (options.musicVolume == 0.0F)
            {
                currentMusic?.Stop();
            }
            else
            {
                currentMusic?.Volume = options.musicVolume * 100.0F;
            }
        }
    }

    public void closeMinecraft()
    {
        if (started)
        {
            currentMusic?.Stop();
            currentMusic?.Dispose();
            currentStreaming?.Stop();
            currentStreaming?.Dispose();

            for (int i = 0; i < MAX_CHANNELS; i++)
            {
                if (soundChannels[i] != null)
                {
                    soundChannels[i].Stop();
                    soundChannels[i].Dispose();
                    soundChannels[i] = null;
                }
            }

            foreach (var bufferList in soundBuffers.Values)
            {
                foreach (var buffer in bufferList)
                {
                    buffer.Dispose();
                }
            }
            soundBuffers.Clear();
        }
    }

    public void addSound(string var1, java.io.File var2)
    {
        soundPoolSounds.addSound(var1, var2);
        LoadSoundBuffer(var1, var2);
    }

    public void addStreaming(string var1, java.io.File var2)
    {
        soundPoolStreaming.addSound(var1, var2);
    }

    public void addMusic(string var1, java.io.File var2)
    {
        soundPoolMusic.addSound(var1, var2);
    }

    private void LoadSoundBuffer(string name, java.io.File file)
    {
        try
        {
            string filepath = file.getPath();

            string originalName = name;
            string resourceName = name;

            int dotIndex = resourceName.IndexOf('.');
            if (dotIndex >= 0)
            {
                resourceName = resourceName[..dotIndex];
            }

            if (soundPoolSounds.isRandom)
            {
                while (resourceName.Length > 0 && char.IsDigit(resourceName[resourceName.Length - 1]))
                {
                    resourceName = resourceName[..^1];
                }
            }

            resourceName = resourceName.Replace("/", ".");

            if (!soundBuffers.TryGetValue(resourceName, out List<SoundBuffer>? value))
            {
                value = [];
                soundBuffers[resourceName] = value;
            }

            SoundBuffer buffer = new(filepath);
            value.Add(buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load sound buffer {name}: {ex.Message}");
        }
    }

    private SoundBuffer getRandomSoundBuffer(string name)
    {
        if (name == null)
        {
            return null;
        }

        if (!soundBuffers.TryGetValue(name, out List<SoundBuffer>? value) || value.Count == 0)
        {
            return null;
        }

        int index = rand.nextInt(value.Count);
        return value[index];
    }

    private SFML.Audio.Sound getFreeSoundChannel(SoundBuffer buffer)
    {
        for (int i = 0; i < MAX_CHANNELS; i++)
        {
            if (soundChannels[i] == null)
            {
                soundChannels[i] = new SFML.Audio.Sound(buffer);
                return soundChannels[i];
            }

            if (soundChannels[i].Status == SoundStatus.Stopped)
            {
                soundChannels[i].SoundBuffer = buffer;
                return soundChannels[i];
            }
        }

        SFML.Audio.Sound stolen = soundChannels[0];
        stolen.Stop();
        stolen.SoundBuffer = buffer;
        return stolen;
    }

    public void playRandomMusicIfReady()
    {
        if (started && options.musicVolume != 0.0F)
        {
            bool isMusicPlaying = currentMusic != null && currentMusic.Status == SoundStatus.Playing;
            bool isStreamingPlaying = currentStreaming != null && currentStreaming.Status == SoundStatus.Playing;

            if (!isMusicPlaying && !isStreamingPlaying)
            {
                if (ticksBeforeMusic > 0)
                {
                    --ticksBeforeMusic;
                    return;
                }

                SoundPoolEntry var1 = soundPoolMusic.getRandomSound();
                if (var1 != null)
                {
                    try
                    {
                        ticksBeforeMusic = rand.nextInt(12000) + 12000;

                        currentMusic?.Stop();
                        currentMusic?.Dispose();

                        var musicName = sanitizePath(var1.soundUrl.getPath());

                        currentMusic = new Music(musicName)
                        {
                            Volume = options.musicVolume * 100.0F,
                            IsLooping = false
                        };

                        currentMusic.RelativeToListener = true;
                        currentMusic.Position = new Vector3f(0, 0, 0);

                        Console.WriteLine($"Playing random music: {musicName}");

                        currentMusic.Play();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to play music: {ex.Message}");
                    }
                }
            }
        }
    }

    public void updateListener(EntityLiving var1, float var2)
    {
        if (started && options.soundVolume != 0.0F)
        {
            if (var1 != null)
            {
                float var3 = var1.prevYaw + (var1.yaw - var1.prevYaw) * var2;
                double var4 = var1.prevX + (var1.x - var1.prevX) * (double)var2;
                double var6 = var1.prevY + (var1.y - var1.prevY) * (double)var2;
                double var8 = var1.prevZ + (var1.z - var1.prevZ) * (double)var2;
                float var10 = MathHelper.cos(-var3 * ((float)Math.PI / 180.0F) - (float)Math.PI);
                float var11 = MathHelper.sin(-var3 * ((float)Math.PI / 180.0F) - (float)Math.PI);
                float var12 = -var11;
                float var13 = 0.0F;
                float var14 = -var10;
                float var15 = 0.0F;
                float var16 = 1.0F;
                float var17 = 0.0F;

                Listener.Position = new Vector3f((float)var4, (float)var6, (float)var8);
                Listener.Direction = new Vector3f(var12, var13, var14);
                Listener.UpVector = new Vector3f(var15, var16, var17);
            }
        }
    }

    public void playStreaming(string var1, float var2, float var3, float var4, float var5, float var6)
    {
        if (started && options.soundVolume != 0.0F)
        {
            if (currentStreaming != null && currentStreaming.Status == SoundStatus.Playing)
            {
                currentStreaming.Stop();
            }

            if (var1 != null)
            {
                SoundPoolEntry var8 = soundPoolStreaming.getRandomSoundFromSoundPool(var1);
                if (var8 != null && var5 > 0.0F)
                {
                    try
                    {
                        if (currentMusic != null && currentMusic.Status == SoundStatus.Playing)
                        {
                            currentMusic.Stop();
                        }

                        currentStreaming?.Dispose();
                        currentStreaming = new Music(sanitizePath(var8.soundUrl.getPath()))
                        {
                            Volume = 0.5F * options.soundVolume * 100.0F,
                            IsLooping = false,
                            RelativeToListener = false,
                            Position = new(var2, var3, var4)
                        };

                        currentStreaming.Play();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to play streaming audio: {ex.Message}");
                    }
                }
            }
        }
    }

    public void playSound(string var1, float var2, float var3, float var4, float var5, float var6)
    {
        if (started && options.soundVolume != 0.0F)
        {
            SoundBuffer buffer = getRandomSoundBuffer(var1);
            if (buffer != null && var5 > 0.0F)
            {
                soundSourceSuffix = (soundSourceSuffix + 1) % 256;

                SFML.Audio.Sound sound = getFreeSoundChannel(buffer);

                sound.Position = new Vector3f(var2, var3, var4);
                sound.RelativeToListener = false;

                float var9 = 16.0F;
                if (var5 > 1.0F)
                {
                    var9 *= var5;
                }
                sound.MinDistance = var9;
                sound.Attenuation = 2.0F;

                sound.Pitch = var6;

                float finalVolume = var5;
                if (finalVolume > 1.0F)
                {
                    finalVolume = 1.0F;
                }
                sound.Volume = finalVolume * options.soundVolume * 100.0F;

                sound.Play();
            }
        }
    }

    public void playSoundFX(string var1, float var2, float var3)
    {
        if (started && options.soundVolume != 0.0F)
        {
            SoundBuffer buffer = getRandomSoundBuffer(var1);
            if (buffer != null)
            {
                soundSourceSuffix = (soundSourceSuffix + 1) % 256;

                SFML.Audio.Sound sound = getFreeSoundChannel(buffer);

                sound.RelativeToListener = true;
                sound.Position = new Vector3f(0.0F, 0.0F, 0.0F);

                sound.Pitch = var3;

                float finalVolume = var2;
                if (finalVolume > 1.0F)
                {
                    finalVolume = 1.0F;
                }
                finalVolume *= 0.25F;
                sound.Volume = finalVolume * options.soundVolume * 100.0F;

                sound.MinDistance = 1.0f;
                sound.Attenuation = 1.0f;

                sound.Play();
            }
        }
    }
}