using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemRecord : Item
    {

        public readonly String recordName;

        public ItemRecord(int var1, String var2) : base(var1)
        {
            recordName = var2;
            maxStackSize = 1;
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var3.getBlockId(var4, var5, var6) == Block.JUKEBOX.id && var3.getBlockMeta(var4, var5, var6) == 0)
            {
                if (var3.multiplayerWorld)
                {
                    return true;
                }
                else
                {
                    ((BlockJukeBox)Block.JUKEBOX).ejectRecord(var3, var4, var5, var6, id);
                    var3.func_28107_a((EntityPlayer)null, 1005, var4, var5, var6, id);
                    --var1.count;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

}