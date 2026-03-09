using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockSponge : Block
{
    public BlockSponge(int id) : base(id, Material.Sponge) => textureId = 48;

    public override void onPlaced(OnPlacedEvt evt)
    {
        sbyte radius = 2;

        for (int checkX = evt.X - radius; checkX <= evt.X + radius; ++checkX)
        {
            for (int checkY = evt.Y - radius; checkY <= evt.Y + radius; ++checkY)
            {
                for (int checkZ = evt.Z - radius; checkZ <= evt.Z + radius; ++checkZ)
                {
                    if (evt.Level.BlocksReader.GetMaterial(checkX, checkY, checkZ) == Material.Water)
                    {
                    }
                }
            }
        }
    }

    public override void onBreak(OnBreakEvt evt)
    {
        sbyte radius = 2;

        for (int checkX = evt.X - radius; checkX <= evt.X + radius; ++checkX)
        {
            for (int checkY = evt.Y - radius; checkY <= evt.Y + radius; ++checkY)
            {
                for (int checkZ = evt.Z - radius; checkZ <= evt.Z + radius; ++checkZ)
                {
                    evt.Level.Broadcaster.NotifyNeighbors(checkX, checkY, checkZ, evt.Level.BlocksReader.GetBlockId(checkX, checkY, checkZ));
                }
            }
        }
    }
}
