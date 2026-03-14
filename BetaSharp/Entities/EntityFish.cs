using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntityFish : Entity
{
    public EntityPlayer angler;
    public Entity bobber;
    private double field_6383_q;
    private double field_6384_p;
    private double field_6385_o;
    private double field_6386_n;
    private double field_6387_m;
    private int field_6388_l;
    private bool inGround;
    private int inTile;
    public int shake;
    private int ticksCatchable;
    private int ticksInAir;
    private int ticksInGround;
    private double vX;
    private double vY;
    private double vZ;
    private int xTile;
    private int yTile;
    private int zTile;

    public EntityFish(IWorldContext world) : base(world)
    {
        xTile = -1;
        yTile = -1;
        zTile = -1;
        inTile = 0;
        inGround = false;
        shake = 0;
        ticksInAir = 0;
        ticksCatchable = 0;
        bobber = null;
        setBoundingBoxSpacing(0.25F, 0.25F);
        ignoreFrustumCheck = true;
    }

    public EntityFish(IWorldContext world, double var2, double var4, double var6) : this(world)
    {
        setPosition(var2, var4, var6);
        ignoreFrustumCheck = true;
    }

    public EntityFish(IWorldContext world, EntityPlayer player) : base(world)
    {
        xTile = -1;
        yTile = -1;
        zTile = -1;
        inTile = 0;
        inGround = false;
        shake = 0;
        ticksInAir = 0;
        ticksCatchable = 0;
        bobber = null;
        ignoreFrustumCheck = true;
        angler = player;
        angler.fishHook = this;
        setBoundingBoxSpacing(0.25F, 0.25F);
        setPositionAndAnglesKeepPrevAngles(player.x, player.y + 1.62D - player.standingEyeHeight, player.z, player.yaw, player.pitch);
        x -= MathHelper.Cos(yaw / 180.0F * (float)Math.PI) * 0.16F;
        y -= 0.1F;
        z -= MathHelper.Sin(yaw / 180.0F * (float)Math.PI) * 0.16F;
        setPosition(x, y, z);
        standingEyeHeight = 0.0F;
        float var3 = 0.4F;
        velocityX = -MathHelper.Sin(yaw / 180.0F * (float)Math.PI) * MathHelper.Cos(pitch / 180.0F * (float)Math.PI) * var3;
        velocityZ = MathHelper.Cos(yaw / 180.0F * (float)Math.PI) * MathHelper.Cos(pitch / 180.0F * (float)Math.PI) * var3;
        velocityY = -MathHelper.Sin(pitch / 180.0F * (float)Math.PI) * var3;
        func_4042_a(velocityX, velocityY, velocityZ, 1.5F, 1.0F);
    }


    public override bool shouldRender(double var1)
    {
        double var3 = boundingBox.AverageEdgeLength * 4.0D;
        var3 *= 64.0D;
        return var1 < var3 * var3;
    }

    public void func_4042_a(double var1, double var3, double var5, float var7, float var8)
    {
        float var9 = MathHelper.Sqrt(var1 * var1 + var3 * var3 + var5 * var5);
        var1 /= var9;
        var3 /= var9;
        var5 /= var9;
        var1 += random.NextGaussian() * 0.0075F * var8;
        var3 += random.NextGaussian() * 0.0075F * var8;
        var5 += random.NextGaussian() * 0.0075F * var8;
        var1 *= var7;
        var3 *= var7;
        var5 *= var7;
        velocityX = var1;
        velocityY = var3;
        velocityZ = var5;
        float var10 = MathHelper.Sqrt(var1 * var1 + var5 * var5);
        prevYaw = yaw = (float)(Math.Atan2(var1, var5) * 180.0D / (float)Math.PI);
        prevPitch = pitch = (float)(Math.Atan2(var3, var10) * 180.0D / (float)Math.PI);
        ticksInGround = 0;
    }

    public override void setPositionAndAnglesAvoidEntities(double var1, double var3, double var5, float var7, float var8, int var9)
    {
        field_6387_m = var1;
        field_6386_n = var3;
        field_6385_o = var5;
        field_6384_p = var7;
        field_6383_q = var8;
        field_6388_l = var9;
        velocityX = vX;
        velocityY = vY;
        velocityZ = vZ;
    }

    public override void setVelocityClient(double var1, double var3, double var5)
    {
        vX = velocityX = var1;
        vY = velocityY = var3;
        vZ = velocityZ = var5;
    }

    public override void tick()
    {
        base.tick();
        if (field_6388_l > 0)
        {
            double var21 = x + (field_6387_m - x) / field_6388_l;
            double var22 = y + (field_6386_n - y) / field_6388_l;
            double var23 = z + (field_6385_o - z) / field_6388_l;

            double var7;
            for (var7 = field_6384_p - yaw; var7 < -180.0D; var7 += 360.0D)
            {
            }

            while (var7 >= 180.0D)
            {
                var7 -= 360.0D;
            }

            yaw = (float)(yaw + var7 / field_6388_l);
            pitch = (float)(pitch + (field_6383_q - pitch) / field_6388_l);
            --field_6388_l;
            setPosition(var21, var22, var23);
            setRotation(yaw, pitch);
        }
        else
        {
            if (!world.IsRemote)
            {
                ItemStack var1 = angler.getHand();
                if (angler.dead || !angler.isAlive() || var1 == null || var1.getItem() != Item.FishingRod || getSquaredDistance(angler) > 1024.0D)
                {
                    markDead();
                    angler.fishHook = null;
                    return;
                }

                if (bobber != null)
                {
                    if (!bobber.dead)
                    {
                        x = bobber.x;
                        y = bobber.boundingBox.MinY + bobber.height * 0.8D;
                        z = bobber.z;
                        return;
                    }

                    bobber = null;
                }
            }

            if (shake > 0)
            {
                --shake;
            }

            if (inGround)
            {
                int var19 = world.Reader.GetBlockId(xTile, yTile, zTile);
                if (var19 == inTile)
                {
                    ++ticksInGround;
                    if (ticksInGround == 1200)
                    {
                        markDead();
                    }

                    return;
                }

                inGround = false;
                velocityX *= random.NextFloat() * 0.2F;
                velocityY *= random.NextFloat() * 0.2F;
                velocityZ *= random.NextFloat() * 0.2F;
                ticksInGround = 0;
                ticksInAir = 0;
            }
            else
            {
                ++ticksInAir;
            }

            Vec3D var20 = new(x, y, z);
            Vec3D var2 = new(x + velocityX, y + velocityY, z + velocityZ);
            HitResult var3 = world.Reader.Raycast(var20, var2, false);
            var20 = new Vec3D(x, y, z);
            var2 = new Vec3D(x + velocityX, y + velocityY, z + velocityZ);
            if (var3.Type != HitResultType.MISS)
            {
                var2 = new Vec3D(var3.Pos.x, var3.Pos.y, var3.Pos.z);
            }

            Entity var4 = null;
            List<Entity> var5 = world.Entities.GetEntities(this, boundingBox.Stretch(velocityX, velocityY, velocityZ).Expand(1.0D, 1.0D, 1.0D));
            double var6 = 0.0D;

            double var13;
            for (int var8 = 0; var8 < var5.Count; ++var8)
            {
                Entity var9 = var5[var8];
                if (var9.isCollidable() && (var9 != angler || ticksInAir >= 5))
                {
                    float var10 = 0.3F;
                    Box var11 = var9.boundingBox.Expand(var10, var10, var10);
                    HitResult var12 = var11.Raycast(var20, var2);
                    if (var12.Type != HitResultType.MISS)
                    {
                        var13 = var20.distanceTo(var12.Pos);
                        if (var13 < var6 || var6 == 0.0D)
                        {
                            var4 = var9;
                            var6 = var13;
                        }
                    }
                }
            }

            if (var4 != null)
            {
                var3 = new HitResult(var4);
            }

            if (var3.Type != HitResultType.MISS)
            {
                if (var3.Entity != null)
                {
                    if (var3.Entity.damage(angler, 0))
                    {
                        bobber = var3.Entity;
                    }
                }
                else
                {
                    inGround = true;
                }
            }

            if (!inGround)
            {
                base.move(velocityX, velocityY, velocityZ);
                float var24 = MathHelper.Sqrt(velocityX * velocityX + velocityZ * velocityZ);
                yaw = (float)(Math.Atan2(velocityX, velocityZ) * 180.0D / (float)Math.PI);

                for (pitch = (float)(Math.Atan2(velocityY, var24) * 180.0D / (float)Math.PI); pitch - prevPitch < -180.0F; prevPitch -= 360.0F)
                {
                }

                while (pitch - prevPitch >= 180.0F)
                {
                    prevPitch += 360.0F;
                }

                while (yaw - prevYaw < -180.0F)
                {
                    prevYaw -= 360.0F;
                }

                while (yaw - prevYaw >= 180.0F)
                {
                    prevYaw += 360.0F;
                }

                pitch = prevPitch + (pitch - prevPitch) * 0.2F;
                yaw = prevYaw + (yaw - prevYaw) * 0.2F;
                float var25 = 0.92F;
                if (onGround || horizontalCollison)
                {
                    var25 = 0.5F;
                }

                byte var26 = 5;
                double var27 = 0.0D;

                for (int var28 = 0; var28 < var26; ++var28)
                {
                    double var14 = boundingBox.MinY + (boundingBox.MaxY - boundingBox.MinY) * (var28 + 0) / var26 - 0.125D + 0.125D;
                    double var16 = boundingBox.MinY + (boundingBox.MaxY - boundingBox.MinY) * (var28 + 1) / var26 - 0.125D + 0.125D;
                    Box var18 = new(boundingBox.MinX, var14, boundingBox.MinZ, boundingBox.MaxX, var16, boundingBox.MaxZ);
                    if (world.Reader.IsFluidInBox(var18, Material.Water))
                    {
                        var27 += 1.0D / var26;
                    }
                }

                if (var27 > 0.0D)
                {
                    if (ticksCatchable > 0)
                    {
                        --ticksCatchable;
                    }
                    else
                    {
                        short var29 = 500;
                        if (world.Environment.IsRainingAt(MathHelper.Floor(x), MathHelper.Floor(y) + 1, MathHelper.Floor(z)))
                        {
                            var29 = 300;
                        }

                        if (random.NextInt(var29) == 0)
                        {
                            ticksCatchable = random.NextInt(30) + 10;
                            velocityY -= 0.2F;
                            world.Broadcaster.PlaySoundAtEntity(this, "random.splash", 0.25F, 1.0F + (random.NextFloat() - random.NextFloat()) * 0.4F);
                            float var30 = MathHelper.Floor(boundingBox.MinY);

                            int var15;
                            float var17;
                            float var31;
                            for (var15 = 0; var15 < 1.0F + width * 20.0F; ++var15)
                            {
                                var31 = (random.NextFloat() * 2.0F - 1.0F) * width;
                                var17 = (random.NextFloat() * 2.0F - 1.0F) * width;
                                world.Broadcaster.AddParticle("bubble", x + var31, var30 + 1.0F, z + var17, velocityX, velocityY - random.NextFloat() * 0.2F, velocityZ);
                            }

                            for (var15 = 0; var15 < 1.0F + width * 20.0F; ++var15)
                            {
                                var31 = (random.NextFloat() * 2.0F - 1.0F) * width;
                                var17 = (random.NextFloat() * 2.0F - 1.0F) * width;
                                world.Broadcaster.AddParticle("splash", x + var31, var30 + 1.0F, z + var17, velocityX, velocityY, velocityZ);
                            }
                        }
                    }
                }

                if (ticksCatchable > 0)
                {
                    velocityY -= random.NextFloat() * random.NextFloat() * random.NextFloat() * 0.2D;
                }

                var13 = var27 * 2.0D - 1.0D;
                velocityY += 0.04F * var13;
                if (var27 > 0.0D)
                {
                    var25 = (float)(var25 * 0.9D);
                    velocityY *= 0.8D;
                }

                velocityX *= var25;
                velocityY *= var25;
                velocityZ *= var25;
                setPosition(x, y, z);
            }
        }
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        nbt.SetShort("xTile", (short)xTile);
        nbt.SetShort("yTile", (short)yTile);
        nbt.SetShort("zTile", (short)zTile);
        nbt.SetByte("inTile", (sbyte)inTile);
        nbt.SetByte("shake", (sbyte)shake);
        nbt.SetByte("inGround", (sbyte)(inGround ? 1 : 0));
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        xTile = nbt.GetShort("xTile");
        yTile = nbt.GetShort("yTile");
        zTile = nbt.GetShort("zTile");
        inTile = nbt.GetByte("inTile") & 255;
        shake = nbt.GetByte("shake") & 255;
        inGround = nbt.GetByte("inGround") == 1;
    }

    public override float getShadowRadius() => 0.0F;

    public int catchFish()
    {
        byte var1 = 0;
        if (bobber != null)
        {
            double var2 = angler.x - x;
            double var4 = angler.y - y;
            double var6 = angler.z - z;
            double var8 = MathHelper.Sqrt(var2 * var2 + var4 * var4 + var6 * var6);
            double var10 = 0.1D;
            bobber.velocityX += var2 * var10;
            bobber.velocityY += var4 * var10 + MathHelper.Sqrt(var8) * 0.08D;
            bobber.velocityZ += var6 * var10;
            var1 = 3;
        }
        else if (ticksCatchable > 0)
        {
            EntityItem var13 = new(world, x, y, z, new ItemStack(Item.RawFish));
            double var3 = angler.x - x;
            double var5 = angler.y - y;
            double var7 = angler.z - z;
            double var9 = MathHelper.Sqrt(var3 * var3 + var5 * var5 + var7 * var7);
            double var11 = 0.1D;
            var13.velocityX = var3 * var11;
            var13.velocityY = var5 * var11 + MathHelper.Sqrt(var9) * 0.08D;
            var13.velocityZ = var7 * var11;
            world.SpawnEntity(var13);
            angler.increaseStat(Stats.Stats.FishCaughtStat, 1);
            var1 = 1;
        }

        if (inGround)
        {
            var1 = 2;
        }

        markDead();
        angler.fishHook = null;
        return var1;
    }
}
