using betareborn.Entities;
using betareborn.Blocks.BlockEntities;
using betareborn.Inventorys;

namespace betareborn.Containers
{
    public class ContainerDispenser : Container
    {
        private BlockEntityDispenser field_21149_a;

        public ContainerDispenser(IInventory var1, BlockEntityDispenser var2)
        {
            field_21149_a = var2;

            int var3;
            int var4;
            for (var3 = 0; var3 < 3; ++var3)
            {
                for (var4 = 0; var4 < 3; ++var4)
                {
                    addSlot(new Slot(var2, var4 + var3 * 3, 62 + var4 * 18, 17 + var3 * 18));
                }
            }

            for (var3 = 0; var3 < 3; ++var3)
            {
                for (var4 = 0; var4 < 9; ++var4)
                {
                    addSlot(new Slot(var1, var4 + var3 * 9 + 9, 8 + var4 * 18, 84 + var3 * 18));
                }
            }

            for (var3 = 0; var3 < 9; ++var3)
            {
                addSlot(new Slot(var1, var3, 8 + var3 * 18, 142));
            }

        }

        public override bool isUsableByPlayer(EntityPlayer var1)
        {
            return field_21149_a.canPlayerUse(var1);
        }
    }

}