using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn
{
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
            var1.rotationYaw = -180.0F;
        }

        public override bool sendBlockRemoved(int var1, int var2, int var3, int var4)
        {
            int var5 = mc.theWorld.getBlockId(var1, var2, var3);
            int var6 = mc.theWorld.getBlockMeta(var1, var2, var3);
            bool var7 = base.sendBlockRemoved(var1, var2, var3, var4);
            ItemStack var8 = mc.thePlayer.getCurrentEquippedItem();
            bool var9 = mc.thePlayer.canHarvestBlock(Block.BLOCKS[var5]);
            if (var8 != null)
            {
                var8.onDestroyBlock(var5, var1, var2, var3, mc.thePlayer);
                if (var8.count == 0)
                {
                    var8.func_1097_a(mc.thePlayer);
                    mc.thePlayer.destroyCurrentEquippedItem();
                }
            }

            if (var7 && var9)
            {
                Block.BLOCKS[var5].harvestBlock(mc.theWorld, mc.thePlayer, var1, var2, var3, var6);
            }

            return var7;
        }

        public override void clickBlock(int var1, int var2, int var3, int var4)
        {
            mc.theWorld.onBlockHit(mc.thePlayer, var1, var2, var3, var4);
            int var5 = mc.theWorld.getBlockId(var1, var2, var3);
            if (var5 > 0 && curBlockDamage == 0.0F)
            {
                Block.BLOCKS[var5].onBlockClicked(mc.theWorld, var1, var2, var3, mc.thePlayer);
            }

            if (var5 > 0 && Block.BLOCKS[var5].blockStrength(mc.thePlayer) >= 1.0F)
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
                    int var5 = mc.theWorld.getBlockId(var1, var2, var3);
                    if (var5 == 0)
                    {
                        return;
                    }

                    Block var6 = Block.BLOCKS[var5];
                    curBlockDamage += var6.blockStrength(mc.thePlayer);
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
                mc.ingameGUI.damageGuiPartialTime = 0.0F;
                mc.renderGlobal.damagePartialTime = 0.0F;
            }
            else
            {
                float var2 = prevBlockDamage + (curBlockDamage - prevBlockDamage) * var1;
                mc.ingameGUI.damageGuiPartialTime = var2;
                mc.renderGlobal.damagePartialTime = var2;
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

}