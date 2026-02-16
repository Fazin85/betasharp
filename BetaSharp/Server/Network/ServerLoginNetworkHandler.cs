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
    public static Logger LOGGER = Logger.getLogger("Minecraft");
    private static java.util.Random random = new();
    public Connection connection;
    public bool closed = false;
    private MinecraftServer server;
    private int loginTicks = 0;
    private string username = null;
    private LoginHelloPacket loginPacket = null;
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
            LOGGER.info("Disconnecting " + getConnectionInfo() + ": " + reason);
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
            serverId = Long.toHexString(random.nextLong());
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
            packet.username = "Player_BetaSharp";
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
        ServerPlayerEntity entity = server.playerManager.connectPlayer(this, packet.username);
        if (entity  != null)
        {
            server.playerManager.loadPlayerData(entity);
            entity.setWorld(server.getWorld(entity.dimensionId));
            LOGGER.info(getConnectionInfo() + " logged in with entity id " + entity.id + " at (" + entity.x + ", " + entity.y + ", " + entity.z + ")");
            ServerWorld ServerWorld = server.getWorld(entity.dimensionId);
            Vec3i Spawn = ServerWorld.getSpawnPos();
            ServerPlayNetworkHandler handler = new ServerPlayNetworkHandler(server, connection, entity);
            handler.sendPacket(new LoginHelloPacket("", entity.id, ServerWorld.getSeed(), (sbyte)ServerWorld.Dimension.id));
            handler.sendPacket(new PlayerSpawnPositionS2CPacket(Spawn.x, Spawn.y, Spawn.z));
            server.playerManager.sendWorldInfo(entity, ServerWorld);
            server.playerManager.sendToAll(new ChatMessagePacket("§e" + entity.name + " joined the game."));
            server.playerManager.addPlayer(entity);
            handler.teleport(entity.x, entity.y, entity.z, entity.yaw, entity.pitch);
            server.connections.addConnection(handler);
            handler.sendPacket(new WorldTimeUpdateS2CPacket(ServerWorld.getTime()));
            entity.initScreenHandler();
        }

        closed = true;
    }

    public override void onDisconnected(string reason, object[] objects)
    {
        LOGGER.info(getConnectionInfo() + " lost connection");
        closed = true;
    }

    public override void handle(Packet packet)
    {
        disconnect("Protocol error");
    }

    public string getConnectionInfo()
    {
        if (connection.getAddress() == null) return "Internal";
        return username != null ? username + " [" + connection.getAddress().toString() + "]" : connection.getAddress().toString();
    }

    public override bool isServerSide()
    {
        return true;
    }
}