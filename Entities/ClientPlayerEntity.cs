using betareborn.NBT;
using betareborn.Stats;
using betareborn.Worlds;
using java.lang;
using betareborn.Inventorys;
using betareborn.Util.Maths;
using betareborn.Client.Guis;
using betareborn.Blocks.Entities;
using betareborn.Client.Input;

namespace betareborn.Entities
{
    public class ClientPlayerEntity : EntityPlayer
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ClientPlayerEntity).TypeHandle);
        public MovementInput movementInput;
        protected Minecraft mc;
        private MouseFilter field_21903_bJ = new MouseFilter();
        private MouseFilter field_21904_bK = new MouseFilter();
        private MouseFilter field_21902_bL = new MouseFilter();

        public ClientPlayerEntity(Minecraft var1, World var2, Session var3, int var4) : base(var2)
        {
            mc = var1;
            dimensionId = var4;
            if (var3 != null && var3.username != null && var3.username.Length > 0)
            {
                skinUrl = "http://s3.amazonaws.com/MinecraftSkins/" + var3.username + ".png";
            }

            name = var3.username;
        }

        public override void move(double var1, double var3, double var5)
        {
            base.move(var1, var3, var5);
        }

        public override void tickLiving()
        {
            base.tickLiving();
            sidewaysSpeed = movementInput.moveStrafe;
            forwardSpeed = movementInput.moveForward;
            jumping = movementInput.jump;
        }

        public override void tickMovement()
        {
            if (!mc.statFileWriter.hasAchievementUnlocked(Achievements.OPEN_INVENTORY))
            {
                mc.guiAchievement.queueAchievementInformation(Achievements.OPEN_INVENTORY);
            }

            lastScreenDistortion = changeDimensionCooldown;
            if (inTeleportationState)
            {
                if (!world.isRemote && vehicle != null)
                {
                    setVehicle((Entity)null);
                }

                if (mc.currentScreen != null)
                {
                    mc.displayGuiScreen((GuiScreen)null);
                }

                if (changeDimensionCooldown == 0.0F)
                {
                    mc.sndManager.playSoundFX("portal.trigger", 1.0F, random.nextFloat() * 0.4F + 0.8F);
                }

                changeDimensionCooldown += 0.0125F;
                if (changeDimensionCooldown >= 1.0F)
                {
                    changeDimensionCooldown = 1.0F;
                    if (!world.isRemote)
                    {
                        portalCooldown = 10;
                        mc.sndManager.playSoundFX("portal.travel", 1.0F, random.nextFloat() * 0.4F + 0.8F);
                        mc.usePortal();
                    }
                }

                inTeleportationState = false;
            }
            else
            {
                if (changeDimensionCooldown > 0.0F)
                {
                    changeDimensionCooldown -= 0.05F;
                }

                if (changeDimensionCooldown < 0.0F)
                {
                    changeDimensionCooldown = 0.0F;
                }
            }

            if (portalCooldown > 0)
            {
                --portalCooldown;
            }

            movementInput.updatePlayerMoveState(this);
            if (movementInput.sneak && cameraOffset < 0.2F)
            {
                cameraOffset = 0.2F;
            }

            pushOutOfBlocks(x - (double)width * 0.35D, boundingBox.minY + 0.5D, z + (double)width * 0.35D);
            pushOutOfBlocks(x - (double)width * 0.35D, boundingBox.minY + 0.5D, z - (double)width * 0.35D);
            pushOutOfBlocks(x + (double)width * 0.35D, boundingBox.minY + 0.5D, z - (double)width * 0.35D);
            pushOutOfBlocks(x + (double)width * 0.35D, boundingBox.minY + 0.5D, z + (double)width * 0.35D);
            base.tickMovement();
        }

        public void resetPlayerKeyState()
        {
            movementInput.resetKeyState();
        }

        public void handleKeyPress(int var1, bool var2)
        {
            movementInput.checkKeyForMovementInput(var1, var2);
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetInteger("Score", score);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
            score = var1.GetInteger("Score");
        }

        public override void closeHandledScreen()
        {
            base.closeHandledScreen();
            mc.displayGuiScreen(null);
        }

        public override void openEditSignScreen(BlockEntitySign var1)
        {
            mc.displayGuiScreen(new GuiEditSign(var1));
        }

        public override void openChestScreen(IInventory var1)
        {
            mc.displayGuiScreen(new GuiChest(inventory, var1));
        }

        public override void openCraftingScreen(int var1, int var2, int var3)
        {
            mc.displayGuiScreen(new GuiCrafting(inventory, world, var1, var2, var3));
        }

        public override void openFurnaceScreen(BlockEntityFurnace var1)
        {
            mc.displayGuiScreen(new GuiFurnace(inventory, var1));
        }

        public override void openDispenserScreen(BlockEntityDispenser var1)
        {
            mc.displayGuiScreen(new GuiDispenser(inventory, var1));
        }

        public override void sendPickup(Entity var1, int var2)
        {
            mc.particleManager.addEffect(new EntityPickupFX(mc.world, var1, this, -0.5F));
        }

        public int getPlayerArmorValue()
        {
            return inventory.getTotalArmorValue();
        }

        public virtual void sendChatMessage(string message)
        {
            mc.ingameGUI.addChatMessage($"<{name}> {message}");
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
                    hearts = maxHealth / 2;
                }
            }
            else
            {
                field_9346_af = var2;
                lastHealth = health;
                hearts = maxHealth;
                applyDamage(var2);
                hurtTime = maxHurtTime = 10;
            }

        }

        public override void respawn()
        {
            mc.respawn(false, 0);
        }

        public override void spawn()
        {
        }

        public override void sendMessage(string var1)
        {
            mc.ingameGUI.addChatMessageTranslate(var1);
        }

        public override void increaseStat(StatBase var1, int var2)
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
            return world.shouldSuffocate(var1, var2, var3);
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
                    velocityX = (double)(-var21);
                }

                if (var18 == 1)
                {
                    velocityX = (double)var21;
                }

                if (var18 == 4)
                {
                    velocityZ = (double)(-var21);
                }

                if (var18 == 5)
                {
                    velocityZ = (double)var21;
                }
            }

            return false;
        }
    }

}