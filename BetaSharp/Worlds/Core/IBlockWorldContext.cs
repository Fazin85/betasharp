using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.PathFinding;
using BetaSharp.Rules;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;
using BetaSharp.Worlds.Dimensions;
using BetaSharp.Worlds.Mechanics;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Worlds.Core;

public interface IBlockWorldContext
{
    public WorldBlockReader BlocksReader { get; }
    public WorldBlockWrite BlockWriter { get; }
    public BlockHost BlockHost { get; }
    public WorldEventBroadcaster Broadcaster { get; }
    public RedstoneEngine Redstone { get; }
    public EntityManager Entities { get; }
    public LightingEngine Lighting { get; }
    public EnvironmentManager Environment { get; }
    public Dimension dimension { get; }
    public WorldTickScheduler TickScheduler { get; }
    public long Seed { get; }
    public bool IsRemote { get; }
    public RuleSet Rules { get; }
    public PersistentStateManager StateManager { get; }
    public int Difficulty { get; }
    public void SetDifficulty(int difficulty);
    public WorldProperties Properties { get; }
    public JavaRandom random { get; }
    internal PathFinder Pathing { get; }
    public long GetTime();
    public bool SpawnEntity(Entity entity);
    public bool SpawnItemDrop(double x, double y, double z, ItemStack itemStack);
    public bool CanInteract(EntityPlayer player, int x, int y, int z);
    public Explosion CreateExplosion(Entity? source, double x, double y, double z, float power, bool fire);
    public Explosion CreateExplosion(Entity? source, double x, double y, double z, float power);
}
