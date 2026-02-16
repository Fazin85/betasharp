using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemDye : Item
{

    public static readonly String[] dyeColors = new String[] { "black", "red", "green", "brown", "blue", "purple", "cyan", "silver", "gray", "pink", "lime", "yellow", "lightBlue", "magenta", "orange", "white" };
    public static readonly int[] field_31002_bk = new int[] { 1973019, 11743532, 3887386, 5320730, 2437522, 8073150, 2651799, 2651799, 4408131, 14188952, 4312372, 14602026, 6719955, 12801229, 15435844, 15790320 };

    public ItemDye(int id) : base(id)
    {
        setHasSubtypes(true);
        setMaxDamage(0);
    }

    public override int getTextureId(int meta)
    {
        return textureId + meta % 8 * 16 + meta / 8;
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        return base.getItemName() + "." + dyeColors[itemStack.getDamage()];
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        if (itemStack.getDamage() == 15)
        {
            int blockId = world.getBlockId(x, y, z);
            if (blockId == Block.Sapling.id)
            {
                if (!world.isRemote)
                {
                    ((BlockSapling)Block.Sapling).generate(world, x, y, z, world.random);
                    --itemStack.count;
                }
                return true;
            }
            if (blockId == Block.Wheat.id)
            {
                if (!world.isRemote)
                {
                    ((BlockCrops)Block.Wheat).applyFullGrowth(world, x, y, z);
                    --itemStack.count;
                }
                return true;
            }
            if (blockId == Block.GrassBlock.id)
            {
                if (!world.isRemote)
                {
                    --itemStack.count;

                    for (int attempt = 0; attempt < 128; ++attempt)
                    {
                        int spawnX = x;
                        int spawnY = y + 1;
                        int spawnZ = z;

                        bool validPosition = true;
                        for (int walkStep = 0; walkStep < attempt / 16 && validPosition; ++walkStep)
                        {
                            spawnX += itemRand.nextInt(3) - 1;
                            spawnY += (itemRand.nextInt(3) - 1) * itemRand.nextInt(3) / 2;
                            spawnZ += itemRand.nextInt(3) - 1;
                            if (world.getBlockId(spawnX, spawnY - 1, spawnZ) != Block.GrassBlock.id || world.shouldSuffocate(spawnX, spawnY, spawnZ))
                            {
                                validPosition = false;
                            }
                        }

                        if (validPosition && world.getBlockId(spawnX, spawnY, spawnZ) == 0)
                        {
                            if (itemRand.nextInt(10) != 0)
                            {
                                world.setBlock(spawnX, spawnY, spawnZ, Block.Grass.id, 1);
                            }
                            else if (itemRand.nextInt(3) != 0)
                            {
                                world.setBlock(spawnX, spawnY, spawnZ, Block.Dandelion.id);
                            }
                            else
                            {
                                world.setBlock(spawnX, spawnY, spawnZ, Block.Rose.id);
                            }
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    public override void useOnEntity(ItemStack itemStack, EntityLiving entityLiving)
    {
        if (entityLiving is EntitySheep)
        {
            EntitySheep sheep = (EntitySheep)entityLiving;
            int woolColor = BlockCloth.getBlockMeta(itemStack.getDamage());
            if (!sheep.getSheared() && sheep.getFleeceColor() != woolColor)
            {
                sheep.setFleeceColor(woolColor);
                --itemStack.count;
            }
        }

    }
}