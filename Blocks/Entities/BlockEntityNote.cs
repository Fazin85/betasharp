using betareborn.Blocks.Materials;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Blocks.Entities
{
    public class BlockEntityNote : BlockEntity
    {
        public sbyte note = 0;
        public bool powered = false;

        public override void writeNbt(NBTTagCompound nbt)
        {
            base.writeNbt(nbt);
            nbt.setByte("note", note);
        }

        public override void readNbt(NBTTagCompound nbt)
        {
            base.readNbt(nbt);
            note = nbt.getByte("note");
            if (note < 0)
            {
                note = 0;
            }

            if (note > 24)
            {
                note = 24;
            }

        }

        public void cycleNote()
        {
            note = (sbyte)((note + 1) % 25);
            markDirty();
        }

        public void playNote(World world, int x, int y, int z)
        {
            if (world.getMaterial(x, y + 1, z) == Material.AIR)
            {
                Material material = world.getMaterial(x, y - 1, z);
                byte instrument = 0;
                if (material == Material.STONE)
                {
                    instrument = 1;
                }

                if (material == Material.SAND)
                {
                    instrument = 2;
                }

                if (material == Material.GLASS)
                {
                    instrument = 3;
                }

                if (material == Material.WOOD)
                {
                    instrument = 4;
                }

                world.playNoteBlockActionAt(x, y, z, instrument, note);
            }
        }
    }

}