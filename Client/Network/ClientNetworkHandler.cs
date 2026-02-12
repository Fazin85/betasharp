using betareborn.Blocks;
using betareborn.Blocks.Entities;
using betareborn.Client.Guis;
using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Network;
using betareborn.Network.Packets;
using betareborn.Network.Packets.Play;
using betareborn.Network.Packets.S2CPlay;
using betareborn.Screens;
using betareborn.Util.Maths;
using betareborn.Worlds;
using betareborn.Worlds.Chunks;
using betareborn.Worlds.Storage;
using java.io;
using java.net;

namespace betareborn.Client.Network
{
    public class ClientNetworkHandler : NetHandler
    {
        private bool disconnected = false;
        private Connection netManager;
        public string field_1209_a;
        private Minecraft mc;
        private ClientWorld worldClient;
        private bool field_1210_g = false;
        public PersistentStateManager clientPersistentStateManager = new PersistentStateManager(null);
        java.util.Random rand = new();

        public ClientNetworkHandler(Minecraft var1, string var2, int var3)
        {

            mc = var1;
            Socket socket = new Socket(InetAddress.getByName(var2), var3);
            socket.setTcpNoDelay(true);
            netManager = new Connection(socket, "Client", this);
        }

        public void tick()
        {
            if (!disconnected)
            {
                netManager.tick();
            }

            netManager.interrupt();
        }

        public override void onHello(LoginHelloPacket var1)
        {
            mc.playerController = new PlayerControllerMP(mc, this);
            mc.statFileWriter.readStat(Stats.Stats.joinMultiplayerStat, 1);
            worldClient = new ClientWorld(this, var1.worldSeed, var1.dimensionId);
            worldClient.isRemote = true;
            mc.changeWorld1(worldClient);
            mc.player.dimensionId = var1.dimensionId;
            mc.displayGuiScreen(new GuiDownloadTerrain(this));
            mc.player.id = var1.protocolVersion;
        }

        public override void onItemEntitySpawn(ItemEntitySpawnS2CPacket var1)
        {
            double var2 = var1.x / 32.0D;
            double var4 = var1.y / 32.0D;
            double var6 = var1.z / 32.0D;
            EntityItem var8 = new EntityItem(worldClient, var2, var4, var6, new ItemStack(var1.itemRawId, var1.itemCount, var1.itemDamage));
            var8.velocityX = var1.velocityX / 128.0D;
            var8.velocityY = var1.velocityY / 128.0D;
            var8.velocityZ = var1.velocityZ / 128.0D;
            var8.trackedPosX = var1.x;
            var8.trackedPosY = var1.y;
            var8.trackedPosZ = var1.z;
            worldClient.forceEntity(var1.id, var8);
        }

        public override void onEntitySpawn(EntitySpawnS2CPacket var1)
        {
            double var2 = var1.x / 32.0D;
            double var4 = var1.y / 32.0D;
            double var6 = var1.z / 32.0D;
            object var8 = null;
            if (var1.entityType == 10)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 0);
            }

            if (var1.entityType == 11)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 1);
            }

            if (var1.entityType == 12)
            {
                var8 = new EntityMinecart(worldClient, var2, var4, var6, 2);
            }

            if (var1.entityType == 90)
            {
                var8 = new EntityFish(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 60)
            {
                var8 = new EntityArrow(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 61)
            {
                var8 = new EntitySnowball(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 63)
            {
                var8 = new EntityFireball(worldClient, var2, var4, var6, var1.velocityX / 8000.0D, var1.velocityY / 8000.0D, var1.velocityZ / 8000.0D);
                var1.entityData = 0;
            }

            if (var1.entityType == 62)
            {
                var8 = new EntityEgg(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 1)
            {
                var8 = new EntityBoat(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 50)
            {
                var8 = new EntityTNTPrimed(worldClient, var2, var4, var6);
            }

            if (var1.entityType == 70)
            {
                var8 = new EntityFallingSand(worldClient, var2, var4, var6, Block.SAND.id);
            }

            if (var1.entityType == 71)
            {
                var8 = new EntityFallingSand(worldClient, var2, var4, var6, Block.GRAVEL.id);
            }

            if (var8 != null)
            {
                ((Entity)var8).trackedPosX = var1.x;
                ((Entity)var8).trackedPosY = var1.y;
                ((Entity)var8).trackedPosZ = var1.z;
                ((Entity)var8).yaw = 0.0F;
                ((Entity)var8).pitch = 0.0F;
                ((Entity)var8).id = var1.id;
                worldClient.forceEntity(var1.id, (Entity)var8);
                if (var1.entityData > 0)
                {
                    if (var1.entityType == 60)
                    {
                        Entity var9 = getEntityByID(var1.entityData);
                        if (var9 is EntityLiving)
                        {
                            ((EntityArrow)var8).owner = (EntityLiving)var9;
                        }
                    }

                    ((Entity)var8).setVelocityClient(var1.velocityX / 8000.0D, var1.velocityY / 8000.0D, var1.velocityZ / 8000.0D);
                }
            }

        }

        public override void onLightningEntitySpawn(GlobalEntitySpawnS2CPacket var1)
        {
            double var2 = var1.x / 32.0D;
            double var4 = var1.y / 32.0D;
            double var6 = var1.z / 32.0D;
            EntityLightningBolt var8 = null;
            if (var1.type == 1)
            {
                var8 = new EntityLightningBolt(worldClient, var2, var4, var6);
            }

            if (var8 != null)
            {
                var8.trackedPosX = var1.x;
                var8.trackedPosY = var1.y;
                var8.trackedPosZ = var1.z;
                var8.yaw = 0.0F;
                var8.pitch = 0.0F;
                var8.id = var1.id;
                worldClient.spawnGlobalEntity(var8);
            }

        }

        public override void onPaintingEntitySpawn(PaintingEntitySpawnS2CPacket var1)
        {
            EntityPainting var2 = new EntityPainting(worldClient, var1.xPosition, var1.yPosition, var1.zPosition, var1.direction, var1.title);
            worldClient.forceEntity(var1.entityId, var2);
        }

        public override void onEntityVelocityUpdate(EntityVelocityUpdateS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.setVelocityClient(var1.motionX / 8000.0D, var1.motionY / 8000.0D, var1.motionZ / 8000.0D);
            }
        }

        public override void onEntityTrackerUpdate(EntityTrackerUpdateS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null && var1.func_21047_b() != null)
            {
                var2.getDataWatcher().updateWatchedObjectsFromList(var1.func_21047_b());
            }

        }

        public override void onPlayerSpawn(PlayerSpawnS2CPacket var1)
        {
            double var2 = var1.xPosition / 32.0D;
            double var4 = var1.yPosition / 32.0D;
            double var6 = var1.zPosition / 32.0D;
            float var8 = var1.rotation * 360 / 256.0F;
            float var9 = var1.pitch * 360 / 256.0F;
            OtherPlayerEntity var10 = new OtherPlayerEntity(mc.world, var1.name);
            var10.prevX = var10.lastTickX = var10.trackedPosX = var1.xPosition;
            var10.prevY = var10.lastTickY = var10.trackedPosY = var1.yPosition;
            var10.prevZ = var10.lastTickZ = var10.trackedPosZ = var1.zPosition;
            int var11 = var1.currentItem;
            if (var11 == 0)
            {
                var10.inventory.main[var10.inventory.selectedSlot] = null;
            }
            else
            {
                var10.inventory.main[var10.inventory.selectedSlot] = new ItemStack(var11, 1, 0);
            }

            var10.setPositionAndAngles(var2, var4, var6, var8, var9);
            worldClient.forceEntity(var1.entityId, var10);
        }

        public override void onEntityPosition(EntityPositionS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null)
            {
                var2.trackedPosX = var1.x;
                var2.trackedPosY = var1.y;
                var2.trackedPosZ = var1.z;
                double var3 = var2.trackedPosX / 32.0D;
                double var5 = var2.trackedPosY / 32.0D + 1.0D / 64.0D;
                double var7 = var2.trackedPosZ / 32.0D;
                float var9 = var1.yaw * 360 / 256.0F;
                float var10 = var1.pitch * 360 / 256.0F;
                var2.setPositionAndAnglesAvoidEntities(var3, var5, var7, var9, var10, 3);
            }
        }

        public override void onEntity(EntityS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null)
            {
                var2.trackedPosX += var1.deltaX;
                var2.trackedPosY += var1.deltaY;
                var2.trackedPosZ += var1.deltaZ;
                double var3 = var2.trackedPosX / 32.0D;
                double var5 = var2.trackedPosY / 32.0D;
                double var7 = var2.trackedPosZ / 32.0D;
                float var9 = var1.rotate ? var1.yaw * 360 / 256.0F : var2.yaw;
                float var10 = var1.rotate ? var1.pitch * 360 / 256.0F : var2.pitch;
                var2.setPositionAndAnglesAvoidEntities(var3, var5, var7, var9, var10, 3);
            }
        }

        public override void onEntityDestroy(EntityDestroyS2CPacket var1)
        {
            worldClient.removeEntityFromWorld(var1.entityId);
        }

        public override void onPlayerMove(PlayerMovePacket var1)
        {
            ClientPlayerEntity var2 = mc.player;
            double var3 = var2.x;
            double var5 = var2.y;
            double var7 = var2.z;
            float var9 = var2.yaw;
            float var10 = var2.pitch;
            if (var1.changePosition)
            {
                var3 = var1.x;
                var5 = var1.y;
                var7 = var1.z;
            }

            if (var1.changeLook)
            {
                var9 = var1.yaw;
                var10 = var1.pitch;
            }

            var2.cameraOffset = 0.0F;
            var2.velocityX = var2.velocityY = var2.velocityZ = 0.0D;
            var2.setPositionAndAngles(var3, var5, var7, var9, var10);
            var1.x = var2.x;
            var1.y = var2.boundingBox.minY;
            var1.z = var2.z;
            var1.eyeHeight = var2.y;
            netManager.sendPacket(var1);
            if (!field_1210_g)
            {
                mc.player.prevX = mc.player.x;
                mc.player.prevY = mc.player.y;
                mc.player.prevZ = mc.player.z;
                field_1210_g = true;
                mc.displayGuiScreen(null);
            }

        }

        public override void onChunkStatusUpdate(ChunkStatusUpdateS2CPacket var1)
        {
            worldClient.updateChunk(var1.x, var1.z, var1.load);
        }

        public override void onChunkDeltaUpdate(ChunkDeltaUpdateS2CPacket var1)
        {
            Chunk var2 = worldClient.getChunk(var1.x, var1.z);
            int var3 = var1.x * 16;
            int var4 = var1.z * 16;

            for (int var5 = 0; var5 < var1._size; ++var5)
            {
                short var6 = var1.positions[var5];
                int var7 = var1.blockRawIds[var5] & 255;
                byte var8 = var1.blockMetadata[var5];
                int var9 = var6 >> 12 & 15;
                int var10 = var6 >> 8 & 15;
                int var11 = var6 & 255;
                var2.setBlock(var9, var11, var10, var7, var8);
                worldClient.clearBlockResets(var9 + var3, var11, var10 + var4, var9 + var3, var11, var10 + var4);
                worldClient.setBlocksDirty(var9 + var3, var11, var10 + var4, var9 + var3, var11, var10 + var4);
            }

        }

        public override void handleChunkData(ChunkDataS2CPacket var1)
        {
            worldClient.clearBlockResets(var1.x, var1.y, var1.z, var1.x + var1.sizeX - 1, var1.y + var1.sizeY - 1, var1.z + var1.sizeZ - 1);
            worldClient.handleChunkDataUpdate(var1.x, var1.y, var1.z, var1.sizeX, var1.sizeY, var1.sizeZ, var1.chunkData);
        }

        public override void onBlockUpdate(BlockUpdateS2CPacket var1)
        {
            worldClient.setBlockWithMetaFromPacket(var1.x, var1.y, var1.z, var1.blockRawId, var1.blockMetadata);
        }

        public override void onDisconnect(DisconnectPacket var1)
        {
            netManager.disconnect("disconnect.kicked", new object[0]);
            disconnected = true;
            mc.changeWorld1(null);
            mc.displayGuiScreen(new GuiConnectFailed("disconnect.disconnected", "disconnect.genericReason", new object[] { var1.reason }));
        }

        public override void onDisconnected(string var1, object[] var2)
        {
            if (!disconnected)
            {
                disconnected = true;
                mc.changeWorld1(null);
                mc.displayGuiScreen(new GuiConnectFailed("disconnect.lost", var1, var2));
            }
        }

        public void sendPacketAndDisconnect(Packet var1)
        {
            if (!disconnected)
            {
                netManager.sendPacket(var1);
                netManager.disconnect();
            }
        }

        public void addToSendQueue(Packet var1)
        {
            if (!disconnected)
            {
                netManager.sendPacket(var1);
            }
        }

        public override void onItemPickupAnimation(ItemPickupAnimationS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            object var3 = (EntityLiving)getEntityByID(var1.collectorEntityId);
            if (var3 == null)
            {
                var3 = mc.player;
            }

            if (var2 != null)
            {
                worldClient.playSound(var2, "random.pop", 0.2F, ((rand.nextFloat() - rand.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                mc.particleManager.addEffect(new EntityPickupFX(mc.world, var2, (Entity)var3, -0.5F));
                worldClient.removeEntityFromWorld(var1.entityId);
            }

        }

        public override void onChatMessage(ChatMessagePacket var1)
        {
            mc.ingameGUI.addChatMessage(var1.chatMessage);
        }

        public override void onEntityAnimation(EntityAnimationPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null)
            {
                EntityPlayer var3;
                if (var1.animationId == 1)
                {
                    var3 = (EntityPlayer)var2;
                    var3.swingHand();
                }
                else if (var1.animationId == 2)
                {
                    var2.animateHurt();
                }
                else if (var1.animationId == 3)
                {
                    var3 = (EntityPlayer)var2;
                    var3.wakeUp(false, false, false);
                }
                else if (var1.animationId == 4)
                {
                    var3 = (EntityPlayer)var2;
                    var3.spawn();
                }

            }
        }

        public override void onPlayerSleepUpdate(PlayerSleepUpdateS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null)
            {
                if (var1.status == 0)
                {
                    EntityPlayer var3 = (EntityPlayer)var2;
                    var3.trySleep(var1.x, var1.y, var1.z);
                }

            }
        }

        public override void onHandshake(HandshakePacket var1)
        {
            if (var1.username.Equals("-"))
            {
                addToSendQueue(new LoginHelloPacket(mc.session.username, 14));
            }
            else
            {
                try
                {
                    URL var2 = new URL("http://www.minecraft.net/game/joinserver.jsp?user=" + mc.session.username + "&sessionId=" + mc.session.sessionId + "&serverId=" + var1.username);
                    BufferedReader var3 = new BufferedReader(new InputStreamReader(var2.openStream()));
                    string var4 = var3.readLine();
                    var3.close();
                    //TODO: AUTH
                    if (var4 == null || var4.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        addToSendQueue(new LoginHelloPacket(mc.session.username, 14));
                    }
                    else
                    {
                        netManager.disconnect("disconnect.loginFailedInfo", new object[] { var4 });
                    }
                }
                catch (java.lang.Exception var5)
                {
                    var5.printStackTrace();
                    netManager.disconnect("disconnect.genericReason", new object[] { "Internal client error: " + var5.toString() });
                }
            }

        }

        public void disconnect()
        {
            disconnected = true;
            netManager.interrupt();
            netManager.disconnect("disconnect.closed", new object[0]);
        }

        public override void onLivingEntitySpawn(LivingEntitySpawnS2CPacket var1)
        {
            double var2 = var1.xPosition / 32.0D;
            double var4 = var1.yPosition / 32.0D;
            double var6 = var1.zPosition / 32.0D;
            float var8 = var1.yaw * 360 / 256.0F;
            float var9 = var1.pitch * 360 / 256.0F;
            EntityLiving var10 = (EntityLiving)EntityRegistry.create(var1.type, mc.world);
            var10.trackedPosX = var1.xPosition;
            var10.trackedPosY = var1.yPosition;
            var10.trackedPosZ = var1.zPosition;
            var10.id = var1.entityId;
            var10.setPositionAndAngles(var2, var4, var6, var8, var9);
            var10.interpolateOnly = true;
            worldClient.forceEntity(var1.entityId, var10);
            java.util.List var11 = var1.getMetadata();
            if (var11 != null)
            {
                var10.getDataWatcher().updateWatchedObjectsFromList(var11);
            }

        }

        public override void onWorldTimeUpdate(WorldTimeUpdateS2CPacket var1)
        {
            mc.world.setTime(var1.time);
        }

        public override void onPlayerSpawnPosition(PlayerSpawnPositionS2CPacket var1)
        {
            mc.player.setSpawnPos(new Vec3i(var1.x, var1.y, var1.z));
            mc.world.getProperties().setSpawn(var1.x, var1.y, var1.z);
        }

        public override void onEntityVehicleSet(EntityVehicleSetS2CPacket var1)
        {
            object var2 = getEntityByID(var1.entityId);
            Entity var3 = getEntityByID(var1.vehicleEntityId);
            if (var1.entityId == mc.player.id)
            {
                var2 = mc.player;
            }

            if (var2 != null)
            {
                ((Entity)var2).setVehicle(var3);
            }
        }

        public override void onEntityStatus(EntityStatusS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.entityId);
            if (var2 != null)
            {
                var2.processServerEntityStatus(var1.entityStatus);
            }

        }

        private Entity getEntityByID(int var1)
        {
            return var1 == mc.player.id ? mc.player : worldClient.getEntity(var1);
        }

        public override void onHealthUpdate(HealthUpdateS2CPacket var1)
        {
            mc.player.setHealth(var1.healthMP);
        }

        public override void onPlayerRespawn(PlayerRespawnPacket var1)
        {
            if (var1.field_28048_a != mc.player.dimensionId)
            {
                field_1210_g = false;
                worldClient = new ClientWorld(this, worldClient.getProperties().getRandomSeed(), var1.field_28048_a);
                worldClient.isRemote = true;
                mc.changeWorld1(worldClient);
                mc.player.dimensionId = var1.field_28048_a;
                mc.displayGuiScreen(new GuiDownloadTerrain(this));
            }

            mc.respawn(true, var1.field_28048_a);
        }

        public override void onExplosion(ExplosionS2CPacket var1)
        {
            Explosion var2 = new Explosion(mc.world, null, var1.explosionX, var1.explosionY, var1.explosionZ, var1.explosionSize);
            var2.destroyedBlockPositions = var1.destroyedBlockPositions;
            var2.doExplosionB(true);
        }

        public override void onOpenScreen(OpenScreenS2CPacket var1)
        {
            if (var1.screenHandlerId == 0)
            {
                InventoryBasic var2 = new InventoryBasic(var1.name, var1.slotsCount);
                mc.player.openChestScreen(var2);
                mc.player.currentScreenHandler.syncId = var1.syncId;
            }
            else if (var1.screenHandlerId == 2)
            {
                BlockEntityFurnace var3 = new BlockEntityFurnace();
                mc.player.openFurnaceScreen(var3);
                mc.player.currentScreenHandler.syncId = var1.syncId;
            }
            else if (var1.screenHandlerId == 3)
            {
                BlockEntityDispenser var4 = new BlockEntityDispenser();
                mc.player.openDispenserScreen(var4);
                mc.player.currentScreenHandler.syncId = var1.syncId;
            }
            else if (var1.screenHandlerId == 1)
            {
                ClientPlayerEntity var5 = mc.player;
                mc.player.openCraftingScreen(MathHelper.floor_double(var5.x), MathHelper.floor_double(var5.y), MathHelper.floor_double(var5.z));
                mc.player.currentScreenHandler.syncId = var1.syncId;
            }

        }

        public override void onScreenHandlerSlotUpdate(ScreenHandlerSlotUpdateS2CPacket var1)
        {
            if (var1.syncId == -1)
            {
                mc.player.inventory.setItemStack(var1.stack);
            }
            else if (var1.syncId == 0 && var1.slot >= 36 && var1.slot < 45)
            {
                ItemStack var2 = mc.player.playerScreenHandler.getSlot(var1.slot).getStack();
                if (var1.stack != null && (var2 == null || var2.count < var1.stack.count))
                {
                    var1.stack.bobbingAnimationTime = 5;
                }

                mc.player.playerScreenHandler.setStackInSlot(var1.slot, var1.stack);
            }
            else if (var1.syncId == mc.player.currentScreenHandler.syncId)
            {
                mc.player.currentScreenHandler.setStackInSlot(var1.slot, var1.stack);
            }

        }

        public override void onScreenHandlerAcknowledgement(ScreenHandlerAcknowledgementPacket var1)
        {
            ScreenHandler var2 = null;
            if (var1.syncId == 0)
            {
                var2 = mc.player.playerScreenHandler;
            }
            else if (var1.syncId == mc.player.currentScreenHandler.syncId)
            {
                var2 = mc.player.currentScreenHandler;
            }

            if (var2 != null)
            {
                if (var1.accepted)
                {
                    var2.onAcknowledgementAccepted(var1.actionType);
                }
                else
                {
                    var2.onAcknowledgementDenied(var1.actionType);
                    addToSendQueue(new ScreenHandlerAcknowledgementPacket(var1.syncId, var1.actionType, true));
                }
            }

        }

        public override void onInventory(InventoryS2CPacket var1)
        {
            if (var1.syncId == 0)
            {
                mc.player.playerScreenHandler.updateSlotStacks(var1.contents);
            }
            else if (var1.syncId == mc.player.currentScreenHandler.syncId)
            {
                mc.player.currentScreenHandler.updateSlotStacks(var1.contents);
            }

        }

        public override void handleUpdateSign(UpdateSignPacket var1)
        {
            if (mc.world.isPosLoaded(var1.x, var1.y, var1.z))
            {
                BlockEntity var2 = mc.world.getBlockEntity(var1.x, var1.y, var1.z);
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

        public override void onScreenHandlerPropertyUpdate(ScreenHandlerPropertyUpdateS2CPacket var1)
        {
            handle(var1);
            if (mc.player.currentScreenHandler != null && mc.player.currentScreenHandler.syncId == var1.syncId)
            {
                mc.player.currentScreenHandler.setProperty(var1.propertyId, var1.value);
            }

        }

        public override void onEntityEquipmentUpdate(EntityEquipmentUpdateS2CPacket var1)
        {
            Entity var2 = getEntityByID(var1.id);
            if (var2 != null)
            {
                var2.setEquipmentStack(var1.slot, var1.itemRawId, var1.itemDamage);
            }

        }

        public override void onCloseScreen(CloseScreenS2CPacket var1)
        {
            mc.player.closeHandledScreen();
        }

        public override void onPlayNoteSound(PlayNoteSoundS2CPacket var1)
        {
            mc.world.playNoteBlockActionAt(var1.xLocation, var1.yLocation, var1.zLocation, var1.instrumentType, var1.pitch);
        }

        public override void onGameStateChange(GameStateChangeS2CPacket var1)
        {
            int var2 = var1.reason;
            if (var2 >= 0 && var2 < GameStateChangeS2CPacket.REASONS.Length && GameStateChangeS2CPacket.REASONS[var2] != null)
            {
                mc.player.sendMessage(GameStateChangeS2CPacket.REASONS[var2]);
            }

            if (var2 == 1)
            {
                worldClient.getProperties().setRaining(true);
                worldClient.setRainGradient(1.0F);
            }
            else if (var2 == 2)
            {
                worldClient.getProperties().setRaining(false);
                worldClient.setRainGradient(0.0F);
            }

        }

        public override void onMapUpdate(MapUpdateS2CPacket var1)
        {
            if (var1.itemRawId == Item.MAP.id)
            {
                ItemMap.getMapState(var1.id, mc.world).func_28171_a(var1.updateData);
            }
            else
            {
                java.lang.System.@out.println("Unknown itemid: " + var1.id);
            }

        }

        public override void onWorldEvent(WorldEventS2CPacket var1)
        {
            mc.world.worldEvent(var1.eventId, var1.x, var1.y, var1.z, var1.data);
        }

        public override void onIncreaseStat(IncreaseStatS2CPacket var1)
        {
            ((EntityClientPlayerMP)mc.player).func_27027_b(Stats.Stats.getStatById(var1.statId), var1.amount);
        }

        public override bool isServerSide()
        {
            return false;
        }
    }

}