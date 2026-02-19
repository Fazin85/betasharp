using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockGravel : BlockSand
{
    public BlockGravel(int i, int j) : base(i, j)
    {
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return random.NextInt(10) == 0 ? Item.Flint.id : id;
    }
}