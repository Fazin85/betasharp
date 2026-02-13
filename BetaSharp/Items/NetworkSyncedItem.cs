using BetaSharp.Entities;
using BetaSharp.Network.Packets;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class NetworkSyncedItem : Item
{
    public NetworkSyncedItem(int id) : base(id)
    {
    }

    public override bool isNetworkSynced()
    {
        return true;
    }

    public virtual Packet getUpdatePacket(ItemStack stack, World world, EntityPlayer player)
    {
        return null;
    }
}