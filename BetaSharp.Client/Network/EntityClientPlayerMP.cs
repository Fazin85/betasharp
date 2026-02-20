using BetaSharp.Client.Entities;
using BetaSharp.Entities;
using BetaSharp.Network.Packets.C2SPlay;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Stats;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Network;

public class EntityClientPlayerMP : ClientPlayerEntity
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityClientPlayerMP).TypeHandle);

    public ClientNetworkHandler sendQueue;
    private int inventorySyncTickCounter;
    private bool hasReceivedInitialHealth;
    private double oldPosX;
    private double lastSentMinY;
    private double oldPosY;
    private double oldPosZ;
    private float oldRotationYaw;
    private float oldRotationPitch;
    private bool lastOnGround;
    private bool wasSneaking;
    private int positionOnlyPacketCount;

    public EntityClientPlayerMP(Minecraft mc, World world, Session session, ClientNetworkHandler clientNetworkHandler) : base(mc, world, session, 0)
    {
        sendQueue = clientNetworkHandler;
    }

    public override bool damage(Entity ent, int amount)
    {
        return false;
    }

    public override void heal(int amount)
    {
    }

    public override void tick()
    {
        if (world.isPosLoaded(MathHelper.Floor(x), 64, MathHelper.Floor(z)))
        {
            base.tick();
            func_4056_N();
        }
    }

    public void func_4056_N()
    {
        if (inventorySyncTickCounter++ == 20)
        {
            sendInventoryChanged();
            inventorySyncTickCounter = 0;
        }

        bool isSneaking = base.isSneaking();
        if (isSneaking != wasSneaking)
        {
            if (isSneaking)
            {
                sendQueue.addToSendQueue(new ClientCommandC2SPacket(this, 1));
            }
            else
            {
                sendQueue.addToSendQueue(new ClientCommandC2SPacket(this, 2));
            }

            wasSneaking = isSneaking;
        }

        double dx = x - oldPosX;
        double dMinY = boundingBox.minY - lastSentMinY;
        double dy = y - oldPosY;
        double dz = z - oldPosZ;
        double dYaw = (double)(yaw - oldRotationYaw);
        double yPitch = (double)(pitch - oldRotationPitch);
        bool positionChanged = dMinY != 0.0D || dy != 0.0D || dx != 0.0D || dz != 0.0D;
        bool rotationChanged = dYaw != 0.0D || yPitch != 0.0D;
        if (vehicle != null)
        {
            if (rotationChanged)
            {
                sendQueue.addToSendQueue(new PlayerMovePositionAndOnGroundPacket(velocityX, -999.0D, -999.0D, velocityZ, onGround));
            }
            else
            {
                sendQueue.addToSendQueue(new PlayerMoveFullPacket(velocityX, -999.0D, -999.0D, velocityZ, yaw, pitch, onGround));
            }

            positionChanged = false;
        }
        else if (positionChanged && rotationChanged)
        {
            sendQueue.addToSendQueue(new PlayerMoveFullPacket(x, boundingBox.minY, y, z, yaw, pitch, onGround));
            positionOnlyPacketCount = 0;
        }
        else if (positionChanged)
        {
            sendQueue.addToSendQueue(new PlayerMovePositionAndOnGroundPacket(x, boundingBox.minY, y, z, onGround));
            positionOnlyPacketCount = 0;
        }
        else if (rotationChanged)
        {
            sendQueue.addToSendQueue(new PlayerMoveLookAndOnGroundPacket(yaw, pitch, onGround));
            positionOnlyPacketCount = 0;
        }
        else
        {
            sendQueue.addToSendQueue(new PlayerMovePacket(onGround));
            if (lastOnGround == onGround && positionOnlyPacketCount <= 200)
            {
                ++positionOnlyPacketCount;
            }
            else
            {
                positionOnlyPacketCount = 0;
            }
        }

        lastOnGround = onGround;
        if (positionChanged)
        {
            oldPosX = x;
            lastSentMinY = boundingBox.minY;
            oldPosY = y;
            oldPosZ = z;
        }

        if (rotationChanged)
        {
            oldRotationYaw = yaw;
            oldRotationPitch = pitch;
        }

    }

    public override void dropSelectedItem()
    {
        sendQueue.addToSendQueue(new PlayerActionC2SPacket(4, 0, 0, 0, 0));
    }

    private void sendInventoryChanged()
    {
    }

    protected override void spawnItem(EntityItem ent)
    {
    }

    public override void sendChatMessage(string message)
    {
        sendQueue.addToSendQueue(new ChatMessagePacket(message));
    }

    public override void swingHand()
    {
        base.swingHand();
        sendQueue.addToSendQueue(new EntityAnimationPacket(this, 1));
    }

    public override void respawn()
    {
        sendInventoryChanged();
        sendQueue.addToSendQueue(new PlayerRespawnPacket((sbyte)dimensionId));
    }

    protected override void applyDamage(int amount)
    {
        health -= amount;
    }

    public override void closeHandledScreen()
    {
        sendQueue.addToSendQueue(new CloseScreenS2CPacket(currentScreenHandler.syncId));
        inventory.setItemStack(null);
        base.closeHandledScreen();
    }

    public override void setHealth(int amount)
    {
        if (hasReceivedInitialHealth)
        {
            base.setHealth(amount);
        }
        else
        {
            health = amount;
            hasReceivedInitialHealth = true;
        }

    }

    public override void increaseStat(StatBase stat, int amount)
    {
        if (stat != null)
        {
            if (stat.localOnly)
            {
                base.increaseStat(stat, amount);
            }

        }
    }

    public void func_27027_b(StatBase stat, int amount)
    {
        if (stat != null)
        {
            if (!stat.localOnly)
            {
                base.increaseStat(stat, amount);
            }

        }
    }
}
