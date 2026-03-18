using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockJukeBox : BlockWithEntity
{

    public BlockJukeBox(int id, string textureId = "jukebox") : base(id, textureId, Material.Wood)
    {
        this.textureId = textureId;
    }

    public override string getTexture(string side)
    {
        return side == "up" ? textureId
            : side == "down" ? $"{textureId}_side"
            : $"{textureId}_side";
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        if (world.getBlockMeta(x, y, z) == 0)
        {
            return false;
        }
        else
        {
            tryEjectRecord(world, x, y, z);
            return true;
        }
    }

    public void insertRecord(World world, int x, int y, int z, int id)
    {
        if (!world.isRemote)
        {
            BlockEntityRecordPlayer jukebox = (BlockEntityRecordPlayer)world.getBlockEntity(x, y, z);
            jukebox.recordId = id;
            jukebox.markDirty();
            world.setBlockMeta(x, y, z, 1);
        }
    }

    public void tryEjectRecord(World world, int x, int y, int z)
    {
        if (!world.isRemote)
        {
            BlockEntityRecordPlayer jukebox = (BlockEntityRecordPlayer)world.getBlockEntity(x, y, z);
            int recordId = jukebox.recordId;
            if (recordId != 0)
            {
                world.worldEvent(1005, x, y, z, 0);
                world.playStreaming((String)null, x, y, z);
                jukebox.recordId = 0;
                jukebox.markDirty();
                world.setBlockMeta(x, y, z, 0);
                float spreadFactor = 0.7F;
                double offsetX = (double)(world.random.NextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
                double offsetY = (double)(world.random.NextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.2D + 0.6D;
                double offsetZ = (double)(world.random.NextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
                EntityItem entityItem = new EntityItem(world, (double)x + offsetX, (double)y + offsetY, (double)z + offsetZ, new ItemStack(recordId, 1, 0));
                entityItem.delayBeforeCanPickup = 10;
                world.SpawnEntity(entityItem);
            }
        }
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        tryEjectRecord(world, x, y, z);
        base.onBreak(world, x, y, z);
    }

    public override void dropStacks(World world, int x, int y, int z, int meta, float luck)
    {
        if (!world.isRemote)
        {
            base.dropStacks(world, x, y, z, meta, luck);
        }
    }

    protected override BlockEntity getBlockEntity()
    {
        return new BlockEntityRecordPlayer();
    }
}
