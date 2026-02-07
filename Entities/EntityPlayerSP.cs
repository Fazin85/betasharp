using betareborn.Guis;
using betareborn.NBT;
using betareborn.Stats;
using betareborn.TileEntities;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityPlayerSP : EntityPlayer
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPlayerSP).TypeHandle);
        public MovementInput movementInput;
        protected Minecraft mc;
        private MouseFilter field_21903_bJ = new MouseFilter();
        private MouseFilter field_21904_bK = new MouseFilter();
        private MouseFilter field_21902_bL = new MouseFilter();

        public EntityPlayerSP(Minecraft var1, World var2, Session var3, int var4) : base(var2)
        {
            mc = var1;
            dimension = var4;
            if (var3 != null && var3.username != null && var3.username.Length > 0)
            {
                skinUrl = "http://s3.amazonaws.com/MinecraftSkins/" + var3.username + ".png";
            }

            username = var3.username;
        }

        public override void moveEntity(double var1, double var3, double var5)
        {
            base.moveEntity(var1, var3, var5);
        }

        public override void updatePlayerActionState()
        {
            base.updatePlayerActionState();
            moveStrafing = movementInput.moveStrafe;
            moveForward = movementInput.moveForward;
            isJumping = movementInput.jump;
        }

        public override void onLivingUpdate()
        {
            if (!mc.statFileWriter.hasAchievementUnlocked(Achievements.OPEN_INVENTORY))
            {
                mc.guiAchievement.queueAchievementInformation(Achievements.OPEN_INVENTORY);
            }

            prevTimeInPortal = timeInPortal;
            if (inPortal)
            {
                if (!worldObj.multiplayerWorld && ridingEntity != null)
                {
                    mountEntity((Entity)null);
                }

                if (mc.currentScreen != null)
                {
                    mc.displayGuiScreen((GuiScreen)null);
                }

                if (timeInPortal == 0.0F)
                {
                    mc.sndManager.playSoundFX("portal.trigger", 1.0F, rand.nextFloat() * 0.4F + 0.8F);
                }

                timeInPortal += 0.0125F;
                if (timeInPortal >= 1.0F)
                {
                    timeInPortal = 1.0F;
                    if (!worldObj.multiplayerWorld)
                    {
                        timeUntilPortal = 10;
                        mc.sndManager.playSoundFX("portal.travel", 1.0F, rand.nextFloat() * 0.4F + 0.8F);
                        mc.usePortal();
                    }
                }

                inPortal = false;
            }
            else
            {
                if (timeInPortal > 0.0F)
                {
                    timeInPortal -= 0.05F;
                }

                if (timeInPortal < 0.0F)
                {
                    timeInPortal = 0.0F;
                }
            }

            if (timeUntilPortal > 0)
            {
                --timeUntilPortal;
            }

            movementInput.updatePlayerMoveState(this);
            if (movementInput.sneak && ySize < 0.2F)
            {
                ySize = 0.2F;
            }

            pushOutOfBlocks(posX - (double)width * 0.35D, boundingBox.minY + 0.5D, posZ + (double)width * 0.35D);
            pushOutOfBlocks(posX - (double)width * 0.35D, boundingBox.minY + 0.5D, posZ - (double)width * 0.35D);
            pushOutOfBlocks(posX + (double)width * 0.35D, boundingBox.minY + 0.5D, posZ - (double)width * 0.35D);
            pushOutOfBlocks(posX + (double)width * 0.35D, boundingBox.minY + 0.5D, posZ + (double)width * 0.35D);
            base.onLivingUpdate();
        }

        public void resetPlayerKeyState()
        {
            movementInput.resetKeyState();
        }

        public void handleKeyPress(int var1, bool var2)
        {
            movementInput.checkKeyForMovementInput(var1, var2);
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            base.writeEntityToNBT(var1);
            var1.setInteger("Score", score);
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            base.readEntityFromNBT(var1);
            score = var1.getInteger("Score");
        }

        public override void closeScreen()
        {
            base.closeScreen();
            mc.displayGuiScreen(null);
        }

        public override void displayGUIEditSign(TileEntitySign var1)
        {
            mc.displayGuiScreen(new GuiEditSign(var1));
        }

        public override void displayGUIChest(IInventory var1)
        {
            mc.displayGuiScreen(new GuiChest(inventory, var1));
        }

        public override void displayWorkbenchGUI(int var1, int var2, int var3)
        {
            mc.displayGuiScreen(new GuiCrafting(inventory, worldObj, var1, var2, var3));
        }

        public override void displayGUIFurnace(TileEntityFurnace var1)
        {
            mc.displayGuiScreen(new GuiFurnace(inventory, var1));
        }

        public override void displayGUIDispenser(TileEntityDispenser var1)
        {
            mc.displayGuiScreen(new GuiDispenser(inventory, var1));
        }

        public override void onItemPickup(Entity var1, int var2)
        {
            mc.effectRenderer.addEffect(new EntityPickupFX(mc.theWorld, var1, this, -0.5F));
        }

        public int getPlayerArmorValue()
        {
            return inventory.getTotalArmorValue();
        }

        public virtual void sendChatMessage(string var1)
        {
        }

        public override bool isSneaking()
        {
            return movementInput.sneak && !sleeping;
        }

        public virtual void setHealth(int var1)
        {
            int var2 = health - var1;
            if (var2 <= 0)
            {
                health = var1;
                if (var2 < 0)
                {
                    heartsLife = heartsHalvesLife / 2;
                }
            }
            else
            {
                field_9346_af = var2;
                prevHealth = health;
                heartsLife = heartsHalvesLife;
                damageEntity(var2);
                hurtTime = maxHurtTime = 10;
            }

        }

        public override void respawnPlayer()
        {
            mc.respawn(false, 0);
        }

        public override void func_6420_o()
        {
        }

        public override void addChatMessage(string var1)
        {
            mc.ingameGUI.addChatMessageTranslate(var1);
        }

        public override void addStat(StatBase var1, int var2)
        {
            if (var1 != null)
            {
                if (var1.isAchievement())
                {
                    Achievement var3 = (Achievement)var1;
                    if (var3.parent == null || mc.statFileWriter.hasAchievementUnlocked(var3.parent))
                    {
                        if (!mc.statFileWriter.hasAchievementUnlocked(var3))
                        {
                            mc.guiAchievement.queueTakenAchievement(var3);
                        }

                        mc.statFileWriter.readStat(var1, var2);
                    }
                }
                else
                {
                    mc.statFileWriter.readStat(var1, var2);
                }

            }
        }

        private bool isBlockTranslucent(int var1, int var2, int var3)
        {
            return worldObj.isBlockNormalCube(var1, var2, var3);
        }

        protected override bool pushOutOfBlocks(double var1, double var3, double var5)
        {
            int var7 = MathHelper.floor_double(var1);
            int var8 = MathHelper.floor_double(var3);
            int var9 = MathHelper.floor_double(var5);
            double var10 = var1 - (double)var7;
            double var12 = var5 - (double)var9;
            if (isBlockTranslucent(var7, var8, var9) || isBlockTranslucent(var7, var8 + 1, var9))
            {
                bool var14 = !isBlockTranslucent(var7 - 1, var8, var9) && !isBlockTranslucent(var7 - 1, var8 + 1, var9);
                bool var15 = !isBlockTranslucent(var7 + 1, var8, var9) && !isBlockTranslucent(var7 + 1, var8 + 1, var9);
                bool var16 = !isBlockTranslucent(var7, var8, var9 - 1) && !isBlockTranslucent(var7, var8 + 1, var9 - 1);
                bool var17 = !isBlockTranslucent(var7, var8, var9 + 1) && !isBlockTranslucent(var7, var8 + 1, var9 + 1);
                int var18 = -1;
                double var19 = 9999.0D;
                if (var14 && var10 < var19)
                {
                    var19 = var10;
                    var18 = 0;
                }

                if (var15 && 1.0D - var10 < var19)
                {
                    var19 = 1.0D - var10;
                    var18 = 1;
                }

                if (var16 && var12 < var19)
                {
                    var19 = var12;
                    var18 = 4;
                }

                if (var17 && 1.0D - var12 < var19)
                {
                    var19 = 1.0D - var12;
                    var18 = 5;
                }

                float var21 = 0.1F;
                if (var18 == 0)
                {
                    motionX = (double)(-var21);
                }

                if (var18 == 1)
                {
                    motionX = (double)var21;
                }

                if (var18 == 4)
                {
                    motionZ = (double)(-var21);
                }

                if (var18 == 5)
                {
                    motionZ = (double)var21;
                }
            }

            return false;
        }
    }

}