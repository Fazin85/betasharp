using betareborn.Blocks;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Hit;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityArrow : Entity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityArrow).TypeHandle);

        private int xTile = -1;
        private int yTile = -1;
        private int zTile = -1;
        private int inTile = 0;
        private int field_28019_h = 0;
        private bool inGround = false;
        public bool doesArrowBelongToPlayer = false;
        public int arrowShake = 0;
        public EntityLiving owner;
        private int ticksInGround;
        private int ticksInAir = 0;

        public EntityArrow(World var1) : base(var1)
        {
            setBoundingBoxSpacing(0.5F, 0.5F);
        }

        public EntityArrow(World var1, double var2, double var4, double var6) : base(var1)
        {
            setBoundingBoxSpacing(0.5F, 0.5F);
            setPosition(var2, var4, var6);
            standingEyeHeight = 0.0F;
        }

        public EntityArrow(World var1, EntityLiving var2) : base(var1)
        {
            owner = var2;
            doesArrowBelongToPlayer = var2 is EntityPlayer;
            setBoundingBoxSpacing(0.5F, 0.5F);
            setPositionAndAnglesKeepPrevAngles(var2.x, var2.y + (double)var2.getEyeHeight(), var2.z, var2.yaw, var2.pitch);
            x -= (double)(MathHelper.cos(yaw / 180.0F * (float)java.lang.Math.PI) * 0.16F);
            y -= (double)0.1F;
            z -= (double)(MathHelper.sin(yaw / 180.0F * (float)java.lang.Math.PI) * 0.16F);
            setPosition(x, y, z);
            standingEyeHeight = 0.0F;
            velocityX = (double)(-MathHelper.sin(yaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(pitch / 180.0F * (float)java.lang.Math.PI));
            velocityZ = (double)(MathHelper.cos(yaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(pitch / 180.0F * (float)java.lang.Math.PI));
            velocityY = (double)(-MathHelper.sin(pitch / 180.0F * (float)java.lang.Math.PI));
            setArrowHeading(velocityX, velocityY, velocityZ, 1.5F, 1.0F);
        }

        protected override void initDataTracker()
        {
        }

        public void setArrowHeading(double var1, double var3, double var5, float var7, float var8)
        {
            float var9 = MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5);
            var1 /= (double)var9;
            var3 /= (double)var9;
            var5 /= (double)var9;
            var1 += random.nextGaussian() * (double)0.0075F * (double)var8;
            var3 += random.nextGaussian() * (double)0.0075F * (double)var8;
            var5 += random.nextGaussian() * (double)0.0075F * (double)var8;
            var1 *= (double)var7;
            var3 *= (double)var7;
            var5 *= (double)var7;
            velocityX = var1;
            velocityY = var3;
            velocityZ = var5;
            float var10 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
            prevYaw = yaw = (float)(java.lang.Math.atan2(var1, var5) * 180.0D / (double)((float)java.lang.Math.PI));
            prevPitch = pitch = (float)(java.lang.Math.atan2(var3, (double)var10) * 180.0D / (double)((float)java.lang.Math.PI));
            ticksInGround = 0;
        }

        public override void setVelocityClient(double var1, double var3, double var5)
        {
            velocityX = var1;
            velocityY = var3;
            velocityZ = var5;
            if (prevPitch == 0.0F && prevYaw == 0.0F)
            {
                float var7 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
                prevYaw = yaw = (float)(java.lang.Math.atan2(var1, var5) * 180.0D / (double)((float)java.lang.Math.PI));
                prevPitch = pitch = (float)(java.lang.Math.atan2(var3, (double)var7) * 180.0D / (double)((float)java.lang.Math.PI));
                prevPitch = pitch;
                prevYaw = yaw;
                setPositionAndAnglesKeepPrevAngles(x, y, z, yaw, pitch);
                ticksInGround = 0;
            }

        }

        public override void tick()
        {
            base.tick();
            if (prevPitch == 0.0F && prevYaw == 0.0F)
            {
                float var1 = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
                prevYaw = yaw = (float)(java.lang.Math.atan2(velocityX, velocityZ) * 180.0D / (double)((float)java.lang.Math.PI));
                prevPitch = pitch = (float)(java.lang.Math.atan2(velocityY, (double)var1) * 180.0D / (double)((float)java.lang.Math.PI));
            }

            int var15 = world.getBlockId(xTile, yTile, zTile);
            if (var15 > 0)
            {
                Block.BLOCKS[var15].updateBoundingBox(world, xTile, yTile, zTile);
                Box? var2 = Block.BLOCKS[var15].getCollisionShape(world, xTile, yTile, zTile);
                if (var2 != null && var2.Value.contains(new Vec3D(x, y, z)))
                {
                    inGround = true;
                }
            }

            if (arrowShake > 0)
            {
                --arrowShake;
            }

            if (inGround)
            {
                var15 = world.getBlockId(xTile, yTile, zTile);
                int var18 = world.getBlockMeta(xTile, yTile, zTile);
                if (var15 == inTile && var18 == field_28019_h)
                {
                    ++ticksInGround;
                    if (ticksInGround == 1200)
                    {
                        markDead();
                    }

                }
                else
                {
                    inGround = false;
                    velocityX *= (double)(random.nextFloat() * 0.2F);
                    velocityY *= (double)(random.nextFloat() * 0.2F);
                    velocityZ *= (double)(random.nextFloat() * 0.2F);
                    ticksInGround = 0;
                    ticksInAir = 0;
                }
            }
            else
            {
                ++ticksInAir;
                Vec3D var16 = new Vec3D(x, y, z);
                Vec3D var17 = new Vec3D(x + velocityX, y + velocityY, z + velocityZ);
                HitResult var3 = world.raycast(var16, var17, false, true);
                var16 = new Vec3D(x, y, z);
                var17 = new Vec3D(x + velocityX, y + velocityY, z + velocityZ);
                if (var3 != null)
                {
                    var17 = new Vec3D(var3.pos.x, var3.pos.y, var3.pos.z);
                }

                Entity var4 = null;
                var var5 = world.getEntities(this, boundingBox.stretch(velocityX, velocityY, velocityZ).expand(1.0D, 1.0D, 1.0D));
                double var6 = 0.0D;

                float var10;
                for (int var8 = 0; var8 < var5.Count; ++var8)
                {
                    Entity var9 = var5[var8];
                    if (var9.isCollidable() && (var9 != owner || ticksInAir >= 5))
                    {
                        var10 = 0.3F;
                        Box var11 = var9.boundingBox.expand((double)var10, (double)var10, (double)var10);
                        HitResult var12 = var11.raycast(var16, var17);
                        if (var12 != null)
                        {
                            double var13 = var16.distanceTo(var12.pos);
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

                float var19;
                if (var3 != null)
                {
                    if (var3.entity != null)
                    {
                        if (var3.entity.damage(owner, 4))
                        {
                            world.playSound(this, "random.drr", 1.0F, 1.2F / (random.nextFloat() * 0.2F + 0.9F));
                            markDead();
                        }
                        else
                        {
                            velocityX *= (double)-0.1F;
                            velocityY *= (double)-0.1F;
                            velocityZ *= (double)-0.1F;
                            yaw += 180.0F;
                            prevYaw += 180.0F;
                            ticksInAir = 0;
                        }
                    }
                    else
                    {
                        xTile = var3.blockX;
                        yTile = var3.blockY;
                        zTile = var3.blockZ;
                        inTile = world.getBlockId(xTile, yTile, zTile);
                        field_28019_h = world.getBlockMeta(xTile, yTile, zTile);
                        velocityX = (double)((float)(var3.pos.x - x));
                        velocityY = (double)((float)(var3.pos.y - y));
                        velocityZ = (double)((float)(var3.pos.z - z));
                        var19 = MathHelper.sqrt_double(velocityX * velocityX + velocityY * velocityY + velocityZ * velocityZ);
                        x -= velocityX / (double)var19 * (double)0.05F;
                        y -= velocityY / (double)var19 * (double)0.05F;
                        z -= velocityZ / (double)var19 * (double)0.05F;
                        world.playSound(this, "random.drr", 1.0F, 1.2F / (random.nextFloat() * 0.2F + 0.9F));
                        inGround = true;
                        arrowShake = 7;
                    }
                }

                x += velocityX;
                y += velocityY;
                z += velocityZ;
                var19 = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
                yaw = (float)(java.lang.Math.atan2(velocityX, velocityZ) * 180.0D / (double)((float)java.lang.Math.PI));

                for (pitch = (float)(java.lang.Math.atan2(velocityY, (double)var19) * 180.0D / (double)((float)java.lang.Math.PI)); pitch - prevPitch < -180.0F; prevPitch -= 360.0F)
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
                float var20 = 0.99F;
                var10 = 0.03F;
                if (isInWater())
                {
                    for (int var21 = 0; var21 < 4; ++var21)
                    {
                        float var22 = 0.25F;
                        world.addParticle("bubble", x - velocityX * (double)var22, y - velocityY * (double)var22, z - velocityZ * (double)var22, velocityX, velocityY, velocityZ);
                    }

                    var20 = 0.8F;
                }

                velocityX *= (double)var20;
                velocityY *= (double)var20;
                velocityZ *= (double)var20;
                velocityY -= (double)var10;
                setPosition(x, y, z);
            }
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            var1.setShort("xTile", (short)xTile);
            var1.setShort("yTile", (short)yTile);
            var1.setShort("zTile", (short)zTile);
            var1.setByte("inTile", (sbyte)inTile);
            var1.setByte("inData", (sbyte)field_28019_h);
            var1.setByte("shake", (sbyte)arrowShake);
            var1.setByte("inGround", (sbyte)(inGround ? 1 : 0));
            var1.setBoolean("player", doesArrowBelongToPlayer);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            xTile = var1.getShort("xTile");
            yTile = var1.getShort("yTile");
            zTile = var1.getShort("zTile");
            inTile = var1.getByte("inTile") & 255;
            field_28019_h = var1.getByte("inData") & 255;
            arrowShake = var1.getByte("shake") & 255;
            inGround = var1.getByte("inGround") == 1;
            doesArrowBelongToPlayer = var1.getBoolean("player");
        }

        public override void onPlayerInteraction(EntityPlayer var1)
        {
            if (!world.isRemote)
            {
                if (inGround && doesArrowBelongToPlayer && arrowShake <= 0 && var1.inventory.addItemStackToInventory(new ItemStack(Item.ARROW, 1)))
                {
                    world.playSound(this, "random.pop", 0.2F, ((random.nextFloat() - random.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                    var1.sendPickup(this, 1);
                    markDead();
                }

            }
        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }
    }

}