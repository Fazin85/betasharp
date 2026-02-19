using BetaSharp.NBT;

namespace BetaSharp.Client.Guis;

public class ServerData
{
    public string Name { get; set; } = "";
    public string Ip { get; set; } = "";
    public string? PopulationInfo { get; set; }
    public string? Motd { get; set; }
    public long Lag { get; set; }
    public bool Polled { get; set; } = false;

    public ServerData(string name, string ip)
    {
        Name = name;
        Ip = ip;
    }

    public NBTTagCompound ToNBT()
    {
        var tag = new NBTTagCompound();
        tag.SetString("name", Name);
        tag.SetString("ip", Ip);
        return tag;
    }

    public static ServerData FromNBT(NBTTagCompound tag)
    {
        return new ServerData(
            tag.GetString("name"),
            tag.GetString("ip")
        );
    }
}
