using betareborn.Entities;
using betareborn.NBT;
using java.util;

namespace betareborn.Worlds
{
    public class WorldProperties : java.lang.Object
    {
        private readonly long randomSeed;
        private int spawnX;
        private int spawnY;
        private int spawnZ;
        private long worldTime;
        private readonly long lastTimePlayed;
        private long sizeOnDisk;
        private NBTTagCompound playerTag;
        private readonly int dimension;
        private string levelName;
        private int saveVersion;
        private bool raining;
        private int rainTime;
        private bool thundering;
        private int thunderTime;

        public WorldProperties(NBTTagCompound var1)
        {
            randomSeed = var1.getLong("RandomSeed");
            spawnX = var1.getInteger("SpawnX");
            spawnY = var1.getInteger("SpawnY");
            spawnZ = var1.getInteger("SpawnZ");
            worldTime = var1.getLong("Time");
            lastTimePlayed = var1.getLong("LastPlayed");
            levelName = var1.getString("LevelName");
            saveVersion = var1.getInteger("version");
            rainTime = var1.getInteger("rainTime");
            raining = var1.getBoolean("raining");
            thunderTime = var1.getInteger("thunderTime");
            thundering = var1.getBoolean("thundering");
            if (var1.hasKey("Player"))
            {
                playerTag = var1.getCompoundTag("Player");
                dimension = playerTag.getInteger("Dimension");
            }

        }

        public WorldProperties(long var1, String var3)
        {
            randomSeed = var1;
            levelName = var3;
        }

        public WorldProperties(WorldProperties var1)
        {
            randomSeed = var1.randomSeed;
            spawnX = var1.spawnX;
            spawnY = var1.spawnY;
            spawnZ = var1.spawnZ;
            worldTime = var1.worldTime;
            lastTimePlayed = var1.lastTimePlayed;
            sizeOnDisk = var1.sizeOnDisk;
            playerTag = var1.playerTag;
            dimension = var1.dimension;
            levelName = var1.levelName;
            saveVersion = var1.saveVersion;
            rainTime = var1.rainTime;
            raining = var1.raining;
            thunderTime = var1.thunderTime;
            thundering = var1.thundering;
        }

        public NBTTagCompound getNBTTagCompound()
        {
            NBTTagCompound var1 = new();
            updateTagCompound(var1, playerTag);
            return var1;
        }

        public NBTTagCompound getNBTTagCompoundWithPlayer(List<object> var1)
        {
            NBTTagCompound var2 = new();
            EntityPlayer var3 = null;
            NBTTagCompound var4 = null;
            if (var1.Count > 0)
            {
                var3 = (EntityPlayer)var1[0];
            }

            if (var3 != null)
            {
                var4 = new NBTTagCompound();
                var3.write(var4);
            }

            updateTagCompound(var2, var4);
            return var2;
        }

        private void updateTagCompound(NBTTagCompound var1, NBTTagCompound var2)
        {
            var1.setLong("RandomSeed", randomSeed);
            var1.setInteger("SpawnX", spawnX);
            var1.setInteger("SpawnY", spawnY);
            var1.setInteger("SpawnZ", spawnZ);
            var1.setLong("Time", worldTime);
            var1.setLong("SizeOnDisk", sizeOnDisk);
            var1.setLong("LastPlayed", java.lang.System.currentTimeMillis());
            var1.setString("LevelName", levelName);
            var1.setInteger("version", saveVersion);
            var1.setInteger("rainTime", rainTime);
            var1.setBoolean("raining", raining);
            var1.setInteger("thunderTime", thunderTime);
            var1.setBoolean("thundering", thundering);
            if (var2 != null)
            {
                var1.setCompoundTag("Player", var2);
            }

        }

        public long getRandomSeed()
        {
            return randomSeed;
        }

        public int getSpawnX()
        {
            return spawnX;
        }

        public int getSpawnY()
        {
            return spawnY;
        }

        public int getSpawnZ()
        {
            return spawnZ;
        }

        public long getTime()
        {
            return worldTime;
        }

        public long getSizeOnDisk()
        {
            return sizeOnDisk;
        }

        public NBTTagCompound getPlayerNBTTagCompound()
        {
            return playerTag;
        }

        public int getDimension()
        {
            return dimension;
        }

        public void setSpawnX(int var1)
        {
            spawnX = var1;
        }

        public void setSpawnY(int var1)
        {
            spawnY = var1;
        }

        public void setSpawnZ(int var1)
        {
            spawnZ = var1;
        }

        public void setWorldTime(long var1)
        {
            worldTime = var1;
        }

        public void setSizeOnDisk(long var1)
        {
            sizeOnDisk = var1;
        }

        public void setPlayerNBTTagCompound(NBTTagCompound var1)
        {
            playerTag = var1;
        }

        public void setSpawn(int var1, int var2, int var3)
        {
            spawnX = var1;
            spawnY = var2;
            spawnZ = var3;
        }

        public string getWorldName()
        {
            return levelName;
        }

        public void setWorldName(string var1)
        {
            levelName = var1;
        }

        public int getSaveVersion()
        {
            return saveVersion;
        }

        public void setSaveVersion(int var1)
        {
            saveVersion = var1;
        }

        public long getLastTimePlayed()
        {
            return lastTimePlayed;
        }

        public bool getThundering()
        {
            return thundering;
        }

        public void setThundering(bool var1)
        {
            thundering = var1;
        }

        public int getThunderTime()
        {
            return thunderTime;
        }

        public void setThunderTime(int var1)
        {
            thunderTime = var1;
        }

        public bool getRaining()
        {
            return raining;
        }

        public void setRaining(bool var1)
        {
            raining = var1;
        }

        public int getRainTime()
        {
            return rainTime;
        }

        public void setRainTime(int var1)
        {
            rainTime = var1;
        }
    }
}