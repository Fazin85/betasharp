using betareborn.Blocks;
using betareborn.Blocks.Entities;
using betareborn.Blocks.Materials;
using betareborn.Worlds.Biomes.Source;
using betareborn.Worlds.Chunks;

namespace betareborn.Worlds
{
    public class WorldRegionSnapshot : BlockView, IDisposable
    {
        public BiomeSource BiomeSource { get; }
        public bool IsLit { get; private set; }

        private readonly int chunkX;
        private readonly int chunkZ;
        private readonly ChunkSnapshot[][] chunkArray;
        private readonly float[] lightTable;
        private readonly int skylightSubtracted;

        public WorldRegionSnapshot(World world, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            //TODO: OPTIMIZE THIS
            BiomeSource = new BiomeSource(world);

            chunkX = x1 >> 4;
            chunkZ = z1 >> 4;

            int maxChunkX = x2 >> 4;
            int maxChunkZ = z2 >> 4;

            int sizeX = maxChunkX - chunkX + 1;
            int sizeZ = maxChunkZ - chunkZ + 1;

            chunkArray = new ChunkSnapshot[sizeX][];

            for (int x = 0; x < sizeX; x++)
            {
                chunkArray[x] = new ChunkSnapshot[sizeZ];
            }

            for (int cx = chunkX; cx <= maxChunkX; cx++)
            {
                for (int cz = chunkZ; cz <= maxChunkZ; cz++)
                {
                    chunkArray[cx - chunkX][cz - chunkZ] =
                        new ChunkSnapshot(world.getChunk(cx, cz));
                }
            }

            lightTable = (float[])world.dimension.lightLevelToLuminance.Clone();
            skylightSubtracted = world.ambientDarkness;
        }

        public int GetBlockId(int x, int y, int z)
        {
            if (y is < 0 or >= 128)
                return 0;

            int localChunkX = (x >> 4) - chunkX;
            int localChunkZ = (z >> 4) - chunkZ;

            if (localChunkX < 0 || localChunkZ < 0 ||
                localChunkX >= chunkArray.Length ||
                localChunkZ >= chunkArray[localChunkX].Length)
                return 0;

            var chunk = chunkArray[localChunkX][localChunkZ];
            return chunk?.getBlockID(x & 15, y, z & 15) ?? 0;
        }

        public BlockEntity GetBlockEntity(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public Material GetMaterial(int x, int y, int z)
        {
            int id = GetBlockId(x, y, z);
            return id == 0 ? Material.AIR : Block.BLOCKS[id].material;
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            if (y is < 0 or >= 128)
                return 0;

            int localChunkX = (x >> 4) - chunkX;
            int localChunkZ = (z >> 4) - chunkZ;

            return chunkArray[localChunkX][localChunkZ]
                .getBlockMetadata(x & 15, y, z & 15);
        }


        public float GetLuminance(int x, int y, int z)
            => lightTable[GetLightValue(x, y, z)];

        public float GetNaturalBrightness(int x, int y, int z, int minimum)
        {
            int light = GetLightValue(x, y, z);
            return lightTable[Math.Max(light, minimum)];
        }

        public int GetLightValue(int x, int y, int z)
            => GetLightValueExt(x, y, z, true);

        public int GetLightValueExt(int x, int y, int z, bool checkNeighbors)
        {
            if (x < -32000000 || z < -32000000 ||
                x >= 32000000 || z > 32000000)
                return 15;

            if (checkNeighbors)
            {
                int blockId = GetBlockId(x, y, z);

                if (blockId == Block.SLAB.id ||
                    blockId == Block.FARMLAND.id ||
                    blockId == Block.WOODEN_STAIRS.id ||
                    blockId == Block.COBBLESTONE_STAIRS.id)
                {
                    return Math.Max(
                        GetLightValueExt(x, y + 1, z, false),
                        Math.Max(
                            GetLightValueExt(x + 1, y, z, false),
                            Math.Max(
                                GetLightValueExt(x - 1, y, z, false),
                                Math.Max(
                                    GetLightValueExt(x, y, z + 1, false),
                                    GetLightValueExt(x, y, z - 1, false)
                                )
                            )
                        )
                    );
                }
            }

            if (y < 0)
                return 0;

            if (y >= 128)
                return Math.Max(0, 15 - skylightSubtracted);

            int localChunkX = (x >> 4) - chunkX;
            int localChunkZ = (z >> 4) - chunkZ;

            var chunk = chunkArray[localChunkX][localChunkZ];

            int light = chunk.getBlockLightValue(
                x & 15, y, z & 15, skylightSubtracted);

            if (chunk.getIsLit())
                IsLit = true;

            return light;
        }

        public bool ShouldSuffocate(int x, int y, int z)
        {
            var block = Block.BLOCKS[GetBlockId(x, y, z)];
            return block != null &&
                   block.material.blocksMovement() &&
                   block.isFullCube();
        }

        public bool IsOpaque(int x, int y, int z)
        {
            var block = Block.BLOCKS[GetBlockId(x, y, z)];
            return block != null && block.isOpaque();
        }

        public void Dispose()
        {
            foreach (var column in chunkArray)
            {
                if (column == null) continue;

                foreach (var snapshot in column)
                    snapshot?.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
