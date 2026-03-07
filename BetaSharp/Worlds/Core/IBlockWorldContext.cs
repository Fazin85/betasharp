using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Rules;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Core;

public interface IBlockWorldContext
{
    bool isRemote { get; }
    RuleSet Rules { get; }
    JavaRandom random { get; }
    void SpawnEntity(Entity entity);
    void SpawnItemDrop(double x, double y, double z, ItemStack itemStack);
}
