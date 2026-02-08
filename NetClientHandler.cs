using betareborn.Blocks;
using betareborn.Chunks;
using betareborn.Containers;
using betareborn.Entities;
using betareborn.Guis;
using betareborn.Items;
using betareborn.Packets;
using betareborn.Worlds;
using betareborn.Worlds.Chunks;
using betareborn.Worlds.Storage;
using java.io;
using java.net;
using betareborn.Blocks.BlockEntities;
using betareborn.Inventorys;

namespace betareborn
{
    public class NetClientHandler : NetHandler
    {
        private bool disconnected = false;
        private NetworkManager netManager;
        public String field_1209_a;
        private Minecraft mc;
        private ClientWorld worldClient;
        private bool field_1210_g = false;
        public PersistentStateManager clientPersistentStateManager = new PersistentStateManager((WorldStorage)null);
        java.util.Random rand = new();

        public NetClientHandler(Minecraft var1, String var2, int var3)
        {

            mc = var1;
            Socket socket = new Socket(InetAddress.getByName(var2), var3);
            socket.setTcpNoDelay(true);
            netManager = new NetworkManager(socket, "Client", this);
        }

        public void tick()
        {
            if (!disconnected)
            {
                netManager.processReadPackets();
            }

            netManager.wakeThreads();
        }

        public override void handleLogin(Packet1Login var1)
        {
            mc.playerController = new PlayerControllerMP(mc, this);
            mc.statFileWriter.readStat(Stats.Stats.joinMultiplayerStat, 1);
            worldClient = new ClientWorld(this, var1.mapSeed, var1.dimension);
            worldClient.isRemote = true;
            mc.changeWorld1(worldClient);
            mc.thePlayer.dimension = var1.dimension;
            mc.displayGuiScreen(new GuiDownloadTerrain(this));
            mc.thePlayer.entityId = var1.protocolVersion;
        }

        public override void handlePickupSpawn(Packet21PickupSpawn var1)
        {
            double var2 = (double)var1.xPosition / 32.0D;
            double var4 = (double)var1.yPosition / 32.0D;
            double var6 = (double)var1.zPosition / 32.0D;
            EntityItem var8 = new EntityItem(worldClient, var2, var4, var6, new ItemStack(var1.itemID, var1.count, var1.itemDamage));
            var8.motionX = (double)var1.rotation / 128.0D;
            var8.motionY = (double)var1.pitch / 128.0D;
            var8.motionZ = (double)var1.roll / 128.0D;
            var8.serverPosX = var1.xPosition;
            var8.serverPosY = var1.yPosition;
            var8.serverPosZ = var1.zPosition;
            worldClient.forceEntity(var1.entityId, var8);
        }

        public override void handleVehicleSpawn(Packet23VehicleSpawn var1)
        {
            double var2 = (double)var1.xPosition / 32.0D;
            double var4 = (double)var1.yPosition / 32.0D;
            double var6 = (double)var1.zPosition / 32.0D;
            Object var8 = null;
            if (var1.type == 10)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 0);
            }

            if (var1.type == 11)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 1);
            }

            if (var1.type == 12)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 2);
            }

            if (var1.type == 90)
            {
                var8 = new EntityFish(worldClient, var2, var4, var6);
            }

            if (var1.type == 60)
            {
                var8 = new EntityArrow(worldClient, var2, var4, var6);
            }

            if (var1.type == 61)
            {
                var8 = new EntitySnowball(worldClient, var2, var4, var6);
            }

            if (var1.type == 63)
            {
                var8 = new EntityFireball(worldClient, var2, var4, var6, (double)var1.field_28047_e / 8000.0D, (double)var1.field_28046_f / 8000.0D, (double)var1.field_28045_g / 8000.0D);
                var1.field_28044_i = 0;
            }

            if (var1.type == 62)
            {
                var8 = new EntityEgg(worldClient, var2, var4, var6);
            }

            if (var1.type == 1)
            {
                var8 = new EntityBoat(worldClient, var2, var4, var6);
            }

            if (var1.type == 50)
            {
                var8 = new EntityTNTPrimed(worldClient, var2, var4, var6);
            }

            if (var1.type == 70)
            {
                var8 = new EntityFallingSand(worldClient, var2, var4, var6, Block.SAND.id);
            }

            if (var1.type == 71)
            {
                var8 = new EntityFallingSand(worldClient, var2, var4, var6, Block.GRAVEL.id);
            }

            if (var8 != null)
            {
                ((Entity)var8).serverPosX = var1.xPosition;
                ((Entity)var8).serverPosY = var1.yPosition;
                ((Entity)var8).serverPosZ = var1.zPosition;
                ((Entity)var8).rotationYaw = 0.0F;
                ((Entity)var8).rotationPitch = 0.0F;
                ((Entity)var8).entityId = var1.entityId;
                worldClient.forceEntity(var1.entityId, (Entity)var8);
                if (var1.field_28044_i > 0)
                {
                    if (var1.type == 60)
                    {
                        Entity var9 = getEntityByID(var1.field_28044_i);
                        if (var9 is EntityLiving)
                        {
                            ((EntityArrow)var8).owner = (EntityLiving)var9;
                        }
                    }

                    ((Entity)var8).setVelocity((double)var1.field_28047_e / 8000.0D, (double)var1.field_28046_f / 8000.0D, (double)var1.field_28045_g / 8000.0D);
                }
            }

        }

        public override void handleWeather(Packet71Weather var1)
        {
            double var2 = (double)var1.field_27053_b / 32.0D;
            double var4 = (double)var1.field_27057_c / 32.0D;
            double var6 = (double)var1.field_27056_d / 32.0D;
            EntityLightningBolt var8 = null;
            if (var1.field_27055_e == 1)
            {
                var8 = new EntityLightningBolt(worldClient, var2, var4, var6);
            }

            if (var8 != null)
            {
                var8.serverPosX = var1.field_27053_b;
                var8.serverPosY = var1.field_27057_c;
                var8.serverPosZ = var1.field_27056_d;
                var8.rotationYaw = 0.0F;
                var8.rotationPitch = 0.0F;
                var8.entityId = var1.field_27054_a;
                worldClient.addWeatherEffect(var8);
            }

        }

        public override void func_21146_a(Packet25EntityPainting var1)
        {
            EntityPainting var2 = new EntityPainting(worldClient, var1.xPosition, var1.yPosition, var1.zPosition, var1.direction, var1.title);
            worldClient.forceEntity(var1.entityId, var2);
        }

        public override void func_6498_a(Packet28EntityVelocity var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.setVelocity((double)var1.motionX / 8000.0D, (double)var1.motionY / 8000.0D, (double)var1.motionZ / 8000.0D);
            }
        }

        public override void func_21148_a(Packet40EntityMetadata var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null && var1.func_21047_b() != null)
            {
                var2.getDataWatcher().updateWatchedObjectsFromList(var1.func_21047_b());
            }

        }

        public override void handleNamedEntitySpawn(Packet20NamedEntitySpawn var1)
        {
            double var2 = (double)var1.xPosition / 32.0D;
            double var4 = (double)var1.yPosition / 32.0D;
            double var6 = (double)var1.zPosition / 32.0D;
            float var8 = (float)(var1.rotation * 360) / 256.0F;
            float var9 = (float)(var1.pitch * 360) / 256.0F;
            EntityOtherPlayerMP var10 = new EntityOtherPlayerMP(mc.theWorld, var1.name);
            var10.prevPosX = var10.lastTickPosX = (double)(var10.serverPosX = var1.xPosition);
            var10.prevPosY = var10.lastTickPosY = (double)(var10.serverPosY = var1.yPosition);
            var10.prevPosZ = var10.lastTickPosZ = (double)(var10.serverPosZ = var1.zPosition);
            int var11 = var1.currentItem;
            if (var11 == 0)
            {
                var10.inventory.mainInventory[var10.inventory.currentItem] = null;
            }
            else
            {
                var10.inventory.mainInventory[var10.inventory.currentItem] = new ItemStack(var11, 1, 0);
            }

            var10.setPositionAndRotation(var2, var4, var6, var8, var9);
            worldClient.forceEntity(var1.entityId, var10);
        }

        public override void handleEntityTeleport(Packet34EntityTeleport var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.serverPosX = var1.xPosition;
                var2.serverPosY = var1.yPosition;
                var2.serverPosZ = var1.zPosition;
                double var3 = (double)var2.serverPosX / 32.0D;
                double var5 = (double)var2.serverPosY / 32.0D + 1.0D / 64.0D;
                double var7 = (double)var2.serverPosZ / 32.0D;
                float var9 = (float)(var1.yaw * 360) / 256.0F;
                float var10 = (float)(var1.pitch * 360) / 256.0F;
                var2.setPositionAndRotation2(var3, var5, var7, var9, var10, 3);
            }
        }

        public override void handleEntity(Packet30Entity var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.serverPosX += var1.xPosition;
                var2.serverPosY += var1.yPosition;
                var2.serverPosZ += var1.zPosition;
                double var3 = (double)var2.serverPosX / 32.0D;
                double var5 = (double)var2.serverPosY / 32.0D;
                double var7 = (double)var2.serverPosZ / 32.0D;
                float var9 = var1.rotating ? (float)(var1.yaw * 360) / 256.0F : var2.rotationYaw;
                float var10 = var1.rotating ? (float)(var1.pitch * 360) / 256.0F : var2.rotationPitch;
                var2.setPositionAndRotation2(var3, var5, var7, var9, var10, 3);
            }
        }

        public override void handleDestroyEntity(Packet29DestroyEntity var1)
        {
            worldClient.removeEntityFromWorld(var1.entityId);
        }

        public override void handleFlying(Packet10Flying var1)
        {
            EntityPlayerSP var2 = mc.thePlayer;
            double var3 = var2.posX;
            double var5 = var2.posY;
            double var7 = var2.posZ;
            float var9 = var2.rotationYaw;
            float var10 = var2.rotationPitch;
            if (var1.moving)
            {
                var3 = var1.xPosition;
                var5 = var1.yPosition;
                var7 = var1.zPosition;
            }

            if (var1.rotating)
            {
                var9 = var1.yaw;
                var10 = var1.pitch;
            }

            var2.ySize = 0.0F;
            var2.motionX = var2.motionY = var2.motionZ = 0.0D;
            var2.setPositionAndRotation(var3, var5, var7, var9, var10);
            var1.xPosition = var2.posX;
            var1.yPosition = var2.boundingBox.minY;
            var1.zPosition = var2.posZ;
            var1.stance = var2.posY;
            netManager.addToSendQueue(var1);
            if (!field_1210_g)
            {
                mc.thePlayer.prevPosX = mc.thePlayer.posX;
                mc.thePlayer.prevPosY = mc.thePlayer.posY;
                mc.thePlayer.prevPosZ = mc.thePlayer.posZ;
                field_1210_g = true;
                mc.displayGuiScreen((GuiScreen)null);
            }

        }

        public override void handlePreChunk(Packet50PreChunk var1)
        {
            worldClient.updateChunk(var1.xPosition, var1.yPosition, var1.mode);
        }

        public override void handleMultiBlockChange(Packet52MultiBlockChange var1)
        {
            Chunk var2 = worldClient.getChunk(var1.xPosition, var1.zPosition);
            int var3 = var1.xPosition * 16;
            int var4 = var1.zPosition * 16;

            for (int var5 = 0; var5 < var1._size; ++var5)
            {
                short var6 = var1.coordinateArray[var5];
                int var7 = var1.typeArray[var5] & 255;
                byte var8 = var1.metadataArray[var5];
                int var9 = var6 >> 12 & 15;
                int var10 = var6 >> 8 & 15;
                int var11 = var6 & 255;
                var2.setBlock(var9, var11, var10, var7, var8);
                worldClient.clearBlockResets(var9 + var3, var11, var10 + var4, var9 + var3, var11, var10 + var4);
                worldClient.setBlocksDirty(var9 + var3, var11, var10 + var4, var9 + var3, var11, var10 + var4);
            }

        }

        public override void handleMapChunk(Packet51MapChunk var1)
        {
            worldClient.clearBlockResets(var1.xPosition, var1.yPosition, var1.zPosition, var1.xPosition + var1.xSize - 1, var1.yPosition + var1.ySize - 1, var1.zPosition + var1.zSize - 1);
            worldClient.setChunkData(var1.xPosition, var1.yPosition, var1.zPosition, var1.xSize, var1.ySize, var1.zSize, var1.chunk);
        }

        public override void handleBlockChange(Packet53BlockChange var1)
        {
            worldClient.setBlockWithMetaFromPacket(var1.xPosition, var1.yPosition, var1.zPosition, var1.type, var1.metadata);
        }

        public override void handleKickDisconnect(Packet255KickDisconnect var1)
        {
            netManager.networkShutdown("disconnect.kicked", new Object[0]);
            disconnected = true;
            mc.changeWorld1((World)null);
            mc.displayGuiScreen(new GuiConnectFailed("disconnect.disconnected", "disconnect.genericReason", new Object[] { var1.reason }));
        }

        public override void handleErrorMessage(String var1, Object[] var2)
        {
            if (!disconnected)
            {
                disconnected = true;
                mc.changeWorld1((World)null);
                mc.displayGuiScreen(new GuiConnectFailed("disconnect.lost", var1, var2));
            }
        }

        public void sendPacketAndDisconnect(Packet var1)
        {
            if (!disconnected)
            {
                netManager.addToSendQueue(var1);
                netManager.func_28142_c();
            }
        }

        public void addToSendQueue(Packet var1)
        {
            if (!disconnected)
            {
                netManager.addToSendQueue(var1);
            }
        }

        public override void handleCollect(Packet22Collect var1)
        {
            Entity var2 = getEntityByID(var1.collectedEntityId);
            Object var3 = (EntityLiving)getEntityByID(var1.collectorEntityId);
            if (var3 == null)
            {
                var3 = mc.thePlayer;
            }

            if (var2 != null)
            {
                worldClient.playSoundAtEntity(var2, "random.pop", 0.2F, ((rand.nextFloat() - rand.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                mc.effectRenderer.addEffect(new EntityPickupFX(mc.theWorld, var2, (Entity)var3, -0.5F));
                worldClient.removeEntityFromWorld(var1.collectedEntityId);
            }

        }

        public override void handleChat(Packet3Chat var1)
        {
            mc.ingameGUI.addChatMessage(var1.message);
        }

        public override void handleArmAnimation(Packet18Animation var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                EntityPlayer var3;
                if (var1.animate == 1)
                {
                    var3 = (EntityPlayer)var2;
                    var3.swingItem();
                }
                else if (var1.animate == 2)
                {
                    var2.performHurtAnimation();
                }
                else if (var1.animate == 3)
                {
                    var3 = (EntityPlayer)var2;
                    var3.wakeUpPlayer(false, false, false);
                }
                else if (var1.animate == 4)
                {
                    var3 = (EntityPlayer)var2;
                    var3.func_6420_o();
                }

            }
        }

        public override void func_22186_a(Packet17Sleep var1)
        {
            Entity var2 = getEntityByID(var1.field_22045_a);
            if (var2 != null)
            {
                if (var1.field_22046_e == 0)
                {
                    EntityPlayer var3 = (EntityPlayer)var2;
                    var3.sleepInBedAt(var1.field_22044_b, var1.field_22048_c, var1.field_22047_d);
                }

            }
        }

        public override void handleHandshake(Packet2Handshake var1)
        {
            if (var1.username.Equals("-"))
            {
                addToSendQueue(new Packet1Login(mc.session.username, 14));
            }
            else
            {
                try
                {
                    URL var2 = new URL("http://www.minecraft.net/game/joinserver.jsp?user=" + mc.session.username + "&sessionId=" + mc.session.sessionId + "&serverId=" + var1.username);
                    BufferedReader var3 = new BufferedReader(new InputStreamReader(var2.openStream()));
                    String var4 = var3.readLine();
                    var3.close();
                    //TODO: AUTH
                    if (var4 == null || var4.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        addToSendQueue(new Packet1Login(mc.session.username, 14));
                    }
                    else
                    {
                        netManager.networkShutdown("disconnect.loginFailedInfo", new Object[] { var4 });
                    }
                }
                catch (java.lang.Exception var5)
                {
                    var5.printStackTrace();
                    netManager.networkShutdown("disconnect.genericReason", new Object[] { "Internal client error: " + var5.toString() });
                }
            }

        }

        public void disconnect()
        {
            disconnected = true;
            netManager.wakeThreads();
            netManager.networkShutdown("disconnect.closed", new Object[0]);
        }

        public override void handleMobSpawn(Packet24MobSpawn var1)
        {
            double var2 = (double)var1.xPosition / 32.0D;
            double var4 = (double)var1.yPosition / 32.0D;
            double var6 = (double)var1.zPosition / 32.0D;
            float var8 = (float)(var1.yaw * 360) / 256.0F;
            float var9 = (float)(var1.pitch * 360) / 256.0F;
            EntityLiving var10 = (EntityLiving)EntityRegistry.create(var1.type, mc.theWorld);
            var10.serverPosX = var1.xPosition;
            var10.serverPosY = var1.yPosition;
            var10.serverPosZ = var1.zPosition;
            var10.entityId = var1.entityId;
            var10.setPositionAndRotation(var2, var4, var6, var8, var9);
            var10.isMultiplayerEntity = true;
            worldClient.forceEntity(var1.entityId, var10);
            java.util.List var11 = var1.getMetadata();
            if (var11 != null)
            {
                var10.getDataWatcher().updateWatchedObjectsFromList(var11);
            }

        }

        public override void handleUpdateTime(Packet4UpdateTime var1)
        {
            mc.theWorld.setWorldTime(var1.time);
        }

        public override void handleSpawnPosition(Packet6SpawnPosition var1)
        {
            mc.thePlayer.setPlayerSpawnCoordinate(new Vec3i(var1.xPosition, var1.yPosition, var1.zPosition));
            mc.theWorld.getWorldInfo().setSpawn(var1.xPosition, var1.yPosition, var1.zPosition);
        }

        public override void func_6497_a(Packet39AttachEntity var1)
        {
            Object var2 = getEntityByID(var1.entityId);
            Entity var3 = getEntityByID(var1.vehicleEntityId);
            if (var1.entityId == mc.thePlayer.entityId)
            {
                var2 = mc.thePlayer;
            }

            if (var2 != null)
            {
                ((Entity)var2).mountEntity(var3);
            }
        }

        public override void func_9447_a(Packet38EntityStatus var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.handleHealthUpdate(var1.entityStatus);
            }

        }

        private Entity getEntityByID(int var1)
        {
            return (Entity)(var1 == mc.thePlayer.entityId ? mc.thePlayer : worldClient.getEntity(var1));
        }

        public override void handleHealth(Packet8UpdateHealth var1)
        {
            mc.thePlayer.setHealth(var1.healthMP);
        }

        public override void func_9448_a(Packet9Respawn var1)
        {
            if (var1.field_28048_a != mc.thePlayer.dimension)
            {
                field_1210_g = false;
                worldClient = new ClientWorld(this, worldClient.getWorldInfo().getRandomSeed(), var1.field_28048_a);
                worldClient.isRemote = true;
                mc.changeWorld1(worldClient);
                mc.thePlayer.dimension = var1.field_28048_a;
                mc.displayGuiScreen(new GuiDownloadTerrain(this));
            }

            mc.respawn(true, var1.field_28048_a);
        }

        public override void func_12245_a(Packet60Explosion var1)
        {
            Explosion var2 = new Explosion(mc.theWorld, (Entity)null, var1.explosionX, var1.explosionY, var1.explosionZ, var1.explosionSize);
            var2.destroyedBlockPositions = var1.destroyedBlockPositions;
            var2.doExplosionB(true);
        }

        public override void func_20087_a(Packet100OpenWindow var1)
        {
            if (var1.inventoryType == 0)
            {
                InventoryBasic var2 = new InventoryBasic(var1.windowTitle, var1.slotsCount);
                mc.thePlayer.displayGUIChest(var2);
                mc.thePlayer.craftingInventory.windowId = var1.windowId;
            }
            else if (var1.inventoryType == 2)
            {
                BlockEntityFurnace var3 = new BlockEntityFurnace();
                mc.thePlayer.displayGUIFurnace(var3);
                mc.thePlayer.craftingInventory.windowId = var1.windowId;
            }
            else if (var1.inventoryType == 3)
            {
                BlockEntityDispenser var4 = new BlockEntityDispenser();
                mc.thePlayer.displayGUIDispenser(var4);
                mc.thePlayer.craftingInventory.windowId = var1.windowId;
            }
            else if (var1.inventoryType == 1)
            {
                EntityPlayerSP var5 = mc.thePlayer;
                mc.thePlayer.displayWorkbenchGUI(MathHelper.floor_double(var5.posX), MathHelper.floor_double(var5.posY), MathHelper.floor_double(var5.posZ));
                mc.thePlayer.craftingInventory.windowId = var1.windowId;
            }

        }

        public override void func_20088_a(Packet103SetSlot var1)
        {
            if (var1.windowId == -1)
            {
                mc.thePlayer.inventory.setItemStack(var1.myItemStack);
            }
            else if (var1.windowId == 0 && var1.itemSlot >= 36 && var1.itemSlot < 45)
            {
                ItemStack var2 = mc.thePlayer.inventorySlots.getSlot(var1.itemSlot).getStack();
                if (var1.myItemStack != null && (var2 == null || var2.count < var1.myItemStack.count))
                {
                    var1.myItemStack.animationsToGo = 5;
                }

                mc.thePlayer.inventorySlots.putStackInSlot(var1.itemSlot, var1.myItemStack);
            }
            else if (var1.windowId == mc.thePlayer.craftingInventory.windowId)
            {
                mc.thePlayer.craftingInventory.putStackInSlot(var1.itemSlot, var1.myItemStack);
            }

        }

        public override void func_20089_a(Packet106Transaction var1)
        {
            Container var2 = null;
            if (var1.windowId == 0)
            {
                var2 = mc.thePlayer.inventorySlots;
            }
            else if (var1.windowId == mc.thePlayer.craftingInventory.windowId)
            {
                var2 = mc.thePlayer.craftingInventory;
            }

            if (var2 != null)
            {
                if (var1.field_20030_c)
                {
                    var2.func_20113_a(var1.field_20028_b);
                }
                else
                {
                    var2.func_20110_b(var1.field_20028_b);
                    addToSendQueue(new Packet106Transaction(var1.windowId, var1.field_20028_b, true));
                }
            }

        }

        public override void func_20094_a(Packet104WindowItems var1)
        {
            if (var1.windowId == 0)
            {
                mc.thePlayer.inventorySlots.putStacksInSlots(var1.itemStack);
            }
            else if (var1.windowId == mc.thePlayer.craftingInventory.windowId)
            {
                mc.thePlayer.craftingInventory.putStacksInSlots(var1.itemStack);
            }

        }

        public override void handleSignUpdate(UpdateSignPacket var1)
        {
            if (mc.theWorld.blockExists(var1.x, var1.y, var1.z))
            {
                BlockEntity var2 = mc.theWorld.getBlockEntity(var1.x, var1.y, var1.z);
                if (var2 is BlockEntitySign)
                {
                    BlockEntitySign var3 = (BlockEntitySign)var2;

                    for (int var4 = 0; var4 < 4; ++var4)
                    {
                        var3.texts[var4] = var1.text[var4];
                    }

                    var3.markDirty();
                }
            }

        }

        public override void func_20090_a(Packet105UpdateProgressbar var1)
        {
            registerPacket(var1);
            if (mc.thePlayer.craftingInventory != null && mc.thePlayer.craftingInventory.windowId == var1.windowId)
            {
                mc.thePlayer.craftingInventory.func_20112_a(var1.progressBar, var1.progressBarValue);
            }

        }

        public override void handlePlayerInventory(Packet5PlayerInventory var1)
        {
            Entity var2 = getEntityByID(var1.entityID);
            if (var2 != null)
            {
                var2.outfitWithItem(var1.slot, var1.itemID, var1.itemDamage);
            }

        }

        public override void func_20092_a(Packet101CloseWindow var1)
        {
            mc.thePlayer.closeScreen();
        }

        public override void handleNotePlay(Packet54PlayNoteBlock var1)
        {
            mc.theWorld.playNoteBlockActionAt(var1.xLocation, var1.yLocation, var1.zLocation, var1.instrumentType, var1.pitch);
        }

        public override void func_25118_a(Packet70Bed var1)
        {
            int var2 = var1.field_25019_b;
            if (var2 >= 0 && var2 < Packet70Bed.field_25020_a.Length && Packet70Bed.field_25020_a[var2] != null)
            {
                mc.thePlayer.addChatMessage(Packet70Bed.field_25020_a[var2]);
            }

            if (var2 == 1)
            {
                worldClient.getWorldInfo().setRaining(true);
                worldClient.func_27158_h(1.0F);
            }
            else if (var2 == 2)
            {
                worldClient.getWorldInfo().setRaining(false);
                worldClient.func_27158_h(0.0F);
            }

        }

        public override void func_28116_a(Packet131MapData var1)
        {
            if (var1.field_28055_a == Item.mapItem.id)
            {
                ItemMap.func_28013_a(var1.field_28054_b, mc.theWorld).func_28171_a(var1.field_28056_c);
            }
            else
            {
                java.lang.System.@out.println("Unknown itemid: " + var1.field_28054_b);
            }

        }

        public override void func_28115_a(Packet61DoorChange var1)
        {
            mc.theWorld.worldEvent(var1.field_28050_a, var1.field_28053_c, var1.field_28052_d, var1.field_28051_e, var1.field_28049_b);
        }

        public override void func_27245_a(Packet200Statistic var1)
        {
            ((EntityClientPlayerMP)mc.thePlayer).func_27027_b(Stats.Stats.func_27361_a(var1.field_27052_a), var1.field_27051_b);
        }

        public override bool isServerHandler()
        {
            return false;
        }
    }

}