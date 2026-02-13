using java.io;

namespace BetaSharp.Network.Packets.Play;

public class ChatMessagePacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ChatMessagePacket).TypeHandle);

    public string chatMessage;

    public ChatMessagePacket()
    {
    }

    public ChatMessagePacket(string msg)
    {
        if (msg.Length > 119)
        {
            msg = msg.Substring(0, 119);
        }

        chatMessage = msg;
    }

    public override void read(DataInputStream stream)
    {
        chatMessage = readString(stream, 119);
    }

    public override void write(DataOutputStream stream)
    {
        writeString(chatMessage, stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onChatMessage(this);
    }

    public override int size()
    {
        return chatMessage.Length;
    }
}