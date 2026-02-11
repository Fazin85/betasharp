using betareborn.NBT;

namespace betareborn.Blocks.Entities
{
    public class BlockEntityRecordPlayer : BlockEntity
    {
        public int recordId;

        public override void readNbt(NbtTagCompound nbt)
        {
            base.readNbt(nbt);
            recordId = nbt.GetInteger("Record");
        }

        public override void writeNbt(NbtTagCompound nbt)
        {
            base.writeNbt(nbt);
            if (recordId > 0)
            {
                nbt.SetInteger("Record", recordId);
            }

        }
    }

}