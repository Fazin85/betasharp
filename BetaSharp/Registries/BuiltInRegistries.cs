using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Rules;
using BetaSharp.Worlds.Biomes;

namespace BetaSharp.Registries;

public static class BuiltInRegistries
{
    public static readonly IRegistry<EntityType> EntityTypes =
        new MappedRegistry<EntityType>(ResourceLocation.Parse("entity_types"));

    public static readonly IRegistry<Biome> Biomes =
        new MappedRegistry<Biome>(ResourceLocation.Parse("biomes"));

    public static readonly IRegistry<BlockEntityType> BlockEntityTypes =
        new MappedRegistry<BlockEntityType>(ResourceLocation.Parse("block_entity_types"));

    //TODO: Implement this properly
    public static readonly IRegistry<IGameRule> GameRules =
        new MappedRegistry<IGameRule>(ResourceLocation.Parse("game_rules"));

    public static void Initialize()
    {
        EntityTypes.Bootstrap(typeof(EntityRegistry));
        Biomes.Bootstrap(typeof(Biome));
        BlockEntityTypes.Bootstrap(typeof(BlockEntity));

        FreezeAll();
    }

    private static void FreezeAll()
    {
        EntityTypes.Freeze();
        Biomes.Freeze();
        BlockEntityTypes.Freeze();
    }
}
