using betareborn.Blocks;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
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
        private bool yawFlipped;
        public int type;
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
            yawFlipped = false;
            preventEntitySpawning = true;
            setBoundingBoxSpacing(0.98F, 0.7F);
            standingEyeHeight = height / 2.0F;
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        protected override void initDataTracker()
        {
        }

        public override Box? getCollisionAgainstShape(Entity var1)
        {
            return var1.boundingBox;
        }

        public override Box? getBoundingBox()
        {
            return null;
        }

        public override bool isPushable()
        {
            return true;
        }

        public EntityMinecart(World var1, double var2, double var4, double var6, int var8) : base(var1)
        {
            setPosition(var2, var4 + (double)standingEyeHeight, var6);
            velocityX = 0.0D;
            velocityY = 0.0D;
            velocityZ = 0.0D;
            prevX = var2;
            prevY = var4;
            prevZ = var6;
            type = var8;
        }

        public override double getPassengerRidingHeight()
        {
            return (double)height * 0.0D - (double)0.3F;
        }

        public override bool damage(Entity var1, int var2)
        {
            if (!world.isRemote && !dead)
            {
                minecartRockDirection = -minecartRockDirection;
                minecartTimeSinceHit = 10;
                scheduleVelocityUpdate();
                minecartCurrentDamage += var2 * 10;
                if (minecartCurrentDamage > 40)
                {
                    if (passenger != null)
                    {
                        passenger.setVehicle(this);
                    }

                    markDead();
                    dropItem(Item.MINECART.id, 1, 0.0F);
                    if (type == 1)
                    {
                        EntityMinecart var3 = this;

                        for (int var4 = 0; var4 < var3.size(); ++var4)
                        {
                            ItemStack var5 = var3.getStack(var4);
                            if (var5 != null)
                            {
                                float var6 = random.nextFloat() * 0.8F + 0.1F;
                                float var7 = random.nextFloat() * 0.8F + 0.1F;
                                float var8 = random.nextFloat() * 0.8F + 0.1F;

                                while (var5.count > 0)
                                {
                                    int var9 = random.nextInt(21) + 10;
                                    if (var9 > var5.count)
                                    {
                                        var9 = var5.count;
                                    }

                                    var5.count -= var9;
                                    EntityItem var10 = new EntityItem(world, x + (double)var6, y + (double)var7, z + (double)var8, new ItemStack(var5.itemId, var9, var5.getDamage()));
                                    float var11 = 0.05F;
                                    var10.velocityX = (double)((float)random.nextGaussian() * var11);
                                    var10.velocityY = (double)((float)random.nextGaussian() * var11 + 0.2F);
                                    var10.velocityZ = (double)((float)random.nextGaussian() * var11);
                                    world.spawnEntity(var10);
                                }
                            }
                        }

                        dropItem(Block.CHEST.id, 1, 0.0F);
                    }
                    else if (type == 2)
                    {
                        dropItem(Block.FURNACE.id, 1, 0.0F);
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public override void animateHurt()
        {
            java.lang.System.@out.println("Animating hurt");
            minecartRockDirection = -minecartRockDirection;
            minecartTimeSinceHit = 10;
            minecartCurrentDamage += minecartCurrentDamage * 10;
        }

        public override bool isCollidable()
        {
            return !dead;
        }

        public override void markDead()
        {
            for (int var1 = 0; var1 < size(); ++var1)
            {
                ItemStack var2 = getStack(var1);
                if (var2 != null)
                {
                    float var3 = random.nextFloat() * 0.8F + 0.1F;
                    float var4 = random.nextFloat() * 0.8F + 0.1F;
                    float var5 = random.nextFloat() * 0.8F + 0.1F;

                    while (var2.count > 0)
                    {
                        int var6 = random.nextInt(21) + 10;
                        if (var6 > var2.count)
                        {
                            var6 = var2.count;
                        }

                        var2.count -= var6;
                        EntityItem var7 = new EntityItem(world, x + (double)var3, y + (double)var4, z + (double)var5, new ItemStack(var2.itemId, var6, var2.getDamage()));
                        float var8 = 0.05F;
                        var7.velocityX = (double)((float)random.nextGaussian() * var8);
                        var7.velocityY = (double)((float)random.nextGaussian() * var8 + 0.2F);
                        var7.velocityZ = (double)((float)random.nextGaussian() * var8);
                        world.spawnEntity(var7);
                    }
                }
            }

            base.markDead();
        }

        public override void tick()
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
            if (world.isRemote && field_9415_k > 0)
            {
                if (field_9415_k > 0)
                {
                    double var46 = x + (field_9414_l - x) / (double)field_9415_k;
                    double var47 = y + (field_9413_m - y) / (double)field_9415_k;
                    double var5 = z + (field_9412_n - z) / (double)field_9415_k;

                    for (var7 = field_9411_o - (double)yaw; var7 < -180.0D; var7 += 360.0D)
                    {
                    }

                    while (var7 >= 180.0D)
                    {
                        var7 -= 360.0D;
                    }

                    yaw = (float)((double)yaw + var7 / (double)field_9415_k);
                    pitch = (float)((double)pitch + (field_9410_p - (double)pitch) / (double)field_9415_k);
                    --field_9415_k;
                    setPosition(var46, var47, var5);
                    setRotation(yaw, pitch);
                }
                else
                {
                    setPosition(x, y, z);
                    setRotation(yaw, pitch);
                }

            }
            else
            {
                prevX = x;
                prevY = y;
                prevZ = z;
                velocityY -= (double)0.04F;
                int var1 = MathHelper.floor_double(x);
                int var2 = MathHelper.floor_double(y);
                int var3 = MathHelper.floor_double(z);
                if (BlockRail.isRail(world, var1, var2 - 1, var3))
                {
                    --var2;
                }

                double var4 = 0.4D;
                bool var6 = false;
                var7 = 1.0D / 128.0D;
                int var9 = world.getBlockId(var1, var2, var3);
                if (BlockRail.isRail(var9))
                {
                    Vec3D var10 = func_514_g(x, y, z);
                    int var11 = world.getBlockMeta(var1, var2, var3);
                    y = (double)var2;
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
                        y = (double)(var2 + 1);
                    }

                    if (var11 == 2)
                    {
                        velocityX -= var7;
                    }

                    if (var11 == 3)
                    {
                        velocityX += var7;
                    }

                    if (var11 == 4)
                    {
                        velocityZ += var7;
                    }

                    if (var11 == 5)
                    {
                        velocityZ -= var7;
                    }

                    int[][] var14 = field_855_j[var11];
                    double var15 = (double)(var14[1][0] - var14[0][0]);
                    double var17 = (double)(var14[1][2] - var14[0][2]);
                    double var19 = java.lang.Math.sqrt(var15 * var15 + var17 * var17);
                    double var21 = velocityX * var15 + velocityZ * var17;
                    if (var21 < 0.0D)
                    {
                        var15 = -var15;
                        var17 = -var17;
                    }

                    double var23 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                    velocityX = var23 * var15 / var19;
                    velocityZ = var23 * var17 / var19;
                    double var25;
                    if (var13)
                    {
                        var25 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                        if (var25 < 0.03D)
                        {
                            velocityX *= 0.0D;
                            velocityY *= 0.0D;
                            velocityZ *= 0.0D;
                        }
                        else
                        {
                            velocityX *= 0.5D;
                            velocityY *= 0.0D;
                            velocityZ *= 0.5D;
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
                        x = (double)var1 + 0.5D;
                        var25 = z - (double)var3;
                    }
                    else if (var17 == 0.0D)
                    {
                        z = (double)var3 + 0.5D;
                        var25 = x - (double)var1;
                    }
                    else
                    {
                        var35 = x - var27;
                        var37 = z - var29;
                        var39 = (var35 * var15 + var37 * var17) * 2.0D;
                        var25 = var39;
                    }

                    x = var27 + var15 * var25;
                    z = var29 + var17 * var25;
                    setPosition(x, y + (double)standingEyeHeight, z);
                    var35 = velocityX;
                    var37 = velocityZ;
                    if (passenger != null)
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

                    move(var35, 0.0D, var37);
                    if (var14[0][1] != 0 && MathHelper.floor_double(x) - var1 == var14[0][0] && MathHelper.floor_double(z) - var3 == var14[0][2])
                    {
                        setPosition(x, y + (double)var14[0][1], z);
                    }
                    else if (var14[1][1] != 0 && MathHelper.floor_double(x) - var1 == var14[1][0] && MathHelper.floor_double(z) - var3 == var14[1][2])
                    {
                        setPosition(x, y + (double)var14[1][1], z);
                    }

                    if (passenger != null)
                    {
                        velocityX *= (double)0.997F;
                        velocityY *= 0.0D;
                        velocityZ *= (double)0.997F;
                    }
                    else
                    {
                        if (type == 2)
                        {
                            var39 = (double)MathHelper.sqrt_double(pushX * pushX + pushZ * pushZ);
                            if (var39 > 0.01D)
                            {
                                var6 = true;
                                pushX /= var39;
                                pushZ /= var39;
                                double var41 = 0.04D;
                                velocityX *= (double)0.8F;
                                velocityY *= 0.0D;
                                velocityZ *= (double)0.8F;
                                velocityX += pushX * var41;
                                velocityZ += pushZ * var41;
                            }
                            else
                            {
                                velocityX *= (double)0.9F;
                                velocityY *= 0.0D;
                                velocityZ *= (double)0.9F;
                            }
                        }

                        velocityX *= (double)0.96F;
                        velocityY *= 0.0D;
                        velocityZ *= (double)0.96F;
                    }

                    Vec3D var52 = func_514_g(x, y, z);
                    if (var52 != null && var10 != null)
                    {
                        double var40 = (var10.yCoord - var52.yCoord) * 0.05D;
                        var23 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                        if (var23 > 0.0D)
                        {
                            velocityX = velocityX / var23 * (var23 + var40);
                            velocityZ = velocityZ / var23 * (var23 + var40);
                        }

                        setPosition(x, var52.yCoord, z);
                    }

                    int var53 = MathHelper.floor_double(x);
                    int var54 = MathHelper.floor_double(z);
                    if (var53 != var1 || var54 != var3)
                    {
                        var23 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                        velocityX = var23 * (double)(var53 - var1);
                        velocityZ = var23 * (double)(var54 - var3);
                    }

                    double var42;
                    if (type == 2)
                    {
                        var42 = (double)MathHelper.sqrt_double(pushX * pushX + pushZ * pushZ);
                        if (var42 > 0.01D && velocityX * velocityX + velocityZ * velocityZ > 0.001D)
                        {
                            pushX /= var42;
                            pushZ /= var42;
                            if (pushX * velocityX + pushZ * velocityZ < 0.0D)
                            {
                                pushX = 0.0D;
                                pushZ = 0.0D;
                            }
                            else
                            {
                                pushX = velocityX;
                                pushZ = velocityZ;
                            }
                        }
                    }

                    if (var12)
                    {
                        var42 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                        if (var42 > 0.01D)
                        {
                            double var44 = 0.06D;
                            velocityX += velocityX / var42 * var44;
                            velocityZ += velocityZ / var42 * var44;
                        }
                        else if (var11 == 1)
                        {
                            if (world.shouldSuffocate(var1 - 1, var2, var3))
                            {
                                velocityX = 0.02D;
                            }
                            else if (world.shouldSuffocate(var1 + 1, var2, var3))
                            {
                                velocityX = -0.02D;
                            }
                        }
                        else if (var11 == 0)
                        {
                            if (world.shouldSuffocate(var1, var2, var3 - 1))
                            {
                                velocityZ = 0.02D;
                            }
                            else if (world.shouldSuffocate(var1, var2, var3 + 1))
                            {
                                velocityZ = -0.02D;
                            }
                        }
                    }
                }
                else
                {
                    if (velocityX < -var4)
                    {
                        velocityX = -var4;
                    }

                    if (velocityX > var4)
                    {
                        velocityX = var4;
                    }

                    if (velocityZ < -var4)
                    {
                        velocityZ = -var4;
                    }

                    if (velocityZ > var4)
                    {
                        velocityZ = var4;
                    }

                    if (onGround)
                    {
                        velocityX *= 0.5D;
                        velocityY *= 0.5D;
                        velocityZ *= 0.5D;
                    }

                    move(velocityX, velocityY, velocityZ);
                    if (!onGround)
                    {
                        velocityX *= (double)0.95F;
                        velocityY *= (double)0.95F;
                        velocityZ *= (double)0.95F;
                    }
                }

                pitch = 0.0F;
                double var48 = prevX - x;
                double var49 = prevZ - z;
                if (var48 * var48 + var49 * var49 > 0.001D)
                {
                    yaw = (float)(java.lang.Math.atan2(var49, var48) * 180.0D / java.lang.Math.PI);
                    if (yawFlipped)
                    {
                        yaw += 180.0F;
                    }
                }

                double var50;
                for (var50 = (double)(yaw - prevYaw); var50 >= 180.0D; var50 -= 360.0D)
                {
                }

                while (var50 < -180.0D)
                {
                    var50 += 360.0D;
                }

                if (var50 < -170.0D || var50 >= 170.0D)
                {
                    yaw += 180.0F;
                    yawFlipped = !yawFlipped;
                }

                setRotation(yaw, pitch);
                var var16 = world.getEntities(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
                if (var16 != null && var16.Count > 0)
                {
                    for (int var51 = 0; var51 < var16.Count; ++var51)
                    {
                        Entity var18 = var16[var51];
                        if (var18 != passenger && var18.isPushable() && var18 is EntityMinecart)
                        {
                            var18.onCollision(this);
                        }
                    }
                }

                if (passenger != null && passenger.dead)
                {
                    passenger = null;
                }

                if (var6 && random.nextInt(4) == 0)
                {
                    --fuel;
                    if (fuel < 0)
                    {
                        pushX = pushZ = 0.0D;
                    }

                    world.addParticle("largesmoke", x, y + 0.8D, z, 0.0D, 0.0D, 0.0D);
                }

            }
        }

        public Vec3D func_515_a(double var1, double var3, double var5, double var7)
        {
            int var9 = MathHelper.floor_double(var1);
            int var10 = MathHelper.floor_double(var3);
            int var11 = MathHelper.floor_double(var5);
            if (BlockRail.isRail(world, var9, var10 - 1, var11))
            {
                --var10;
            }

            int var12 = world.getBlockId(var9, var10, var11);
            if (!BlockRail.isRail(var12))
            {
                return null;
            }
            else
            {
                int var13 = world.getBlockMeta(var9, var10, var11);
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
            if (BlockRail.isRail(world, var7, var8 - 1, var9))
            {
                --var8;
            }

            int var10 = world.getBlockId(var7, var8, var9);
            if (BlockRail.isRail(var10))
            {
                int var11 = world.getBlockMeta(var7, var8, var9);
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

        public override void writeNbt(NbtTagCompound var1)
        {
            var1.SetInteger("Type", type);
            if (type == 2)
            {
                var1.SetDouble("PushX", pushX);
                var1.SetDouble("PushZ", pushZ);
                var1.SetShort("Fuel", (short)fuel);
            }
            else if (type == 1)
            {
                NBTTagList var2 = new NBTTagList();

                for (int var3 = 0; var3 < cargoItems.Length; ++var3)
                {
                    if (cargoItems[var3] != null)
                    {
                        NbtTagCompound var4 = new NbtTagCompound();
                        var4.SetByte("Slot", (sbyte)var3);
                        cargoItems[var3].writeToNBT(var4);
                        var2.SetTag(var4);
                    }
                }

                var1.SetTag("Items", var2);
            }

        }

        public override void readNbt(NbtTagCompound var1)
        {
            type = var1.GetInteger("Type");
            if (type == 2)
            {
                pushX = var1.GetDouble("PushX");
                pushZ = var1.GetDouble("PushZ");
                fuel = var1.GetShort("Fuel");
            }
            else if (type == 1)
            {
                NBTTagList var2 = var1.GetTagList("Items");
                cargoItems = new ItemStack[size()];

                for (int var3 = 0; var3 < var2.TagCount(); ++var3)
                {
                    NbtTagCompound var4 = (NbtTagCompound)var2.TagAt(var3);
                    int var5 = var4.GetByte("Slot") & 255;
                    if (var5 >= 0 && var5 < cargoItems.Length)
                    {
                        cargoItems[var5] = new ItemStack(var4);
                    }
                }
            }

        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }

        public override void onCollision(Entity var1)
        {
            if (!world.isRemote)
            {
                if (var1 != passenger)
                {
                    if (var1 is EntityLiving && !(var1 is EntityPlayer) && type == 0 && velocityX * velocityX + velocityZ * velocityZ > 0.01D && passenger == null && var1.vehicle == null)
                    {
                        var1.setVehicle(this);
                    }

                    double var2 = var1.x - x;
                    double var4 = var1.z - z;
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
                        var2 *= (double)(1.0F - pushSpeedReduction);
                        var4 *= (double)(1.0F - pushSpeedReduction);
                        var2 *= 0.5D;
                        var4 *= 0.5D;
                        if (var1 is EntityMinecart)
                        {
                            double var10 = var1.x - x;
                            double var12 = var1.z - z;
                            double var14 = var10 * var1.velocityZ + var12 * var1.prevX;
                            var14 *= var14;
                            if (var14 > 5.0D)
                            {
                                return;
                            }

                            double var16 = var1.velocityX + velocityX;
                            double var18 = var1.velocityZ + velocityZ;
                            if (((EntityMinecart)var1).type == 2 && type != 2)
                            {
                                velocityX *= (double)0.2F;
                                velocityZ *= (double)0.2F;
                                addVelocity(var1.velocityX - var2, 0.0D, var1.velocityZ - var4);
                                var1.velocityX *= (double)0.7F;
                                var1.velocityZ *= (double)0.7F;
                            }
                            else if (((EntityMinecart)var1).type != 2 && type == 2)
                            {
                                var1.velocityX *= (double)0.2F;
                                var1.velocityZ *= (double)0.2F;
                                var1.addVelocity(velocityX + var2, 0.0D, velocityZ + var4);
                                velocityX *= (double)0.7F;
                                velocityZ *= (double)0.7F;
                            }
                            else
                            {
                                var16 /= 2.0D;
                                var18 /= 2.0D;
                                velocityX *= (double)0.2F;
                                velocityZ *= (double)0.2F;
                                addVelocity(var16 - var2, 0.0D, var18 - var4);
                                var1.velocityX *= (double)0.2F;
                                var1.velocityZ *= (double)0.2F;
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
                    var3 = cargoItems[var1].split(var2);
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
            if (type == 0)
            {
                if (passenger != null && passenger is EntityPlayer && passenger != var1)
                {
                    return true;
                }

                if (!world.isRemote)
                {
                    var1.setVehicle(this);
                }
            }
            else if (type == 1)
            {
                if (!world.isRemote)
                {
                    var1.openChestScreen(this);
                }
            }
            else if (type == 2)
            {
                ItemStack var2 = var1.inventory.getSelectedItem();
                if (var2 != null && var2.itemId == Item.COAL.id)
                {
                    if (--var2.count == 0)
                    {
                        var1.inventory.setStack(var1.inventory.selectedSlot, (ItemStack)null);
                    }

                    fuel += 1200;
                }

                pushX = x - var1.x;
                pushZ = z - var1.z;
            }

            return true;
        }

        public override void setPositionAndAnglesAvoidEntities(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            field_9414_l = var1;
            field_9413_m = var3;
            field_9412_n = var5;
            field_9411_o = (double)var7;
            field_9410_p = (double)var8;
            field_9415_k = var9 + 2;
            velocityX = field_9409_q;
            velocityY = field_9408_r;
            velocityZ = field_9407_s;
        }

        public override void setVelocityClient(double var1, double var3, double var5)
        {
            field_9409_q = velocityX = var1;
            field_9408_r = velocityY = var3;
            field_9407_s = velocityZ = var5;
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return dead ? false : var1.getSquaredDistance(this) <= 64.0D;
        }
    }

}