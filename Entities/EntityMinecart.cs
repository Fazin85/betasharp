using betareborn.Blocks;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    //TODO: BREAKING MINECART CRASHES THE GAME!!
    public class EntityMinecart : Entity, IInventory
    {
        public static readonly Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityMinecart).TypeHandle);
        private ItemStack[] cargoItems;
        public int minecartCurrentDamage;
        public int minecartTimeSinceHit;
        public int minecartRockDirection;
        private bool field_856_i;
        public int minecartType;
        public int fuel;
        public double pushX;
        public double pushZ;
        private static readonly int[][][] field_855_j =
    [
        [[0, 0, -1], [0, 0, 1]],
    [[-1, 0, 0], [1, 0, 0]],
    [[-1, -1, 0], [1, 0, 0]],
    [[-1, 0, 0], [1, -1, 0]],
    [[0, 0, -1], [0, -1, 1]],
    [[0, -1, -1], [0, 0, 1]],
    [[0, 0, 1], [1, 0, 0]],
    [[0, 0, 1], [-1, 0, 0]],
    [[0, 0, -1], [-1, 0, 0]],
    [[0, 0, -1], [1, 0, 0]]
    ];
        private int field_9415_k;
        private double field_9414_l;
        private double field_9413_m;
        private double field_9412_n;
        private double field_9411_o;
        private double field_9410_p;
        private double field_9409_q;
        private double field_9408_r;
        private double field_9407_s;

        public EntityMinecart(World var1) : base(var1)
        {
            cargoItems = new ItemStack[36];
            minecartCurrentDamage = 0;
            minecartTimeSinceHit = 0;
            minecartRockDirection = 1;
            field_856_i = false;
            preventEntitySpawning = true;
            setSize(0.98F, 0.7F);
            yOffset = height / 2.0F;
        }

        protected override bool canTriggerWalking()
        {
            return false;
        }

        protected override void entityInit()
        {
        }

        public override Box? getCollisionBox(Entity var1)
        {
            return var1.boundingBox;
        }

        public override Box? getBoundingBox()
        {
            return null;
        }

        public override bool canBePushed()
        {
            return true;
        }

        public EntityMinecart(World var1, double var2, double var4, double var6, int var8) : base(var1)
        {
            setPosition(var2, var4 + (double)yOffset, var6);
            motionX = 0.0D;
            motionY = 0.0D;
            motionZ = 0.0D;
            prevPosX = var2;
            prevPosY = var4;
            prevPosZ = var6;
            minecartType = var8;
        }

        public override double getMountedYOffset()
        {
            return (double)height * 0.0D - (double)0.3F;
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            if (!worldObj.isRemote && !isDead)
            {
                minecartRockDirection = -minecartRockDirection;
                minecartTimeSinceHit = 10;
                setBeenAttacked();
                minecartCurrentDamage += var2 * 10;
                if (minecartCurrentDamage > 40)
                {
                    if (riddenByEntity != null)
                    {
                        riddenByEntity.mountEntity(this);
                    }

                    setEntityDead();
                    dropItemWithOffset(Item.minecartEmpty.id, 1, 0.0F);
                    if (minecartType == 1)
                    {
                        EntityMinecart var3 = this;

                        for (int var4 = 0; var4 < var3.size(); ++var4)
                        {
                            ItemStack var5 = var3.getStack(var4);
                            if (var5 != null)
                            {
                                float var6 = rand.nextFloat() * 0.8F + 0.1F;
                                float var7 = rand.nextFloat() * 0.8F + 0.1F;
                                float var8 = rand.nextFloat() * 0.8F + 0.1F;

                                while (var5.count > 0)
                                {
                                    int var9 = rand.nextInt(21) + 10;
                                    if (var9 > var5.count)
                                    {
                                        var9 = var5.count;
                                    }

                                    var5.count -= var9;
                                    EntityItem var10 = new EntityItem(worldObj, posX + (double)var6, posY + (double)var7, posZ + (double)var8, new ItemStack(var5.itemID, var9, var5.getItemDamage()));
                                    float var11 = 0.05F;
                                    var10.motionX = (double)((float)rand.nextGaussian() * var11);
                                    var10.motionY = (double)((float)rand.nextGaussian() * var11 + 0.2F);
                                    var10.motionZ = (double)((float)rand.nextGaussian() * var11);
                                    worldObj.spawnEntity(var10);
                                }
                            }
                        }

                        dropItemWithOffset(Block.CHEST.id, 1, 0.0F);
                    }
                    else if (minecartType == 2)
                    {
                        dropItemWithOffset(Block.FURNACE.id, 1, 0.0F);
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public override void performHurtAnimation()
        {
            java.lang.System.@out.println("Animating hurt");
            minecartRockDirection = -minecartRockDirection;
            minecartTimeSinceHit = 10;
            minecartCurrentDamage += minecartCurrentDamage * 10;
        }

        public override bool canBeCollidedWith()
        {
            return !isDead;
        }

        public override void setEntityDead()
        {
            for (int var1 = 0; var1 < size(); ++var1)
            {
                ItemStack var2 = getStack(var1);
                if (var2 != null)
                {
                    float var3 = rand.nextFloat() * 0.8F + 0.1F;
                    float var4 = rand.nextFloat() * 0.8F + 0.1F;
                    float var5 = rand.nextFloat() * 0.8F + 0.1F;

                    while (var2.count > 0)
                    {
                        int var6 = rand.nextInt(21) + 10;
                        if (var6 > var2.count)
                        {
                            var6 = var2.count;
                        }

                        var2.count -= var6;
                        EntityItem var7 = new EntityItem(worldObj, posX + (double)var3, posY + (double)var4, posZ + (double)var5, new ItemStack(var2.itemID, var6, var2.getItemDamage()));
                        float var8 = 0.05F;
                        var7.motionX = (double)((float)rand.nextGaussian() * var8);
                        var7.motionY = (double)((float)rand.nextGaussian() * var8 + 0.2F);
                        var7.motionZ = (double)((float)rand.nextGaussian() * var8);
                        worldObj.spawnEntity(var7);
                    }
                }
            }

            base.setEntityDead();
        }

        public override void onUpdate()
        {
            if (minecartTimeSinceHit > 0)
            {
                --minecartTimeSinceHit;
            }

            if (minecartCurrentDamage > 0)
            {
                --minecartCurrentDamage;
            }

            double var7;
            if (worldObj.isRemote && field_9415_k > 0)
            {
                if (field_9415_k > 0)
                {
                    double var46 = posX + (field_9414_l - posX) / (double)field_9415_k;
                    double var47 = posY + (field_9413_m - posY) / (double)field_9415_k;
                    double var5 = posZ + (field_9412_n - posZ) / (double)field_9415_k;

                    for (var7 = field_9411_o - (double)rotationYaw; var7 < -180.0D; var7 += 360.0D)
                    {
                    }

                    while (var7 >= 180.0D)
                    {
                        var7 -= 360.0D;
                    }

                    rotationYaw = (float)((double)rotationYaw + var7 / (double)field_9415_k);
                    rotationPitch = (float)((double)rotationPitch + (field_9410_p - (double)rotationPitch) / (double)field_9415_k);
                    --field_9415_k;
                    setPosition(var46, var47, var5);
                    setRotation(rotationYaw, rotationPitch);
                }
                else
                {
                    setPosition(posX, posY, posZ);
                    setRotation(rotationYaw, rotationPitch);
                }

            }
            else
            {
                prevPosX = posX;
                prevPosY = posY;
                prevPosZ = posZ;
                motionY -= (double)0.04F;
                int var1 = MathHelper.floor_double(posX);
                int var2 = MathHelper.floor_double(posY);
                int var3 = MathHelper.floor_double(posZ);
                if (BlockRail.isRail(worldObj, var1, var2 - 1, var3))
                {
                    --var2;
                }

                double var4 = 0.4D;
                bool var6 = false;
                var7 = 1.0D / 128.0D;
                int var9 = worldObj.getBlockId(var1, var2, var3);
                if (BlockRail.isRail(var9))
                {
                    Vec3D var10 = func_514_g(posX, posY, posZ);
                    int var11 = worldObj.getBlockMeta(var1, var2, var3);
                    posY = (double)var2;
                    bool var12 = false;
                    bool var13 = false;
                    if (var9 == Block.POWERED_RAIL.id)
                    {
                        var12 = (var11 & 8) != 0;
                        var13 = !var12;
                    }

                    if (((BlockRail)Block.BLOCKS[var9]).isAlwaysStraight())
                    {
                        var11 &= 7;
                    }

                    if (var11 >= 2 && var11 <= 5)
                    {
                        posY = (double)(var2 + 1);
                    }

                    if (var11 == 2)
                    {
                        motionX -= var7;
                    }

                    if (var11 == 3)
                    {
                        motionX += var7;
                    }

                    if (var11 == 4)
                    {
                        motionZ += var7;
                    }

                    if (var11 == 5)
                    {
                        motionZ -= var7;
                    }

                    int[][] var14 = field_855_j[var11];
                    double var15 = (double)(var14[1][0] - var14[0][0]);
                    double var17 = (double)(var14[1][2] - var14[0][2]);
                    double var19 = java.lang.Math.sqrt(var15 * var15 + var17 * var17);
                    double var21 = motionX * var15 + motionZ * var17;
                    if (var21 < 0.0D)
                    {
                        var15 = -var15;
                        var17 = -var17;
                    }

                    double var23 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                    motionX = var23 * var15 / var19;
                    motionZ = var23 * var17 / var19;
                    double var25;
                    if (var13)
                    {
                        var25 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                        if (var25 < 0.03D)
                        {
                            motionX *= 0.0D;
                            motionY *= 0.0D;
                            motionZ *= 0.0D;
                        }
                        else
                        {
                            motionX *= 0.5D;
                            motionY *= 0.0D;
                            motionZ *= 0.5D;
                        }
                    }

                    var25 = 0.0D;
                    double var27 = (double)var1 + 0.5D + (double)var14[0][0] * 0.5D;
                    double var29 = (double)var3 + 0.5D + (double)var14[0][2] * 0.5D;
                    double var31 = (double)var1 + 0.5D + (double)var14[1][0] * 0.5D;
                    double var33 = (double)var3 + 0.5D + (double)var14[1][2] * 0.5D;
                    var15 = var31 - var27;
                    var17 = var33 - var29;
                    double var35;
                    double var37;
                    double var39;
                    if (var15 == 0.0D)
                    {
                        posX = (double)var1 + 0.5D;
                        var25 = posZ - (double)var3;
                    }
                    else if (var17 == 0.0D)
                    {
                        posZ = (double)var3 + 0.5D;
                        var25 = posX - (double)var1;
                    }
                    else
                    {
                        var35 = posX - var27;
                        var37 = posZ - var29;
                        var39 = (var35 * var15 + var37 * var17) * 2.0D;
                        var25 = var39;
                    }

                    posX = var27 + var15 * var25;
                    posZ = var29 + var17 * var25;
                    setPosition(posX, posY + (double)yOffset, posZ);
                    var35 = motionX;
                    var37 = motionZ;
                    if (riddenByEntity != null)
                    {
                        var35 *= 0.75D;
                        var37 *= 0.75D;
                    }

                    if (var35 < -var4)
                    {
                        var35 = -var4;
                    }

                    if (var35 > var4)
                    {
                        var35 = var4;
                    }

                    if (var37 < -var4)
                    {
                        var37 = -var4;
                    }

                    if (var37 > var4)
                    {
                        var37 = var4;
                    }

                    moveEntity(var35, 0.0D, var37);
                    if (var14[0][1] != 0 && MathHelper.floor_double(posX) - var1 == var14[0][0] && MathHelper.floor_double(posZ) - var3 == var14[0][2])
                    {
                        setPosition(posX, posY + (double)var14[0][1], posZ);
                    }
                    else if (var14[1][1] != 0 && MathHelper.floor_double(posX) - var1 == var14[1][0] && MathHelper.floor_double(posZ) - var3 == var14[1][2])
                    {
                        setPosition(posX, posY + (double)var14[1][1], posZ);
                    }

                    if (riddenByEntity != null)
                    {
                        motionX *= (double)0.997F;
                        motionY *= 0.0D;
                        motionZ *= (double)0.997F;
                    }
                    else
                    {
                        if (minecartType == 2)
                        {
                            var39 = (double)MathHelper.sqrt_double(pushX * pushX + pushZ * pushZ);
                            if (var39 > 0.01D)
                            {
                                var6 = true;
                                pushX /= var39;
                                pushZ /= var39;
                                double var41 = 0.04D;
                                motionX *= (double)0.8F;
                                motionY *= 0.0D;
                                motionZ *= (double)0.8F;
                                motionX += pushX * var41;
                                motionZ += pushZ * var41;
                            }
                            else
                            {
                                motionX *= (double)0.9F;
                                motionY *= 0.0D;
                                motionZ *= (double)0.9F;
                            }
                        }

                        motionX *= (double)0.96F;
                        motionY *= 0.0D;
                        motionZ *= (double)0.96F;
                    }

                    Vec3D var52 = func_514_g(posX, posY, posZ);
                    if (var52 != null && var10 != null)
                    {
                        double var40 = (var10.yCoord - var52.yCoord) * 0.05D;
                        var23 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                        if (var23 > 0.0D)
                        {
                            motionX = motionX / var23 * (var23 + var40);
                            motionZ = motionZ / var23 * (var23 + var40);
                        }

                        setPosition(posX, var52.yCoord, posZ);
                    }

                    int var53 = MathHelper.floor_double(posX);
                    int var54 = MathHelper.floor_double(posZ);
                    if (var53 != var1 || var54 != var3)
                    {
                        var23 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                        motionX = var23 * (double)(var53 - var1);
                        motionZ = var23 * (double)(var54 - var3);
                    }

                    double var42;
                    if (minecartType == 2)
                    {
                        var42 = (double)MathHelper.sqrt_double(pushX * pushX + pushZ * pushZ);
                        if (var42 > 0.01D && motionX * motionX + motionZ * motionZ > 0.001D)
                        {
                            pushX /= var42;
                            pushZ /= var42;
                            if (pushX * motionX + pushZ * motionZ < 0.0D)
                            {
                                pushX = 0.0D;
                                pushZ = 0.0D;
                            }
                            else
                            {
                                pushX = motionX;
                                pushZ = motionZ;
                            }
                        }
                    }

                    if (var12)
                    {
                        var42 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                        if (var42 > 0.01D)
                        {
                            double var44 = 0.06D;
                            motionX += motionX / var42 * var44;
                            motionZ += motionZ / var42 * var44;
                        }
                        else if (var11 == 1)
                        {
                            if (worldObj.shouldSuffocate(var1 - 1, var2, var3))
                            {
                                motionX = 0.02D;
                            }
                            else if (worldObj.shouldSuffocate(var1 + 1, var2, var3))
                            {
                                motionX = -0.02D;
                            }
                        }
                        else if (var11 == 0)
                        {
                            if (worldObj.shouldSuffocate(var1, var2, var3 - 1))
                            {
                                motionZ = 0.02D;
                            }
                            else if (worldObj.shouldSuffocate(var1, var2, var3 + 1))
                            {
                                motionZ = -0.02D;
                            }
                        }
                    }
                }
                else
                {
                    if (motionX < -var4)
                    {
                        motionX = -var4;
                    }

                    if (motionX > var4)
                    {
                        motionX = var4;
                    }

                    if (motionZ < -var4)
                    {
                        motionZ = -var4;
                    }

                    if (motionZ > var4)
                    {
                        motionZ = var4;
                    }

                    if (onGround)
                    {
                        motionX *= 0.5D;
                        motionY *= 0.5D;
                        motionZ *= 0.5D;
                    }

                    moveEntity(motionX, motionY, motionZ);
                    if (!onGround)
                    {
                        motionX *= (double)0.95F;
                        motionY *= (double)0.95F;
                        motionZ *= (double)0.95F;
                    }
                }

                rotationPitch = 0.0F;
                double var48 = prevPosX - posX;
                double var49 = prevPosZ - posZ;
                if (var48 * var48 + var49 * var49 > 0.001D)
                {
                    rotationYaw = (float)(java.lang.Math.atan2(var49, var48) * 180.0D / java.lang.Math.PI);
                    if (field_856_i)
                    {
                        rotationYaw += 180.0F;
                    }
                }

                double var50;
                for (var50 = (double)(rotationYaw - prevRotationYaw); var50 >= 180.0D; var50 -= 360.0D)
                {
                }

                while (var50 < -180.0D)
                {
                    var50 += 360.0D;
                }

                if (var50 < -170.0D || var50 >= 170.0D)
                {
                    rotationYaw += 180.0F;
                    field_856_i = !field_856_i;
                }

                setRotation(rotationYaw, rotationPitch);
                var var16 = worldObj.getEntitiesWithinAABBExcludingEntity(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
                if (var16 != null && var16.Count > 0)
                {
                    for (int var51 = 0; var51 < var16.Count; ++var51)
                    {
                        Entity var18 = var16[var51];
                        if (var18 != riddenByEntity && var18.canBePushed() && var18 is EntityMinecart)
                        {
                            var18.applyEntityCollision(this);
                        }
                    }
                }

                if (riddenByEntity != null && riddenByEntity.isDead)
                {
                    riddenByEntity = null;
                }

                if (var6 && rand.nextInt(4) == 0)
                {
                    --fuel;
                    if (fuel < 0)
                    {
                        pushX = pushZ = 0.0D;
                    }

                    worldObj.addParticle("largesmoke", posX, posY + 0.8D, posZ, 0.0D, 0.0D, 0.0D);
                }

            }
        }

        public Vec3D func_515_a(double var1, double var3, double var5, double var7)
        {
            int var9 = MathHelper.floor_double(var1);
            int var10 = MathHelper.floor_double(var3);
            int var11 = MathHelper.floor_double(var5);
            if (BlockRail.isRail(worldObj, var9, var10 - 1, var11))
            {
                --var10;
            }

            int var12 = worldObj.getBlockId(var9, var10, var11);
            if (!BlockRail.isRail(var12))
            {
                return null;
            }
            else
            {
                int var13 = worldObj.getBlockMeta(var9, var10, var11);
                if (((BlockRail)Block.BLOCKS[var12]).isAlwaysStraight())
                {
                    var13 &= 7;
                }

                var3 = (double)var10;
                if (var13 >= 2 && var13 <= 5)
                {
                    var3 = (double)(var10 + 1);
                }

                int[][] var14 = field_855_j[var13];
                double var15 = (double)(var14[1][0] - var14[0][0]);
                double var17 = (double)(var14[1][2] - var14[0][2]);
                double var19 = java.lang.Math.sqrt(var15 * var15 + var17 * var17);
                var15 /= var19;
                var17 /= var19;
                var1 += var15 * var7;
                var5 += var17 * var7;
                if (var14[0][1] != 0 && MathHelper.floor_double(var1) - var9 == var14[0][0] && MathHelper.floor_double(var5) - var11 == var14[0][2])
                {
                    var3 += (double)var14[0][1];
                }
                else if (var14[1][1] != 0 && MathHelper.floor_double(var1) - var9 == var14[1][0] && MathHelper.floor_double(var5) - var11 == var14[1][2])
                {
                    var3 += (double)var14[1][1];
                }

                return func_514_g(var1, var3, var5);
            }
        }

        public Vec3D func_514_g(double var1, double var3, double var5)
        {
            int var7 = MathHelper.floor_double(var1);
            int var8 = MathHelper.floor_double(var3);
            int var9 = MathHelper.floor_double(var5);
            if (BlockRail.isRail(worldObj, var7, var8 - 1, var9))
            {
                --var8;
            }

            int var10 = worldObj.getBlockId(var7, var8, var9);
            if (BlockRail.isRail(var10))
            {
                int var11 = worldObj.getBlockMeta(var7, var8, var9);
                var3 = (double)var8;
                if (((BlockRail)Block.BLOCKS[var10]).isAlwaysStraight())
                {
                    var11 &= 7;
                }

                if (var11 >= 2 && var11 <= 5)
                {
                    var3 = (double)(var8 + 1);
                }

                int[][] var12 = field_855_j[var11];
                double var13 = 0.0D;
                double var15 = (double)var7 + 0.5D + (double)var12[0][0] * 0.5D;
                double var17 = (double)var8 + 0.5D + (double)var12[0][1] * 0.5D;
                double var19 = (double)var9 + 0.5D + (double)var12[0][2] * 0.5D;
                double var21 = (double)var7 + 0.5D + (double)var12[1][0] * 0.5D;
                double var23 = (double)var8 + 0.5D + (double)var12[1][1] * 0.5D;
                double var25 = (double)var9 + 0.5D + (double)var12[1][2] * 0.5D;
                double var27 = var21 - var15;
                double var29 = (var23 - var17) * 2.0D;
                double var31 = var25 - var19;
                if (var27 == 0.0D)
                {
                    var1 = (double)var7 + 0.5D;
                    var13 = var5 - (double)var9;
                }
                else if (var31 == 0.0D)
                {
                    var5 = (double)var9 + 0.5D;
                    var13 = var1 - (double)var7;
                }
                else
                {
                    double var33 = var1 - var15;
                    double var35 = var5 - var19;
                    double var37 = (var33 * var27 + var35 * var31) * 2.0D;
                    var13 = var37;
                }

                var1 = var15 + var27 * var13;
                var3 = var17 + var29 * var13;
                var5 = var19 + var31 * var13;
                if (var29 < 0.0D)
                {
                    ++var3;
                }

                if (var29 > 0.0D)
                {
                    var3 += 0.5D;
                }

                return Vec3D.createVector(var1, var3, var5);
            }
            else
            {
                return null;
            }
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            var1.setInteger("Type", minecartType);
            if (minecartType == 2)
            {
                var1.setDouble("PushX", pushX);
                var1.setDouble("PushZ", pushZ);
                var1.setShort("Fuel", (short)fuel);
            }
            else if (minecartType == 1)
            {
                NBTTagList var2 = new NBTTagList();

                for (int var3 = 0; var3 < cargoItems.Length; ++var3)
                {
                    if (cargoItems[var3] != null)
                    {
                        NBTTagCompound var4 = new NBTTagCompound();
                        var4.setByte("Slot", (sbyte)var3);
                        cargoItems[var3].writeToNBT(var4);
                        var2.setTag(var4);
                    }
                }

                var1.setTag("Items", var2);
            }

        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            minecartType = var1.getInteger("Type");
            if (minecartType == 2)
            {
                pushX = var1.getDouble("PushX");
                pushZ = var1.getDouble("PushZ");
                fuel = var1.getShort("Fuel");
            }
            else if (minecartType == 1)
            {
                NBTTagList var2 = var1.getTagList("Items");
                cargoItems = new ItemStack[size()];

                for (int var3 = 0; var3 < var2.tagCount(); ++var3)
                {
                    NBTTagCompound var4 = (NBTTagCompound)var2.tagAt(var3);
                    int var5 = var4.getByte("Slot") & 255;
                    if (var5 >= 0 && var5 < cargoItems.Length)
                    {
                        cargoItems[var5] = new ItemStack(var4);
                    }
                }
            }

        }

        public override float getShadowSize()
        {
            return 0.0F;
        }

        public override void applyEntityCollision(Entity var1)
        {
            if (!worldObj.isRemote)
            {
                if (var1 != riddenByEntity)
                {
                    if (var1 is EntityLiving && !(var1 is EntityPlayer) && minecartType == 0 && motionX * motionX + motionZ * motionZ > 0.01D && riddenByEntity == null && var1.ridingEntity == null)
                    {
                        var1.mountEntity(this);
                    }

                    double var2 = var1.posX - posX;
                    double var4 = var1.posZ - posZ;
                    double var6 = var2 * var2 + var4 * var4;
                    if (var6 >= (double)1.0E-4F)
                    {
                        var6 = (double)MathHelper.sqrt_double(var6);
                        var2 /= var6;
                        var4 /= var6;
                        double var8 = 1.0D / var6;
                        if (var8 > 1.0D)
                        {
                            var8 = 1.0D;
                        }

                        var2 *= var8;
                        var4 *= var8;
                        var2 *= (double)0.1F;
                        var4 *= (double)0.1F;
                        var2 *= (double)(1.0F - entityCollisionReduction);
                        var4 *= (double)(1.0F - entityCollisionReduction);
                        var2 *= 0.5D;
                        var4 *= 0.5D;
                        if (var1 is EntityMinecart)
                        {
                            double var10 = var1.posX - posX;
                            double var12 = var1.posZ - posZ;
                            double var14 = var10 * var1.motionZ + var12 * var1.prevPosX;
                            var14 *= var14;
                            if (var14 > 5.0D)
                            {
                                return;
                            }

                            double var16 = var1.motionX + motionX;
                            double var18 = var1.motionZ + motionZ;
                            if (((EntityMinecart)var1).minecartType == 2 && minecartType != 2)
                            {
                                motionX *= (double)0.2F;
                                motionZ *= (double)0.2F;
                                addVelocity(var1.motionX - var2, 0.0D, var1.motionZ - var4);
                                var1.motionX *= (double)0.7F;
                                var1.motionZ *= (double)0.7F;
                            }
                            else if (((EntityMinecart)var1).minecartType != 2 && minecartType == 2)
                            {
                                var1.motionX *= (double)0.2F;
                                var1.motionZ *= (double)0.2F;
                                var1.addVelocity(motionX + var2, 0.0D, motionZ + var4);
                                motionX *= (double)0.7F;
                                motionZ *= (double)0.7F;
                            }
                            else
                            {
                                var16 /= 2.0D;
                                var18 /= 2.0D;
                                motionX *= (double)0.2F;
                                motionZ *= (double)0.2F;
                                addVelocity(var16 - var2, 0.0D, var18 - var4);
                                var1.motionX *= (double)0.2F;
                                var1.motionZ *= (double)0.2F;
                                var1.addVelocity(var16 + var2, 0.0D, var18 + var4);
                            }
                        }
                        else
                        {
                            addVelocity(-var2, 0.0D, -var4);
                            var1.addVelocity(var2 / 4.0D, 0.0D, var4 / 4.0D);
                        }
                    }

                }
            }
        }

        public int size()
        {
            return 27;
        }

        public ItemStack getStack(int var1)
        {
            return cargoItems[var1];
        }

        public ItemStack removeStack(int var1, int var2)
        {
            if (cargoItems[var1] != null)
            {
                ItemStack var3;
                if (cargoItems[var1].count <= var2)
                {
                    var3 = cargoItems[var1];
                    cargoItems[var1] = null;
                    return var3;
                }
                else
                {
                    var3 = cargoItems[var1].splitStack(var2);
                    if (cargoItems[var1].count == 0)
                    {
                        cargoItems[var1] = null;
                    }

                    return var3;
                }
            }
            else
            {
                return null;
            }
        }

        public void setStack(int var1, ItemStack var2)
        {
            cargoItems[var1] = var2;
            if (var2 != null && var2.count > getMaxCountPerStack())
            {
                var2.count = getMaxCountPerStack();
            }

        }

        public string getName()
        {
            return "Minecart";
        }

        public int getMaxCountPerStack()
        {
            return 64;
        }

        public void markDirty()
        {
        }

        public override bool interact(EntityPlayer var1)
        {
            if (minecartType == 0)
            {
                if (riddenByEntity != null && riddenByEntity is EntityPlayer && riddenByEntity != var1)
                {
                    return true;
                }

                if (!worldObj.isRemote)
                {
                    var1.mountEntity(this);
                }
            }
            else if (minecartType == 1)
            {
                if (!worldObj.isRemote)
                {
                    var1.displayGUIChest(this);
                }
            }
            else if (minecartType == 2)
            {
                ItemStack var2 = var1.inventory.getCurrentItem();
                if (var2 != null && var2.itemID == Item.coal.id)
                {
                    if (--var2.count == 0)
                    {
                        var1.inventory.setStack(var1.inventory.currentItem, (ItemStack)null);
                    }

                    fuel += 1200;
                }

                pushX = posX - var1.posX;
                pushZ = posZ - var1.posZ;
            }

            return true;
        }

        public override void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            field_9414_l = var1;
            field_9413_m = var3;
            field_9412_n = var5;
            field_9411_o = (double)var7;
            field_9410_p = (double)var8;
            field_9415_k = var9 + 2;
            motionX = field_9409_q;
            motionY = field_9408_r;
            motionZ = field_9407_s;
        }

        public override void setVelocity(double var1, double var3, double var5)
        {
            field_9409_q = motionX = var1;
            field_9408_r = motionY = var3;
            field_9407_s = motionZ = var5;
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return isDead ? false : var1.getDistanceSqToEntity(this) <= 64.0D;
        }
    }

}