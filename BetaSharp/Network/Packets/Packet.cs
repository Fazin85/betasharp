using BetaSharp.Network.Packets.C2SPlay;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using java.lang;
using java.util;

namespace BetaSharp.Network.Packets;

public abstract class Packet
{
    private static Map IO_TO_TYPE = new HashMap();
    private static Map TYPE_TO_ID = new HashMap();
    private static Set S2C = new HashSet();
    private static Set C2S = new HashSet();
    public readonly long creationTime = java.lang.System.currentTimeMillis();
    public bool worldPacket = false;
    private static HashMap trackers;
    private static int incomingCount;

    static void register(int rawId, bool clientBound, bool serverBound, Type type)
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
        return ((Integer)TYPE_TO_ID.get(GetType())).intValue();
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
        register(0, true, true, typeof(KeepAlivePacket));
        register(1, true, true, typeof(LoginHelloPacket));
        register(2, true, true, typeof(HandshakePacket));
        register(3, true, true, typeof(ChatMessagePacket));
        register(4, true, false, typeof(WorldTimeUpdateS2CPacket));
        register(5, true, false, typeof(EntityEquipmentUpdateS2CPacket));
        register(6, true, false, typeof(PlayerSpawnPositionS2CPacket));
        register(7, false, true, typeof(PlayerInteractEntityC2SPacket));
        register(8, true, false, typeof(HealthUpdateS2CPacket));
        register(9, true, true, typeof(PlayerRespawnPacket));
        register(10, true, true, typeof(PlayerMovePacket));
        register(11, true, true, typeof(PlayerMovePositionAndOnGroundPacket));
        register(12, true, true, typeof(PlayerMoveLookAndOnGroundPacket));
        register(13, true, true, typeof(PlayerMoveFullPacket));
        register(14, false, true, typeof(PlayerActionC2SPacket));
        register(15, false, true, typeof(PlayerInteractBlockC2SPacket));
        register(16, false, true, typeof(UpdateSelectedSlotC2SPacket));
        register(17, true, false, typeof(PlayerSleepUpdateS2CPacket));
        register(18, true, true, typeof(EntityAnimationPacket));
        register(19, false, true, typeof(ClientCommandC2SPacket));
        register(20, true, false, typeof(PlayerSpawnS2CPacket));
        register(21, true, false, typeof(ItemEntitySpawnS2CPacket));
        register(22, true, false, typeof(ItemPickupAnimationS2CPacket));
        register(23, true, false, typeof(EntitySpawnS2CPacket));
        register(24, true, false, typeof(LivingEntitySpawnS2CPacket));
        register(25, true, false, typeof(PaintingEntitySpawnS2CPacket));
        register(27, false, true, typeof(PlayerInputC2SPacket));
        register(28, true, false, typeof(EntityVelocityUpdateS2CPacket));
        register(29, true, false, typeof(EntityDestroyS2CPacket));
        register(30, true, false, typeof(EntityS2CPacket));
        register(31, true, false, typeof(EntityMoveRelativeS2CPacket));
        register(32, true, false, typeof(EntityRotateS2CPacket));
        register(33, true, false, typeof(EntityRotateAndMoveRelativeS2CPacket));
        register(34, true, false, typeof(EntityPositionS2CPacket));
        register(38, true, false, typeof(EntityStatusS2CPacket));
        register(39, true, false, typeof(EntityVehicleSetS2CPacket));
        register(40, true, false, typeof(EntityTrackerUpdateS2CPacket));
        register(50, true, false, typeof(ChunkStatusUpdateS2CPacket));
        register(51, true, false, typeof(ChunkDataS2CPacket));
        register(52, true, false, typeof(ChunkDeltaUpdateS2CPacket));
        register(53, true, false, typeof(BlockUpdateS2CPacket));
        register(54, true, false, typeof(PlayNoteSoundS2CPacket));
        register(60, true, false, typeof(ExplosionS2CPacket));
        register(61, true, false, typeof(WorldEventS2CPacket));
        register(70, true, false, typeof(GameStateChangeS2CPacket));
        register(71, true, false, typeof(GlobalEntitySpawnS2CPacket));
        register(100, true, false, typeof(OpenScreenS2CPacket));
        register(101, true, true, typeof(CloseScreenS2CPacket));
        register(102, false, true, typeof(ClickSlotC2SPacket));
        register(103, true, false, typeof(ScreenHandlerSlotUpdateS2CPacket));
        register(104, true, false, typeof(InventoryS2CPacket));
        register(105, true, false, typeof(ScreenHandlerPropertyUpdateS2CPacket));
        register(106, true, true, typeof(ScreenHandlerAcknowledgementPacket));
        register(130, true, true, typeof(UpdateSignPacket));
        register(131, true, false, typeof(MapUpdateS2CPacket));
        register(200, true, false, typeof(IncreaseStatS2CPacket));
        register(255, true, true, typeof(DisconnectPacket));
        trackers = new HashMap();
        incomingCount = 0;
    }
}
