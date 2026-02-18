using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class GameStateChangeS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(GameStateChangeS2CPacket).TypeHandle);

    public static readonly string[] REASONS = ["tile.bed.notValid", null, null];
    public int reason;

    public GameStateChangeS2CPacket()
    {
    }

    public GameStateChangeS2CPacket(int reason)
    {
        this.reason = reason;
    }

    public override void read(DataInputStream stream)
    {
        reason = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeByte(reason);
    }

    public override void apply(NetHandler handler)
    {
        handler.onGameStateChange(this);
    }

    public override int size()
    {
        return 1;
    }
}