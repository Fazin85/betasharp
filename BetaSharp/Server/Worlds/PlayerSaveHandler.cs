using BetaSharp.Entities;

namespace BetaSharp.Server.Worlds;

public interface PlayerSaveHandler
{
    void savePlayerData(EntityPlayer player);

    void loadPlayerData(EntityPlayer player);
}