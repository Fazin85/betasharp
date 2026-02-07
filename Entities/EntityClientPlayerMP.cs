using betareborn.Packets;
using betareborn.Stats;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityClientPlayerMP : EntityPlayerSP
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityClientPlayerMP).TypeHandle);

        public NetClientHandler sendQueue;
        private int field_9380_bx = 0;
        private bool field_21093_bH = false;
        private double oldPosX;
        private double field_9378_bz;
        private double oldPosY;
        private double oldPosZ;
        private float oldRotationYaw;
        private float oldRotationPitch;
        private bool field_9382_bF = false;
        private bool wasSneaking = false;
        private int field_12242_bI = 0;

        public EntityClientPlayerMP(Minecraft var1, World var2, Session var3, NetClientHandler var4) : base(var1, var2, var3, 0)
        {
            sendQueue = var4;
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            return false;
        }

        public override void heal(int var1)
        {
        }

        public override void onUpdate()
        {
            if (worldObj.blockExists(MathHelper.floor_double(posX), 64, MathHelper.floor_double(posZ)))
            {
                base.onUpdate();
                func_4056_N();
            }
        }

        public void func_4056_N()
        {
            if (field_9380_bx++ == 20)
            {
                sendInventoryChanged();
                field_9380_bx = 0;
            }

            bool var1 = isSneaking();
            if (var1 != wasSneaking)
            {
                if (var1)
                {
                    sendQueue.addToSendQueue(new Packet19EntityAction(this, 1));
                }
                else
                {
                    sendQueue.addToSendQueue(new Packet19EntityAction(this, 2));
                }

                wasSneaking = var1;
            }

            double var2 = posX - oldPosX;
            double var4 = boundingBox.minY - field_9378_bz;
            double var6 = posY - oldPosY;
            double var8 = posZ - oldPosZ;
            double var10 = (double)(rotationYaw - oldRotationYaw);
            double var12 = (double)(rotationPitch - oldRotationPitch);
            bool var14 = var4 != 0.0D || var6 != 0.0D || var2 != 0.0D || var8 != 0.0D;
            bool var15 = var10 != 0.0D || var12 != 0.0D;
            if (ridingEntity != null)
            {
                if (var15)
                {
                    sendQueue.addToSendQueue(new Packet11PlayerPosition(motionX, -999.0D, -999.0D, motionZ, onGround));
                }
                else
                {
                    sendQueue.addToSendQueue(new Packet13PlayerLookMove(motionX, -999.0D, -999.0D, motionZ, rotationYaw, rotationPitch, onGround));
                }

                var14 = false;
            }
            else if (var14 && var15)
            {
                sendQueue.addToSendQueue(new Packet13PlayerLookMove(posX, boundingBox.minY, posY, posZ, rotationYaw, rotationPitch, onGround));
                field_12242_bI = 0;
            }
            else if (var14)
            {
                sendQueue.addToSendQueue(new Packet11PlayerPosition(posX, boundingBox.minY, posY, posZ, onGround));
                field_12242_bI = 0;
            }
            else if (var15)
            {
                sendQueue.addToSendQueue(new Packet12PlayerLook(rotationYaw, rotationPitch, onGround));
                field_12242_bI = 0;
            }
            else
            {
                sendQueue.addToSendQueue(new Packet10Flying(onGround));
                if (field_9382_bF == onGround && field_12242_bI <= 200)
                {
                    ++field_12242_bI;
                }
                else
                {
                    field_12242_bI = 0;
                }
            }

            field_9382_bF = onGround;
            if (var14)
            {
                oldPosX = posX;
                field_9378_bz = boundingBox.minY;
                oldPosY = posY;
                oldPosZ = posZ;
            }

            if (var15)
            {
                oldRotationYaw = rotationYaw;
                oldRotationPitch = rotationPitch;
            }

        }

        public override void dropCurrentItem()
        {
            sendQueue.addToSendQueue(new Packet14BlockDig(4, 0, 0, 0, 0));
        }

        private void sendInventoryChanged()
        {
        }

        protected override void joinEntityItemWithWorld(EntityItem var1)
        {
        }

        public override void sendChatMessage(String var1)
        {
            sendQueue.addToSendQueue(new Packet3Chat(var1));
        }

        public override void swingItem()
        {
            base.swingItem();
            sendQueue.addToSendQueue(new Packet18Animation(this, 1));
        }

        public override void respawnPlayer()
        {
            sendInventoryChanged();
            sendQueue.addToSendQueue(new Packet9Respawn((sbyte)dimension));
        }

        protected override void damageEntity(int var1)
        {
            health -= var1;
        }

        public override void closeScreen()
        {
            sendQueue.addToSendQueue(new Packet101CloseWindow(craftingInventory.windowId));
            inventory.setItemStack(null);
            base.closeScreen();
        }

        public override void setHealth(int var1)
        {
            if (field_21093_bH)
            {
                base.setHealth(var1);
            }
            else
            {
                health = var1;
                field_21093_bH = true;
            }

        }

        public override void addStat(StatBase var1, int var2)
        {
            if (var1 != null)
            {
                if (var1.localOnly)
                {
                    base.addStat(var1, var2);
                }

            }
        }

        public void func_27027_b(StatBase var1, int var2)
        {
            if (var1 != null)
            {
                if (!var1.localOnly)
                {
                    base.addStat(var1, var2);
                }

            }
        }
    }

}