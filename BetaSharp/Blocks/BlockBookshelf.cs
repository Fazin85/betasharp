using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockBookshelf : Block
{
    public BlockBookshelf(int id, string textureId) : base(id, textureId, Material.Wood)
    {
    }

    public override string getTexture(string side) => side switch
    {
        "up" => $"{textureId}_top",
        "down" => $"{textureId}_bottom",
        "north" => $"{textureId}_front",
        "east" => $"{textureId}_right",
        "west" => $"{textureId}_left",
        _ => $"{textureId}_side"  // south = dos
    };


    public override int getDroppedItemCount(JavaRandom random)
    {
        return 0;
    }
}
