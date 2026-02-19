using BetaSharp.Entities;

namespace BetaSharp.Server.Worlds;

public interface IPlayerSaveHandler
{
    void SavePlayerData(EntityPlayer player);

    void LoadPlayerData(EntityPlayer player);
}
