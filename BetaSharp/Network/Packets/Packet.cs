using BetaSharp.Network.Packets.C2SPlay;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using java.lang;
using java.util;

namespace BetaSharp.Network.Packets;

public abstract class Packet : java.lang.Object
{
    private static Map IO_TO_TYPE = new HashMap();
    private static Map TYPE_TO_ID = new HashMap();
    private static Set S2C = new HashSet();
    private static Set C2S = new HashSet();
    public readonly long creationTime = java.lang.System.currentTimeMillis();
    public bool worldPacket = false;
    private static HashMap trackers;
    private static int incomingCount;

    static void register(int rawId, bool clientBound, bool serverBound, Class type)
    {
        if (IO_TO_TYPE.containsKey(Integer.valueOf(rawId)))
        {
            throw new IllegalArgumentException("Duplicate packet id:" + rawId);
        }
        else if (TYPE_TO_ID.containsKey(type))
        {
            throw new IllegalArgumentException("Duplicate packet class:" + type);
        }
        else
        {
            IO_TO_TYPE.put(Integer.valueOf(rawId), type);
            TYPE_TO_ID.put(type, Integer.valueOf(rawId));
            if (clientBound)
            {
                S2C.add(Integer.valueOf(rawId));
            }

            if (serverBound)
            {
                C2S.add(Integer.valueOf(rawId));
            }

        }
    }

    public static Packet create(int rawId)
    {
        try
        {
            Class packetClass = (Class)IO_TO_TYPE.get(Integer.valueOf(rawId));
            return packetClass == null ? null : (Packet)packetClass.newInstance();
        }
        catch (java.lang.Exception ex)
        {
            Log.Error(ex);
            Log.Info($"Skipping packet with id {rawId}");
            return null;
        }
    }

    public int getRawId()
    {
        return ((Integer)TYPE_TO_ID.get(getClass())).intValue();
    }

    public static Packet read(java.io.DataInputStream stream, bool server)
    {
        Packet packet = null;


        int rawId;
        try
        {
            rawId = stream.read();
            if (rawId == -1)
            {
                return null;
            }

            if (server && !C2S.contains(Integer.valueOf(rawId)) || !server && !S2C.contains(Integer.valueOf(rawId)))
            {
                throw new java.io.IOException("Bad packet id " + rawId);
            }

            packet = create(rawId);
            if (packet == null)
            {
                throw new java.io.IOException("Bad packet id " + rawId);
            }

            packet.read(stream);
        }
        catch (java.io.EOFException)
        {
            Log.Info("Reached end of stream");
            return null;
        }

        PacketTracker tracker = (PacketTracker)trackers.get(Integer.valueOf(rawId));
        if (tracker == null)
        {
            tracker = new PacketTracker();
            trackers.put(Integer.valueOf(rawId), tracker);
        }

        tracker.update(packet.size());
        ++incomingCount;
        if (incomingCount % 1000 == 0)
        {
        }

        return packet;
    }

    public static void write(Packet packet, java.io.DataOutputStream stream)
    {
        stream.write(packet.getRawId());
        packet.write(stream);
    }

    public static void writeString(string packetData, java.io.DataOutputStream stream)
    {
        if (packetData.Length > Short.MAX_VALUE)
        {
            throw new java.io.IOException("String too big");
        }
        else
        {
            stream.writeShort(packetData.Length);
            stream.writeChars(packetData);
        }
    }

    public static string readString(java.io.DataInputStream stream, int maxLength)
    {

        short length = stream.readShort();
        if (length > maxLength)
        {
            throw new java.io.IOException("Received string length longer than maximum allowed (" + length + " > " + maxLength + ")");
        }
        else if (length < 0)
        {
            throw new java.io.IOException("Received string length is less than zero! Weird string!");
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            for (int var4 = 0; var4 < length; ++var4)
            {
                sb.append(stream.readChar());
            }

            return sb.toString();
        }
    }

    public abstract void read(java.io.DataInputStream stream);

    public abstract void write(java.io.DataOutputStream stream);

    public abstract void apply(NetHandler handler);

    public abstract int size();

    public virtual void ProcessForInternal()
    {
    }

    static Packet()
    {
        register(0, true, true, KeepAlivePacket.Class);
        register(1, true, true, LoginHelloPacket.Class);
        register(2, true, true, HandshakePacket.Class);
        register(3, true, true, ChatMessagePacket.Class);
        register(4, true, false, WorldTimeUpdateS2CPacket.Class);
        register(5, true, false, EntityEquipmentUpdateS2CPacket.Class);
        register(6, true, false, PlayerSpawnPositionS2CPacket.Class);
        register(7, false, true, PlayerInteractEntityC2SPacket.Class);
        register(8, true, false, HealthUpdateS2CPacket.Class);
        register(9, true, true, PlayerRespawnPacket.Class);
        register(10, true, true, PlayerMovePacket.Class);
        register(11, true, true, PlayerMovePositionAndOnGroundPacket.Class);
        register(12, true, true, PlayerMoveLookAndOnGroundPacket.Class);
        register(13, true, true, PlayerMoveFullPacket.Class);
        register(14, false, true, PlayerActionC2SPacket.Class);
        register(15, false, true, PlayerInteractBlockC2SPacket.Class);
        register(16, false, true, UpdateSelectedSlotC2SPacket.Class);
        register(17, true, false, PlayerSleepUpdateS2CPacket.Class);
        register(18, true, true, EntityAnimationPacket.Class);
        register(19, false, true, ClientCommandC2SPacket.Class);
        register(20, true, false, PlayerSpawnS2CPacket.Class);
        register(21, true, false, ItemEntitySpawnS2CPacket.Class);
        register(22, true, false, ItemPickupAnimationS2CPacket.Class);
        register(23, true, false, EntitySpawnS2CPacket.Class);
        register(24, true, false, LivingEntitySpawnS2CPacket.Class);
        register(25, true, false, PaintingEntitySpawnS2CPacket.Class);
        register(27, false, true, PlayerInputC2SPacket.Class);
        register(28, true, false, EntityVelocityUpdateS2CPacket.Class);
        register(29, true, false, EntityDestroyS2CPacket.Class);
        register(30, true, false, EntityS2CPacket.Class);
        register(31, true, false, EntityMoveRelativeS2CPacket.Class);
        register(32, true, false, EntityRotateS2CPacket.Class);
        register(33, true, false, EntityRotateAndMoveRelativeS2CPacket.Class);
        register(34, true, false, EntityPositionS2CPacket.Class);
        register(38, true, false, EntityStatusS2CPacket.Class);
        register(39, true, false, EntityVehicleSetS2CPacket.Class);
        register(40, true, false, EntityTrackerUpdateS2CPacket.Class);
        register(50, true, false, ChunkStatusUpdateS2CPacket.Class);
        register(51, true, false, ChunkDataS2CPacket.Class);
        register(52, true, false, ChunkDeltaUpdateS2CPacket.Class);
        register(53, true, false, BlockUpdateS2CPacket.Class);
        register(54, true, false, PlayNoteSoundS2CPacket.Class);
        register(60, true, false, ExplosionS2CPacket.Class);
        register(61, true, false, WorldEventS2CPacket.Class);
        register(70, true, false, GameStateChangeS2CPacket.Class);
        register(71, true, false, GlobalEntitySpawnS2CPacket.Class);
        register(100, true, false, OpenScreenS2CPacket.Class);
        register(101, true, true, CloseScreenS2CPacket.Class);
        register(102, false, true, ClickSlotC2SPacket.Class);
        register(103, true, false, ScreenHandlerSlotUpdateS2CPacket.Class);
        register(104, true, false, InventoryS2CPacket.Class);
        register(105, true, false, ScreenHandlerPropertyUpdateS2CPacket.Class);
        register(106, true, true, ScreenHandlerAcknowledgementPacket.Class);
        register(130, true, true, UpdateSignPacket.Class);
        register(131, true, false, MapUpdateS2CPacket.Class);
        register(200, true, false, IncreaseStatS2CPacket.Class);
        register(255, true, true, DisconnectPacket.Class);
        trackers = new HashMap();
        incomingCount = 0;
    }
}
