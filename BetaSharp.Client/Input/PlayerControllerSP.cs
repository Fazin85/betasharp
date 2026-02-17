using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Input;

public class PlayerControllerSP : PlayerController
{
    private int field_1074_c = -1;
    private int field_1073_d = -1;
    private int field_1072_e = -1;
    private float curBlockDamage = 0.0F;
    private float prevBlockDamage = 0.0F;
    private float field_1069_h = 0.0F;
    private int blockHitWait = 0;

    public PlayerControllerSP(Minecraft var1) : base(var1)
    {
    }

    public override void flipPlayer(EntityPlayer var1)
    {
        var1.yaw = -180.0F;
    }

    public override bool sendBlockRemoved(int x, int y, int z, int var4)
    {
        int blockId = mc.world.getBlockId(x, y, z);
        int var6 = mc.world.getBlockMeta(x, y, z);
        bool var7 = base.sendBlockRemoved(x, y, z, var4);
        ItemStack var8 = mc.player.getHand();
        bool var9 = mc.player.canHarvest(Block.Blocks[blockId]);
        if (var8 != null)
        {
            var8.postMine(blockId, x, y, z, mc.player);
            if (var8.count == 0)
            {
                var8.onRemoved(mc.player);
                mc.player.clearStackInHand();
            }
        }

        if (var7 && var9)
        {
            Block.Blocks[blockId].afterBreak(mc.world, mc.player, x, y, z, var6);
        }

        return var7;
    }

    public override void clickBlock(int var1, int var2, int var3, int var4)
    {
        mc.world.extinguishFire(mc.player, var1, var2, var3, var4);
        int var5 = mc.world.getBlockId(var1, var2, var3);
        if (var5 > 0 && curBlockDamage == 0.0F)
        {
            Block.Blocks[var5].onBlockBreakStart(mc.world, var1, var2, var3, mc.player);
        }

        if (var5 > 0 && Block.Blocks[var5].getHardness(mc.player) >= 1.0F)
        {
            sendBlockRemoved(var1, var2, var3, var4);
        }

    }

    public override void resetBlockRemoving()
    {
        curBlockDamage = 0.0F;
        blockHitWait = 0;
    }

    public override void sendBlockRemoving(int var1, int var2, int var3, int var4)
    {
        if (blockHitWait > 0)
        {
            --blockHitWait;
        }
        else
        {
            if (var1 == field_1074_c && var2 == field_1073_d && var3 == field_1072_e)
            {
                int var5 = mc.world.getBlockId(var1, var2, var3);
                if (var5 == 0)
                {
                    return;
                }

                Block var6 = Block.Blocks[var5];
                curBlockDamage += var6.getHardness(mc.player);
                if (field_1069_h % 4.0F == 0.0F && var6 != null)
                {
                    mc.sndManager.playSound(var6.soundGroup.func_1145_d(), (float)var1 + 0.5F, (float)var2 + 0.5F, (float)var3 + 0.5F, (var6.soundGroup.getVolume() + 1.0F) / 8.0F, var6.soundGroup.getPitch() * 0.5F);
                }

                ++field_1069_h;
                if (curBlockDamage >= 1.0F)
                {
                    sendBlockRemoved(var1, var2, var3, var4);
                    curBlockDamage = 0.0F;
                    prevBlockDamage = 0.0F;
                    field_1069_h = 0.0F;
                    blockHitWait = 5;
                }
            }
            else
            {
                curBlockDamage = 0.0F;
                prevBlockDamage = 0.0F;
                field_1069_h = 0.0F;
                field_1074_c = var1;
                field_1073_d = var2;
                field_1072_e = var3;
            }

        }
    }

    public override void setPartialTime(float var1)
    {
        if (curBlockDamage <= 0.0F)
        {
            mc.ingameGUI._damageGuiPartialTime = 0.0F;
            mc.terrainRenderer.damagePartialTime = 0.0F;
        }
        else
        {
            float var2 = prevBlockDamage + (curBlockDamage - prevBlockDamage) * var1;
            mc.ingameGUI._damageGuiPartialTime = var2;
            mc.terrainRenderer.damagePartialTime = var2;
        }

    }

    public override float getBlockReachDistance()
    {
        return 4.0F;
    }

    public override void func_717_a(World var1)
    {
        base.func_717_a(var1);
    }

    public override void updateController()
    {
        prevBlockDamage = curBlockDamage;
        mc.sndManager.playRandomMusicIfReady();
    }
}
