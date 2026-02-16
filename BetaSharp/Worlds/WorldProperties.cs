using BetaSharp.Entities;
using BetaSharp.NBT;

namespace BetaSharp.Worlds;

public class WorldProperties
{
    public long RandomSeed { get; }
    public int SpawnX { get; set; }
    public int SpawnY { get; set; }
    public int SpawnZ { get; set; }
    public long WorldTime { get; set; }
    public long LastTimePlayed { get; }
    public long SizeOnDisk { get; set; }
    public NBTTagCompound? PlayerTag { get; set; }
    public int Dimension { get; }
    public string LevelName { get; set; }
    public int SaveVersion { get; set; }
    public bool IsRaining { get; set; }
    public int RainTime { get; set; }
    public bool IsThundering { get; set; }
    public int ThunderTime { get; set; }

    public WorldProperties(NBTTagCompound WorldNBT)
    {
        RandomSeed = WorldNBT.GetLong("RandomSeed");
        SpawnX = WorldNBT.GetInteger("SpawnX");
        SpawnY = WorldNBT.GetInteger("SpawnY");
        SpawnZ = WorldNBT.GetInteger("SpawnZ");
        WorldTime = WorldNBT.GetLong("Time");
        LastTimePlayed = WorldNBT.GetLong("LastPlayed");
        LevelName = WorldNBT.GetString("LevelName");
        SaveVersion = WorldNBT.GetInteger("version");
        RainTime = WorldNBT.GetInteger("rainTime");
        IsRaining = WorldNBT.GetBoolean("raining");
        ThunderTime = WorldNBT.GetInteger("thunderTime");
        IsThundering = WorldNBT.GetBoolean("thundering");
        if (WorldNBT.HasKey("Player"))
        {
            PlayerTag = WorldNBT.GetCompoundTag("Player");
            Dimension = PlayerTag.GetInteger("Dimension");
        }

    }

    public WorldProperties(long RandomSeed, string LevelName)
    {
        this.RandomSeed = RandomSeed;
        this.LevelName = LevelName;
    }

    public WorldProperties(WorldProperties WorldProp)
    {
        RandomSeed = WorldProp.RandomSeed;
        SpawnX = WorldProp.SpawnX;
        SpawnY = WorldProp.SpawnY;
        SpawnZ = WorldProp.SpawnZ;
        WorldTime = WorldProp.WorldTime;
        LastTimePlayed = WorldProp.LastTimePlayed;
        SizeOnDisk = WorldProp.SizeOnDisk;
        PlayerTag = WorldProp.PlayerTag;
        Dimension = WorldProp.Dimension;
        LevelName = WorldProp.LevelName;
        SaveVersion = WorldProp.SaveVersion;
        RainTime = WorldProp.RainTime;
        IsRaining = WorldProp.IsRaining;
        ThunderTime = WorldProp.ThunderTime;
        IsThundering = WorldProp.IsThundering;
    }

    public NBTTagCompound getNBTTagCompound()
    {
        NBTTagCompound NBT = new();
        UpdateTagCompound(NBT, PlayerTag);
        return NBT;
    }

    public NBTTagCompound getNBTTagCompoundWithPlayer(List<EntityPlayer> Players)
    {
        NBTTagCompound NBT = new();
        NBTTagCompound? PlayerNBT = null;

        if (Players.Count > 0 && Players[0] is EntityPlayer player)
        {
            PlayerNBT = new NBTTagCompound();
            player.write(PlayerNBT); // Assuming write is the NBT save method
        }

        UpdateTagCompound(NBT, PlayerNBT);
        return NBT;
    }

    private void UpdateTagCompound(NBTTagCompound WorldNBT, NBTTagCompound PlayerNBT)
    {
        WorldNBT.SetLong("RandomSeed", RandomSeed);
        WorldNBT.SetInteger("SpawnX", SpawnX);
        WorldNBT.SetInteger("SpawnY", SpawnY);
        WorldNBT.SetInteger("SpawnZ", SpawnZ);
        WorldNBT.SetLong("Time", WorldTime);
        WorldNBT.SetLong("SizeOnDisk", SizeOnDisk);

        WorldNBT.SetLong("LastPlayed", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        WorldNBT.SetString("LevelName", LevelName);
        WorldNBT.SetInteger("version", SaveVersion);
        WorldNBT.SetInteger("rainTime", RainTime);
        WorldNBT.SetBoolean("raining", IsRaining);
        WorldNBT.SetInteger("thunderTime", ThunderTime);
        WorldNBT.SetBoolean("thundering", IsThundering);

        if (PlayerNBT != null)
            WorldNBT.SetCompoundTag("Player", PlayerNBT);
    }

    public void SetSpawn(int X, int Y, int Z)
    {
        SpawnX = X;
        SpawnY = Y;
        SpawnZ = Z;
    }
}