using betareborn.Entities;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockNote : BlockContainer
    {
        public BlockNote(int var1) : base(var1, 74, Material.WOOD)
        {
        }

        public override int getTexture(int var1)
        {
            return textureId;
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (var5 > 0 && Block.BLOCKS[var5].canProvidePower())
            {
                bool var6 = var1.isBlockGettingPowered(var2, var3, var4);
                TileEntityNote var7 = (TileEntityNote)var1.getBlockTileEntity(var2, var3, var4);
                if (var7.powered != var6)
                {
                    if (var6)
                    {
                        var7.playNote(var1, var2, var3, var4);
                    }

                    var7.powered = var6;
                }
            }

        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var1.multiplayerWorld)
            {
                return true;
            }
            else
            {
                TileEntityNote var6 = (TileEntityNote)var1.getBlockTileEntity(var2, var3, var4);
                var6.cycleNote();
                var6.playNote(var1, var2, var3, var4);
                return true;
            }
        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (!var1.multiplayerWorld)
            {
                TileEntityNote var6 = (TileEntityNote)var1.getBlockTileEntity(var2, var3, var4);
                var6.playNote(var1, var2, var3, var4);
            }
        }

        protected override TileEntity getBlockEntity()
        {
            return new TileEntityNote();
        }

        public override void playBlock(World var1, int var2, int var3, int var4, int var5, int var6)
        {
            float var7 = (float)java.lang.Math.pow(2.0D, (double)(var6 - 12) / 12.0D);
            string var8 = "harp";
            if (var5 == 1)
            {
                var8 = "bd";
            }

            if (var5 == 2)
            {
                var8 = "snare";
            }

            if (var5 == 3)
            {
                var8 = "hat";
            }

            if (var5 == 4)
            {
                var8 = "bassattack";
            }

            var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "note." + var8, 3.0F, var7);
            var1.addParticle("note", (double)var2 + 0.5D, (double)var3 + 1.2D, (double)var4 + 0.5D, (double)var6 / 24.0D, 0.0D, 0.0D);
        }
    }

}