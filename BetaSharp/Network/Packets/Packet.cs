using BetaSharp.Network.Packets.C2SPlay;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using java.lang;
using Microsoft.Extensions.Logging;
using StringBuilder = System.Text.StringBuilder;

namespace BetaSharp.Network.Packets;

public abstract class Packet
{
    private static readonly Dictionary<int, PacketRegisterItem> s_ioToType = new ();
    private static readonly Dictionary<Type, PacketRegisterRout> s_typeToId = new ();
    private static readonly ILogger<Packet> s_logger = Log.Instance.For<Packet>();

    private static readonly Dictionary<int, PacketTracker> s_trackers = new ();

    public readonly long CreationTime = java.lang.System.currentTimeMillis();
    public bool WorldPacket = false;
    private PacketRegisterRout? _rout = null;

    private static void Register<T>(PacketRegisterItem<T> item) where T : Packet
    {
        if (s_ioToType.ContainsKey(item.Id))
        {
            throw new ArgumentException("Duplicate packet id:" + item.Id, nameof(item));
        }
        if (s_typeToId.ContainsKey(typeof(T)))
        {
            throw new ArgumentException("Duplicate packet class:" + typeof(T));
        }

        s_ioToType.Add(item.Id, item);
        s_typeToId.Add(typeof(T), item);
    }

    public static Packet? Create(int rawId)
    {
        if (s_ioToType.TryGetValue(rawId, out PacketRegisterItem? item))
        {
            return item.NewPacket();
        }

        s_logger.LogInformation($"Skipping packet with id {rawId}");
        return null;
    }

    public int GetRawId()
    {
        if (!s_typeToId.TryGetValue(GetType(), out _rout))
        {
            s_logger.LogError($"Id not found for packet type {GetType()}");
            return -1;
        }
        return _rout.Id;
    }

    public static Packet? Read(java.io.DataInputStream stream, bool server)
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

            if (!s_ioToType.TryGetValue(rawId, out PacketRegisterItem? packetR))
            {
                throw new IOException("Bad packet id " + rawId);
            }

            if (server)
            {
                if (!packetR.ServerBound) throw new IOException("Bad server bound packet id " + rawId);
            }
            else
            {
                if (!packetR.ClientBound) throw new IOException("Bad client bound packet id " + rawId);
            }

            packet = packetR.NewPacket();

            packet.Read(stream);
        }
        catch (IOException e)
        {
            s_logger.LogInformation("Reached end of stream : " + e.Message);
            return null;
        }

        if (!s_trackers.TryGetValue(rawId, out PacketTracker? tracker))
        {
            tracker = new PacketTracker();
            s_trackers.Add(rawId, tracker);
        }

        tracker.update(packet.Size());

        return packet;
    }

    public static void Write(Packet packet, java.io.DataOutputStream stream)
    {
        stream.write(packet.GetRawId());
        packet.Write(stream);
    }

    public static void WriteString(string packetData, java.io.DataOutputStream stream)
    {
        if (packetData.Length > Short.MAX_VALUE)
        {
            throw new IOException("String too big");
        }

        stream.writeShort(packetData.Length);
        stream.writeChars(packetData);
    }

    public static string ReadString(java.io.DataInputStream stream, int maxLength)
    {

        short length = stream.readShort();
        if (length > maxLength)
        {
            throw new IOException("Received string length longer than maximum allowed (" + length + " > " + maxLength + ")");
        }
        if (length < 0)
        {
            throw new IOException("Received string length is less than zero! Weird string!");
        }

        var sb = new StringBuilder();

        for (int i = 0; i < length; ++i)
        {
            sb.Append(stream.readChar());
        }

        return sb.ToString();
    }

    public abstract void Read(java.io.DataInputStream stream);

    public abstract void Write(java.io.DataOutputStream stream);

    public abstract void Apply(NetHandler handler);

    public abstract int Size();

    public virtual void ProcessForInternal()
    {
    }

    static Packet()
    {
        Register(New(0, true, true, () => new KeepAlivePacket()));
        Register(New(1, true, true, () => new LoginHelloPacket()));
        Register(New(2, true, true, () => new HandshakePacket()));
        Register(New(3, true, true, () => new ChatMessagePacket()));
        Register(New(4, true, false, () => new WorldTimeUpdateS2CPacket()));
        Register(New(5, true, false, () => new EntityEquipmentUpdateS2CPacket()));
        Register(New(6, true, false, () => new PlayerSpawnPositionS2CPacket()));
        Register(New(7, false, true, () => new PlayerInteractEntityC2SPacket()));
        Register(New(8, true, false, () => new HealthUpdateS2CPacket()));
        Register(New(9, true, true, () => new PlayerRespawnPacket()));
        Register(New(10, true, true, () => new PlayerMovePacket()));
        Register(New(11, true, true, () => new PlayerMovePositionAndOnGroundPacket()));
        Register(New(12, true, true, () => new PlayerMoveLookAndOnGroundPacket()));
        Register(New(13, true, true, () => new PlayerMoveFullPacket()));
        Register(New(14, false, true, () => new PlayerActionC2SPacket()));
        Register(New(15, false, true, () => new PlayerInteractBlockC2SPacket()));
        Register(New(16, false, true, () => new UpdateSelectedSlotC2SPacket()));
        Register(New(17, true, false, () => new PlayerSleepUpdateS2CPacket()));
        Register(New(18, true, true, () => new EntityAnimationPacket()));
        Register(New(19, false, true, () => new ClientCommandC2SPacket()));
        Register(New(20, true, false, () => new PlayerSpawnS2CPacket()));
        Register(New(21, true, false, () => new ItemEntitySpawnS2CPacket()));
        Register(New(22, true, false, () => new ItemPickupAnimationS2CPacket()));
        Register(New(23, true, false, () => new EntitySpawnS2CPacket()));
        Register(New(24, true, false, () => new LivingEntitySpawnS2CPacket()));
        Register(New(25, true, false, () => new PaintingEntitySpawnS2CPacket()));
        Register(New(27, false, true, () => new PlayerInputC2SPacket()));
        Register(New(28, true, false, () => new EntityVelocityUpdateS2CPacket()));
        Register(New(29, true, false, () => new EntityDestroyS2CPacket()));
        Register(New(30, true, false, () => new EntityS2CPacket()));
        Register(New(31, true, false, () => new EntityMoveRelativeS2CPacket()));
        Register(New(32, true, false, () => new EntityRotateS2CPacket()));
        Register(New(33, true, false, () => new EntityRotateAndMoveRelativeS2CPacket()));
        Register(New(34, true, false, () => new EntityPositionS2CPacket()));
        Register(New(38, true, false, () => new EntityStatusS2CPacket()));
        Register(New(39, true, false, () => new EntityVehicleSetS2CPacket()));
        Register(New(40, true, false, () => new EntityTrackerUpdateS2CPacket()));
        Register(New(50, true, false, () => new ChunkStatusUpdateS2CPacket()));
        Register(New(51, true, false, () => new ChunkDataS2CPacket()));
        Register(New(52, true, false, () => new ChunkDeltaUpdateS2CPacket()));
        Register(New(53, true, false, () => new BlockUpdateS2CPacket()));
        Register(New(54, true, false, () => new PlayNoteSoundS2CPacket()));
        Register(New(60, true, false, () => new ExplosionS2CPacket()));
        Register(New(61, true, false, () => new WorldEventS2CPacket()));
        Register(New(70, true, false, () => new GameStateChangeS2CPacket()));
        Register(New(71, true, false, () => new GlobalEntitySpawnS2CPacket()));
        Register(New(100, true, false, () => new OpenScreenS2CPacket()));
        Register(New(101, true, true, () => new CloseScreenS2CPacket()));
        Register(New(102, false, true, () => new ClickSlotC2SPacket()));
        Register(New(103, true, false, () => new ScreenHandlerSlotUpdateS2CPacket()));
        Register(New(104, true, false, () => new InventoryS2CPacket()));
        Register(New(105, true, false, () => new ScreenHandlerPropertyUpdateS2CPacket()));
        Register(New(106, true, true, () => new ScreenHandlerAcknowledgementPacket()));
        Register(New(130, true, true, () => new UpdateSignPacket()));
        Register(New(131, true, false, () => new MapUpdateS2CPacket()));
        Register(New(200, true, false, () => new IncreaseStatS2CPacket()));
        Register(New(255, true, true, () => new DisconnectPacket()));
    }

    private abstract class PacketRegisterRout(int rawId, bool clientBound, bool serverBound)
    {
        public readonly int Id = rawId;
        public readonly bool ClientBound = clientBound;
        public readonly bool ServerBound = serverBound;
    }

    private abstract class PacketRegisterItem(int rawId, bool clientBound, bool serverBound) : PacketRegisterRout(rawId, clientBound, serverBound)
    {
        public abstract Packet NewPacket();
    }

    private class PacketRegisterItem<T>(int rawId, bool clientBound, bool serverBound, Func<T> factory) : PacketRegisterItem(rawId, clientBound, serverBound)
        where T : Packet
    {
        public override Packet NewPacket()
        {
            Packet p = factory();
            p._rout = this;
            return p;
        }
    }

    private static PacketRegisterItem<T> New<T>(int rawId, bool clientBound, bool serverBound, Func<T> factory) where T : Packet =>
        new(rawId, clientBound, serverBound, factory);
}
