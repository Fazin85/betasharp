using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class GameStateChangeS2CPacket : Packet
{
    public static readonly string[] REASONS = ["tile.bed.notValid", null, null];
    public int reason;

    public GameStateChangeS2CPacket()
    {
    }

    public GameStateChangeS2CPacket(int reason)
    {
        this.reason = reason;
    }

    public override void Read(DataInputStream stream)
    {
        reason = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(reason);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onGameStateChange(this);
    }

    public override int Size()
    {
        return 1;
    }
}