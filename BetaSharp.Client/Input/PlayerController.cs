using BetaSharp.Blocks;
using BetaSharp.Client.Entities;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Input;

public class PlayerController
{
    protected readonly Minecraft mc;
    public bool IsHittingBlock = false;

    public PlayerController(Minecraft mc)
    {
        this.mc = mc;
    }

    public virtual void func_717_a(World World1)
    {
    }

    public virtual void clickBlock(int var1, int var2, int var3, int var4)
    {
        mc.world.extinguishFire(mc.player, var1, var2, var3, var4);
        sendBlockRemoved(var1, var2, var3, var4);
    }

    public virtual bool sendBlockRemoved(int var1, int var2, int var3, int var4)
    {
        World var5 = mc.world;
        Block var6 = Block.Blocks[var5.getBlockId(var1, var2, var3)];
        var5.worldEvent(2001, var1, var2, var3, var6.id + var5.getBlockMeta(var1, var2, var3) * 256);
        int var7 = var5.getBlockMeta(var1, var2, var3);
        bool var8 = var5.setBlock(var1, var2, var3, 0);
        if (var6 != null && var8)
        {
            var6.onMetadataChange(var5, var1, var2, var3, var7);
        }

        return var8;
    }

    public virtual void sendBlockRemoving(int var1, int var2, int var3, int var4)
    {
    }

    public virtual void resetBlockRemoving()
    {
    }

    public virtual void setPartialTime(float var1)
    {
    }

    public virtual float getBlockReachDistance()
    {
        return 5.0F;
    }

    public virtual bool sendUseItem(EntityPlayer Player, World World, ItemStack ItemStack)
    {
        int var4 = ItemStack.count;
        ItemStack var5 = ItemStack.use(World, Player);
        if (var5 != ItemStack || var5 != null && var5.count != var4)
        {
            Player.inventory.main[Player.inventory.selectedSlot] = var5;
            if (var5.count == 0)
            {
                Player.inventory.main[Player.inventory.selectedSlot] = null;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void flipPlayer(EntityPlayer Player)
    {
    }

    public virtual void updateController()
    {
    }

    public virtual bool shouldDrawHUD()
    {
        return true;
    }

    public virtual void onPlayerJoinWorld(EntityPlayer player)
    {
    }

    public virtual bool sendPlaceBlock(EntityPlayer var1, World var2, ItemStack var3, int var4, int var5, int var6, int var7)
    {
        int var8 = var2.getBlockId(var4, var5, var6);
        return var8 > 0 && Block.Blocks[var8].onUse(var2, var4, var5, var6, var1) ? true : (var3 == null ? false : var3.useOnBlock(var1, var2, var4, var5, var6, var7));
    }

    public virtual EntityPlayer createPlayer(World World)
    {
        return new ClientPlayerEntity(mc, World, mc.session, World.Dimension.id);
    }

    public virtual void interactWithEntity(EntityPlayer Player, Entity Entity)
    {
        Player.interact(Entity);
    }

    public virtual void attackEntity(EntityPlayer Player, Entity Entity)
    {
        Player.attack(Entity);
    }

    public virtual ItemStack func_27174_a(int var1, int var2, int var3, bool var4, EntityPlayer Player)
    {
        return Player.currentScreenHandler.onSlotClick(var2, var3, var4, Player);
    }

    public virtual void func_20086_a(int var1, EntityPlayer Player)
    {
        Player.currentScreenHandler.onClosed(Player);
        Player.currentScreenHandler = Player.playerScreenHandler;
    }
}
