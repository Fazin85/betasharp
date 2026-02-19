using BetaSharp.Entities;
using BetaSharp.Network;
using BetaSharp.Network.Packets;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Server.Internal;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.lang;
using java.net;
using java.util.logging;

namespace BetaSharp.Server.Network;

public class ServerLoginNetworkHandler : NetHandler
{
    private static JavaRandom random = new();
    public Connection connection;
    public bool closed;
    private MinecraftServer server;
    private int loginTicks;
    private string username;
    private LoginHelloPacket loginPacket;
    private string serverId = "";

    public ServerLoginNetworkHandler(MinecraftServer server, Socket socket, string name)
    {
        this.server = server;
        connection = new Connection(socket, name, this);
        connection.lag = 0;
    }

    public ServerLoginNetworkHandler(MinecraftServer server, Connection connection)
    {
        this.server = server;
        this.connection = connection;
        connection.setNetworkHandler(this);
        connection.lag = 0;
    }

    public void tick()
    {
        if (loginPacket != null)
        {
            accept(loginPacket);
            loginPacket = null;
        }

        if (loginTicks++ == 600)
        {
            disconnect("Took too long to log in");
        }
        else
        {
            connection.tick();
        }
    }

    public void disconnect(string reason)
    {
        try
        {
            Log.Info($"Disconnecting {getConnectionInfo()}: {reason}");
            connection.sendPacket(new DisconnectPacket(reason));
            connection.disconnect();
            closed = true;
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
    }

    public override void onHandshake(HandshakePacket packet)
    {
        if (server.onlineMode)
        {
            serverId = Long.toHexString(random.NextLong());
            connection.sendPacket(new HandshakePacket(serverId));
        }
        else
        {
            connection.sendPacket(new HandshakePacket("-"));
        }
    }

    public override void onHello(LoginHelloPacket packet)
    {
        if (server is InternalServer)
        {
            packet.username = "player";
        }
        if (packet.worldSeed == LoginHelloPacket.BETASHARP_CLIENT_SIGNATURE)
        {
            // This is a BetaSharp client. We can use this for future protocol extensions.
        }

        username = packet.username;
        if (packet.protocolVersion != 14)
        {
            if (packet.protocolVersion > 14)
            {
                disconnect("Outdated server!");
            }
            else
            {
                disconnect("Outdated client!");
            }
        }
        else
        {
            if (!server.onlineMode)
            {
                accept(packet);
            }
            else
            {
                //TODO: ADD SOME KIND OF AUTH
                //new C_15575233(this, packet).start();
                throw new IllegalStateException("Auth not supported");
            }
        }
    }

    public void accept(LoginHelloPacket packet)
    {
        ServerPlayerEntity ent = server.playerManager.connectPlayer(this, packet.username);
        if (ent != null)
        {
            server.playerManager.loadPlayerData(ent);
            ent.setWorld(server.getWorld(ent.dimensionId));
            Log.Info($"{getConnectionInfo()} logged in with entity id {ent.id} at ({ent.x}, {ent.y}, {ent.z})");
            ServerWorld var3 = server.getWorld(ent.dimensionId);
            Vec3i var4 = var3.getSpawnPos();
            ServerPlayNetworkHandler handler = new ServerPlayNetworkHandler(server, connection, ent);
            handler.sendPacket(new LoginHelloPacket("", ent.id, var3.getSeed(), (sbyte)var3.dimension.id));
            handler.sendPacket(new PlayerSpawnPositionS2CPacket(var4.x, var4.y, var4.z));
            server.playerManager.sendWorldInfo(ent, var3);
            server.playerManager.sendToAll(new ChatMessagePacket("§e" + ent.name + " joined the game."));
            server.playerManager.addPlayer(ent);
            handler.teleport(ent.x, ent.y, ent.z, ent.yaw, ent.pitch);
            server.connections.AddConnection(handler);
            handler.sendPacket(new WorldTimeUpdateS2CPacket(var3.getTime()));
            ent.initScreenHandler();
        }

        closed = true;
    }

    public override void onDisconnected(string reason, object[]? objects)
    {
        Log.Info($"{getConnectionInfo()} lost connection");
        closed = true;
    }

    public override void handle(Packet packet)
    {
        disconnect("Protocol error");
    }

    public string getConnectionInfo()
    {
        if (connection.getAddress() == null) return "Internal";
        return username != null ? username + " [" + connection.getAddress()!.toString() + "]" : connection.getAddress()!.toString();
    }

    public override bool isServerSide()
    {
        return true;
    }
}
