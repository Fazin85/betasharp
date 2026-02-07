using betareborn.Blocks;
using betareborn.Chunks;
using betareborn.Entities;
using betareborn.Worlds;
using java.util;

namespace betareborn
{
    public class Explosion : java.lang.Object
    {
        public bool isFlaming = false;
        private java.util.Random ExplosionRNG = new();
        private World worldObj;
        public double explosionX;
        public double explosionY;
        public double explosionZ;
        public Entity exploder;
        public float explosionSize;
        public Set destroyedBlockPositions = new HashSet();

        public Explosion(World var1, Entity var2, double var3, double var5, double var7, float var9)
        {
            worldObj = var1;
            exploder = var2;
            explosionSize = var9;
            explosionX = var3;
            explosionY = var5;
            explosionZ = var7;
        }

        public void doExplosionA()
        {
            float var1 = explosionSize;
            byte var2 = 16;

            int var3;
            int var4;
            int var5;
            double var15;
            double var17;
            double var19;
            for (var3 = 0; var3 < var2; ++var3)
            {
                for (var4 = 0; var4 < var2; ++var4)
                {
                    for (var5 = 0; var5 < var2; ++var5)
                    {
                        if (var3 == 0 || var3 == var2 - 1 || var4 == 0 || var4 == var2 - 1 || var5 == 0 || var5 == var2 - 1)
                        {
                            double var6 = (double)((float)var3 / ((float)var2 - 1.0F) * 2.0F - 1.0F);
                            double var8 = (double)((float)var4 / ((float)var2 - 1.0F) * 2.0F - 1.0F);
                            double var10 = (double)((float)var5 / ((float)var2 - 1.0F) * 2.0F - 1.0F);
                            double var12 = java.lang.Math.sqrt(var6 * var6 + var8 * var8 + var10 * var10);
                            var6 /= var12;
                            var8 /= var12;
                            var10 /= var12;
                            float var14 = explosionSize * (0.7F + worldObj.random.nextFloat() * 0.6F);
                            var15 = explosionX;
                            var17 = explosionY;
                            var19 = explosionZ;

                            for (float var21 = 0.3F; var14 > 0.0F; var14 -= var21 * (12.0F / 16.0F))
                            {
                                int var22 = MathHelper.floor_double(var15);
                                int var23 = MathHelper.floor_double(var17);
                                int var24 = MathHelper.floor_double(var19);
                                int var25 = worldObj.getBlockId(var22, var23, var24);
                                if (var25 > 0)
                                {
                                    var14 -= (Block.BLOCKS[var25].getExplosionResistance(exploder) + 0.3F) * var21;
                                }

                                if (var14 > 0.0F)
                                {
                                    destroyedBlockPositions.add(new ChunkPosition(var22, var23, var24));
                                }

                                var15 += var6 * (double)var21;
                                var17 += var8 * (double)var21;
                                var19 += var10 * (double)var21;
                            }
                        }
                    }
                }
            }

            explosionSize *= 2.0F;
            var3 = MathHelper.floor_double(explosionX - (double)explosionSize - 1.0D);
            var4 = MathHelper.floor_double(explosionX + (double)explosionSize + 1.0D);
            var5 = MathHelper.floor_double(explosionY - (double)explosionSize - 1.0D);
            int var29 = MathHelper.floor_double(explosionY + (double)explosionSize + 1.0D);
            int var7 = MathHelper.floor_double(explosionZ - (double)explosionSize - 1.0D);
            int var30 = MathHelper.floor_double(explosionZ + (double)explosionSize + 1.0D);
            var var9 = worldObj.getEntitiesWithinAABBExcludingEntity(exploder, Box.createCached((double)var3, (double)var5, (double)var7, (double)var4, (double)var29, (double)var30));
            Vec3D var31 = Vec3D.createVector(explosionX, explosionY, explosionZ);

            for (int var11 = 0; var11 < var9.Count; ++var11)
            {
                Entity var33 = var9[var11];
                double var13 = var33.getDistance(explosionX, explosionY, explosionZ) / (double)explosionSize;
                if (var13 <= 1.0D)
                {
                    var15 = var33.posX - explosionX;
                    var17 = var33.posY - explosionY;
                    var19 = var33.posZ - explosionZ;
                    double var39 = (double)MathHelper.sqrt_double(var15 * var15 + var17 * var17 + var19 * var19);
                    var15 /= var39;
                    var17 /= var39;
                    var19 /= var39;
                    double var40 = (double)worldObj.func_675_a(var31, var33.boundingBox);
                    double var41 = (1.0D - var13) * var40;
                    var33.attackEntityFrom(exploder, (int)((var41 * var41 + var41) / 2.0D * 8.0D * (double)explosionSize + 1.0D));
                    var33.motionX += var15 * var41;
                    var33.motionY += var17 * var41;
                    var33.motionZ += var19 * var41;
                }
            }

            explosionSize = var1;
            ArrayList var32 = new ArrayList();
            var32.addAll(destroyedBlockPositions);
            if (isFlaming)
            {
                for (int var34 = var32.size() - 1; var34 >= 0; --var34)
                {
                    ChunkPosition var35 = (ChunkPosition)var32.get(var34);
                    int var36 = var35.x;
                    int var37 = var35.y;
                    int var16 = var35.z;
                    int var38 = worldObj.getBlockId(var36, var37, var16);
                    int var18 = worldObj.getBlockId(var36, var37 - 1, var16);
                    if (var38 == 0 && Block.BLOCKS_OPAQUE[var18] && ExplosionRNG.nextInt(3) == 0)
                    {
                        worldObj.setBlockWithNotify(var36, var37, var16, Block.FIRE.id);
                    }
                }
            }

        }

        public void doExplosionB(bool var1)
        {
            worldObj.playSoundEffect(explosionX, explosionY, explosionZ, "random.explode", 4.0F, (1.0F + (worldObj.random.nextFloat() - worldObj.random.nextFloat()) * 0.2F) * 0.7F);
            ArrayList var2 = new ArrayList();
            var2.addAll(destroyedBlockPositions);

            for (int var3 = var2.size() - 1; var3 >= 0; --var3)
            {
                ChunkPosition var4 = (ChunkPosition)var2.get(var3);
                int var5 = var4.x;
                int var6 = var4.y;
                int var7 = var4.z;
                int var8 = worldObj.getBlockId(var5, var6, var7);
                if (var1)
                {
                    double var9 = (double)((float)var5 + worldObj.random.nextFloat());
                    double var11 = (double)((float)var6 + worldObj.random.nextFloat());
                    double var13 = (double)((float)var7 + worldObj.random.nextFloat());
                    double var15 = var9 - explosionX;
                    double var17 = var11 - explosionY;
                    double var19 = var13 - explosionZ;
                    double var21 = (double)MathHelper.sqrt_double(var15 * var15 + var17 * var17 + var19 * var19);
                    var15 /= var21;
                    var17 /= var21;
                    var19 /= var21;
                    double var23 = 0.5D / (var21 / (double)explosionSize + 0.1D);
                    var23 *= (double)(worldObj.random.nextFloat() * worldObj.random.nextFloat() + 0.3F);
                    var15 *= var23;
                    var17 *= var23;
                    var19 *= var23;
                    worldObj.addParticle("explode", (var9 + explosionX * 1.0D) / 2.0D, (var11 + explosionY * 1.0D) / 2.0D, (var13 + explosionZ * 1.0D) / 2.0D, var15, var17, var19);
                    worldObj.addParticle("smoke", var9, var11, var13, var15, var17, var19);
                }

                if (var8 > 0)
                {
                    Block.BLOCKS[var8].dropStacks(worldObj, var5, var6, var7, worldObj.getBlockMeta(var5, var6, var7), 0.3F);
                    worldObj.setBlockWithNotify(var5, var6, var7, 0);
                    Block.BLOCKS[var8].onBlockDestroyedByExplosion(worldObj, var5, var6, var7);
                }
            }

        }
    }

}