using betareborn.Network.Packets;
using java.io;

namespace betareborn.Network.Packets.Play
{
    public class PlayerRespawnPacket : Packet
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerRespawnPacket).TypeHandle);

        public sbyte dimensionId;

        public PlayerRespawnPacket()
        {
        }

        public PlayerRespawnPacket(sbyte var1)
        {
            dimensionId = var1;
        }

        public override void apply(NetHandler var1)
        {
            var1.onPlayerRespawn(this);
        }

        public override void read(DataInputStream var1)
        {
            dimensionId = (sbyte)var1.readByte();
        }

        public override void write(DataOutputStream var1)
        {
            var1.writeByte(dimensionId);
        }

        public override int size()
        {
            return 1;
        }
    }
}