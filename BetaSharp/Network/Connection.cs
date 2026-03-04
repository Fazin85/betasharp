using System.Net;
using System.Net.Sockets;
using BetaSharp.Network.Packets;
using BetaSharp.Threading;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class Connection
{
    private readonly ILogger<Connection> _logger = Log.Instance.For<Connection>();
    protected object lck = new();
    private Socket? _socket;
    private readonly IPEndPoint? _address;
    protected readonly List<Packet> readQueue = [];
    protected readonly List<Packet> sendQueue = [];
    protected readonly List<Packet> delayedSendQueue = [];
    protected NetHandler? networkHandler;
    protected bool closed;
    protected bool disconnected;
    protected string disconnectedReason = "";
    protected object[]? disconnectReasonArgs;
    protected int timeout;
    protected int sendQueueSize;
    public static readonly int[] TOTAL_READ_SIZE = new int[256];
    public static readonly int[] TOTAL_SEND_SIZE = new int[256];
    public int lag = 0;
    private int _delay = 0;
    protected readonly ManualResetEventSlim wakeSignal = new(false);
    private NetworkStream? _networkStream;

    public Connection(Socket socket, string address, NetHandler networkHandler)
    {
        _socket = socket;
        _address = (IPEndPoint?)socket.RemoteEndPoint;
        this.networkHandler = networkHandler;

        socket.ReceiveTimeout = 30000;
        // setTrafficClass doesn't have a direct .NET equivalent and can be omitted

        _networkStream = new NetworkStream(socket);

        Task.Factory.StartNew(
            () =>
            {
                while (!closed)
                {
                    if (!read())
                    {
                        break;
                    }
                }
            },
            TaskCreationOptions.LongRunning);

        Task.Factory.StartNew(
            () =>
            {
                try
                {
                    while (!closed)
                    {
                        while (write())
                        {
                        }

                        try
                        {
                            _networkStream?.Flush();
                        }
                        catch (IOException ex)
                        {
                            if (!disconnected)
                            {
                                disconnect(ex);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            },
            TaskCreationOptions.LongRunning);
    }

    protected Connection()
    {
        _address = null;
    }

    public void setNetworkHandler(NetHandler netHandler)
    {
        networkHandler = netHandler;
    }

    public virtual void sendPacket(Packet packet)
    {
        if (!closed)
        {
            object lockObj = lck;
            lock (lockObj)
            {
                sendQueueSize += packet.Size() + 1;
                if (Packet.Registry[packet.Id]!.WorldPacket)
                {
                    delayedSendQueue.Add(packet);
                }
                else
                {
                    sendQueue.Add(packet);
                }
            }
        }
    }

    protected virtual bool write()
    {
        if (_networkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool wrotePacket = false;

        try
        {
            int[] sizeStats;
            Packet packet;
            object lockObj;
            if (sendQueue.Count > 0 && (lag == 0 || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    - sendQueue[0].CreationTime >= lag))
            {
                lockObj = lck;
                lock (lockObj)
                {
                    packet = sendQueue[0];
                    sendQueue.RemoveAt(0);
                    sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, _networkStream);
                sizeStats = TOTAL_SEND_SIZE;
                sizeStats[packet.Id] += packet.Size() + 1;
                wrotePacket = true;
            }

            if (_delay-- <= 0 && delayedSendQueue.Count > 0 && (lag == 0 || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    - delayedSendQueue[0].CreationTime >= lag))
            {
                lockObj = lck;
                lock (lockObj)
                {
                    packet = delayedSendQueue[0];
                    delayedSendQueue.RemoveAt(0);
                    sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, _networkStream);
                sizeStats = TOTAL_SEND_SIZE;
                sizeStats[packet.Id] += packet.Size() + 1;
                _delay = 0;
                wrotePacket = true;
            }

            return wrotePacket;
        }
        catch (Exception ex)
        {
            if (!disconnected)
            {
                disconnect(ex);
            }

            return false;
        }
    }

    public virtual void interrupt()
    {
        // closed = true;
    }

    protected virtual bool read()
    {
        if (networkHandler == null || _networkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool receivedPacket = false;

        try
        {
            Packet? packet = Packet.Read(_networkStream, networkHandler.isServerSide());
            if (packet != null)
            {
                int[] sizeStats = TOTAL_READ_SIZE;
                int packetId = packet.Id;
                sizeStats[packetId] += packet.Size() + 1;
                lock (lck)
                {
                    readQueue.Add(packet);
                }

                receivedPacket = true;
            }
            else
            {
                disconnect("disconnect.endOfStream");
            }

            return receivedPacket;
        }
        catch (Exception ex)
        {
            if (!disconnected)
            {
                disconnect(ex);
            }

            return false;
        }
    }

    private void disconnect(Exception e)
    {
        _logger.LogError(e, e.Message);
        disconnect("disconnect.genericReason", "Internal exception: " + e);
    }

    public virtual void disconnect(string disconnectedReason, params object[] disconnectReasonArgs)
    {
        if (!closed)
        {
            disconnected = true;
            this.disconnectedReason = disconnectedReason;
            this.disconnectReasonArgs = disconnectReasonArgs;

            closed = true;

            try
            {
                _networkStream?.Close();
                _networkStream = null;

                _socket?.Close();
                _socket = null;
            }
            catch (Exception)
            {
                // Ignore.
            }
        }
    }

    public virtual void tick()
    {
        if (sendQueueSize > 1048576)
        {
            disconnect("disconnect.overflow");
        }

        if (readQueue.Count == 0)
        {
            if (timeout++ == 1200)
            {
                disconnect("disconnect.timeout");
            }
        }
        else
        {
            timeout = 0;
        }

        processPackets();

        interrupt();
        if (disconnected && readQueue.Count == 0)
        {
            networkHandler?.onDisconnected(disconnectedReason, disconnectReasonArgs);
        }
    }

    protected virtual void processPackets()
    {
        if (networkHandler == null)
        {
            throw new Exception("networkHandler is null");
        }

        int maxPacketsPerTick = 100;

        while (readQueue.Count > 0 && maxPacketsPerTick-- >= 0)
        {
            Packet packet;
            lock (lck)
            {
                if (readQueue.Count == 0) break;
                packet = readQueue[0];
                readQueue.RemoveAt(0);
            }

            packet.Apply(networkHandler);
        }
    }

    public virtual IPEndPoint? getAddress()
    {
        return _address;
    }

    public virtual void disconnect()
    {
        interrupt();
        closed = true;

        _ = Task.Run(async () =>
        {
            // await Task.Delay(2000);

            try
            {
                if (!closed)
                {
                    disconnect(new Exception("disconnect.closed"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing connection");
            }
        });
    }

    public int getDelayedSendQueueSize()
    {
        return delayedSendQueue.Count;
    }
}
