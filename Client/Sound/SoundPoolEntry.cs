using java.net;

namespace betareborn.Client.Sound
{
    public class SoundPoolEntry : java.lang.Object
    {
        public string soundName;
        public URL soundUrl;

        public SoundPoolEntry(string var1, URL var2)
        {
            soundName = var1;
            soundUrl = var2;
        }
    }
}