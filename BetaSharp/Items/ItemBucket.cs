using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemBucket : Item
{

    private int isFull;

    public ItemBucket(int id, int isFull) : base(id)
    {
        maxCount = 1;
        this.isFull = isFull;
    }

    public override ItemStack use(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        float partialTick = 1.0F;
        float pitch = entityPlayer.prevPitch + (entityPlayer.pitch - entityPlayer.prevPitch) * partialTick;
        float yaw = entityPlayer.prevYaw + (entityPlayer.yaw - entityPlayer.prevYaw) * partialTick;
        double x = entityPlayer.prevX + (entityPlayer.x - entityPlayer.prevX) * (double)partialTick;
        double y = entityPlayer.prevY + (entityPlayer.y - entityPlayer.prevY) * (double)partialTick + 1.62D - (double)entityPlayer.standingEyeHeight;
        double z = entityPlayer.prevZ + (entityPlayer.z - entityPlayer.prevZ) * (double)partialTick;
        Vec3D rayStart = new Vec3D(x, y, z);
        float cosYaw = MathHelper.cos(-yaw * ((float)Math.PI / 180.0F) - (float)Math.PI);
        float sinYaw = MathHelper.sin(-yaw * ((float)Math.PI / 180.0F) - (float)Math.PI);
        float cosPitch = -MathHelper.cos(-pitch * ((float)Math.PI / 180.0F));
        float sinPitch = MathHelper.sin(-pitch * ((float)Math.PI / 180.0F));
        float dirX = sinYaw * cosPitch;
        float dirZ = cosYaw * cosPitch;
        double reachDistance = 5.0D;
        Vec3D rayEnd = rayStart + new Vec3D((double)dirX * reachDistance, (double)sinPitch * reachDistance, (double)dirZ * reachDistance);
        HitResult hitResult = world.raycast(rayStart, rayEnd, isFull == 0);
        if (hitResult == null)
        {
            return itemStack;
        }
        else
        {
            if (hitResult.type == HitResultType.TILE)
            {
                int hitX = hitResult.blockX;
                int hitY = hitResult.blockY;
                int hitZ = hitResult.blockZ;
                if (!world.canInteract(entityPlayer, hitX, hitY, hitZ))
                {
                    return itemStack;
                }

                if (isFull == 0)
                {
                    if (world.getMaterial(hitX, hitY, hitZ) == Material.Water && world.getBlockMeta(hitX, hitY, hitZ) == 0)
                    {
                        world.setBlock(hitX, hitY, hitZ, 0);
                        return new ItemStack(Item.WaterBucket);
                    }

                    if (world.getMaterial(hitX, hitY, hitZ) == Material.Lava && world.getBlockMeta(hitX, hitY, hitZ) == 0)
                    {
                        world.setBlock(hitX, hitY, hitZ, 0);
                        return new ItemStack(Item.LavaBucket);
                    }
                }
                else
                {
                    if (isFull < 0)
                    {
                        return new ItemStack(Item.Bucket);
                    }

                    if (hitResult.side == 0)
                    {
                        --hitY;
                    }

                    if (hitResult.side == 1)
                    {
                        ++hitY;
                    }

                    if (hitResult.side == 2)
                    {
                        --hitZ;
                    }

                    if (hitResult.side == 3)
                    {
                        ++hitZ;
                    }

                    if (hitResult.side == 4)
                    {
                        --hitX;
                    }

                    if (hitResult.side == 5)
                    {
                        ++hitX;
                    }

                    if (world.isAir(hitX, hitY, hitZ) || !world.getMaterial(hitX, hitY, hitZ).IsSolid)
                    {
                        if (world.dimension.evaporatesWater && isFull == Block.FlowingWater.id)
                        {
                            world.playSound(x + 0.5D, y + 0.5D, z + 0.5D, "random.fizz", 0.5F, 2.6F + (world.random.NextFloat() - world.random.NextFloat()) * 0.8F);

                            for (int particleIndex = 0; particleIndex < 8; ++particleIndex)
                            {
                                world.addParticle("largesmoke", (double)hitX + java.lang.Math.random(), (double)hitY + java.lang.Math.random(), (double)hitZ + java.lang.Math.random(), 0.0D, 0.0D, 0.0D);
                            }
                        }
                        else
                        {
                            world.setBlock(hitX, hitY, hitZ, isFull, 0);
                        }

                        return new ItemStack(Item.Bucket);
                    }
                }
            }
            else if (isFull == 0 && hitResult.entity is EntityCow)
            {
                return new ItemStack(Item.MilkBucket);
            }

            return itemStack;
        }
    }
}