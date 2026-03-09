using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Generation.Biomes.Source;

namespace BetaSharp.Worlds.Core;

/// <summary>
///     Null Object implementation of IBlockAccess for use in contexts where no world is
///     available (inventory rendering, held-item rendering, entity block rendering).
///     Returns safe open-air defaults so all block renderers function without modification.
/// </summary>
public sealed class NullBlockReader : IBlockReader
{
    public static readonly NullBlockReader Instance = new();

    private NullBlockReader()
    {
    }

    public int GetBlockId(int x, int y, int z) => 0;

    public BlockEntity? GetBlockEntity(int x, int y, int z) => null;

    public bool IsOpaque(int x, int y, int z) => false;

    public bool ShouldSuffocate(int x, int y, int z) => false;

    public BiomeSource GetBiomeSource() => null!;
    public int GetMeta(int x, int y, int z) => throw new NotImplementedException();
    public Material GetMaterial(int x, int y, int z) => throw new NotImplementedException();
    public bool IsAir(int x, int y, int z) => throw new NotImplementedException();
    public int GetBrightness(int x, int y, int z) => throw new NotImplementedException();
    public bool IsTopY(int x, int y, int z) => throw new NotImplementedException();
    public int GetTopY(int x, int z) => throw new NotImplementedException();
    public int GetTopSolidBlockY(int x, int z) => throw new NotImplementedException();
    public int GetSpawnPositionValidityY(int x, int z) => throw new NotImplementedException();
    public float GetVisibilityRatio(Vec3D sourcePosition, Box targetBox) => throw new NotImplementedException();
    public HitResult Raycast(Vec3D start, Vec3D end) => throw new NotImplementedException();
    public HitResult Raycast(Vec3D start, Vec3D end, bool includeFluids) => throw new NotImplementedException();
    public HitResult Raycast(Vec3D start, Vec3D target, bool includeFluids, bool ignoreNonSolid) => throw new NotImplementedException();
    public bool IsAnyBlockInBox(Box area) => throw new NotImplementedException();
    public bool IsBoxSubmergedInFluid(Box area) => throw new NotImplementedException();
    public bool IsFireOrLavaInBox(Box area) => throw new NotImplementedException();
    public bool IsMaterialInBox(Box area, Material material) => throw new NotImplementedException();
    public bool IsFluidInBox(Box area, Material fluid) => throw new NotImplementedException();
    public bool UpdateMovementInFluid(Box entityBox, Material fluidMaterial, Entity entity) => throw new NotImplementedException();

    public float GetNaturalBrightness(int x, int y, int z, int blockLight) => 1.0f;

    public float GetLuminance(int x, int y, int z) => 1.0f;

    public int getBlockMeta(int x, int y, int z) => 0;

    public Material getMaterial(int x, int y, int z) => Material.Air;
    public bool IsPosLoaded(int x, int y, int z) => throw new NotImplementedException();
}
