using BetaSharp.Network.Packets;
using BetaSharp.Threading;
using java.io;
using java.net;
using java.util;

namespace BetaSharp.Network;

public class Connection
{
    public static readonly object LOCK = new();
    public static int READ_THREAD_COUNTER;
    public static int WRITE_THREAD_COUNTER;
    protected object lck = new();
    private Socket? _socket;
    private readonly SocketAddress? _address;
    private DataInputStream? _inputStream;
    private DataOutputStream? _outputStream;
    protected bool open = true;
    protected List readQueue = Collections.synchronizedList(new ArrayList());
    protected List sendQueue = Collections.synchronizedList(new ArrayList());
    protected List delayedSendQueue = Collections.synchronizedList(new ArrayList());
    protected NetHandler? networkHandler;
    protected bool closed;
    private readonly java.lang.Thread _writer;
    private readonly java.lang.Thread _reader;
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

    public Connection(Socket socket, string address, NetHandler networkHandler)
    {
        _socket = socket;
        _address = socket.getRemoteSocketAddress();
        this.networkHandler = networkHandler;

        try
        {
            socket.setSoTimeout(30000);
            socket.setTrafficClass(24);
        }
        catch (SocketException ex)
        {
            java.lang.System.err.println(ex.getMessage());
        }

        _inputStream = new DataInputStream(socket.getInputStream());
        _outputStream = new DataOutputStream(new BufferedOutputStream(socket.getOutputStream(), 65536));
        _reader = new NetworkReaderThread(this, address + " read thread");
        _writer = new NetworkWriterThread(this, address + " write thread");
        _reader.start();
        _writer.start();
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
                sendQueueSize += packet.size() + 1;
                if (packet.worldPacket)
                {
                    delayedSendQueue.add(packet);
                }
                else
                {
                    sendQueue.add(packet);
                }

            }
        }
    }

    protected virtual bool write()
    {
        if (_outputStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool wrotePacket = false;

        try
        {
            int[] sizeStats;
            int packetId;
            Packet packet;
            object lockObj;
            if (!sendQueue.isEmpty() && (lag == 0 || java.lang.System.currentTimeMillis() - ((Packet)sendQueue.get(0)).creationTime >= lag))
            {
                lockObj = lck;
                lock (lockObj)
                {
                    packet = (Packet)sendQueue.remove(0);
                    sendQueueSize -= packet.size() + 1;
                }

                Packet.write(packet, _outputStream);
                sizeStats = TOTAL_SEND_SIZE;
                packetId = packet.getRawId();
                sizeStats[packetId] += packet.size() + 1;
                wrotePacket = true;
            }

            if (_delay-- <= 0 && !delayedSendQueue.isEmpty() && (lag == 0 || java.lang.System.currentTimeMillis() - ((Packet)delayedSendQueue.get(0)).creationTime >= lag))
            {
                lockObj = lck;
                lock (lockObj)
                {
                    packet = (Packet)delayedSendQueue.remove(0);
                    sendQueueSize -= packet.size() + 1;
                }

                Packet.write(packet, _outputStream);
                sizeStats = TOTAL_SEND_SIZE;
                packetId = packet.getRawId();
                sizeStats[packetId] += packet.size() + 1;
                _delay = 0;
                wrotePacket = true;
            }

            return wrotePacket;
        }
        catch (java.lang.Exception ex)
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
        wakeSignal.Set();
    }

    public void waitForSignal(int timeoutMs)
    {
        wakeSignal.Wait(timeoutMs);
        wakeSignal.Reset();
    }

    protected virtual bool read()
    {
        if (networkHandler == null || _inputStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool receivedPacket = false;

        try
        {
            Packet packet = Packet.read(_inputStream, networkHandler.isServerSide());
            if (packet != null)
            {
                int[] sizeStats = TOTAL_READ_SIZE;
                int packetId = packet.getRawId();
                sizeStats[packetId] += packet.size() + 1;
                readQueue.add(packet);
                receivedPacket = true;
            }
            else
            {
                disconnect("disconnect.endOfStream");
            }

            return receivedPacket;
        }
        catch (java.lang.Exception ex)
        {
            if (!disconnected)
            {
                disconnect(ex);
            }

            return false;
        }
    }

    private void disconnect(java.lang.Exception ex)
    {
        ex.printStackTrace();
        disconnect("disconnect.genericReason", "Internal exception: " + ex.toString());
    }

    public virtual void disconnect(string disconnectedReason, params object[] disconnectReasonArgs)
    {
        if (open)
        {
            disconnected = true;
            this.disconnectedReason = disconnectedReason;
            this.disconnectReasonArgs = disconnectReasonArgs;
            new NetworkMasterThread(this).start();
            open = false;

            try
            {
                _inputStream?.close();
                _inputStream = null;
            }
            catch (java.lang.Throwable)
            {
            }

            try
            {
                _outputStream?.close();
                _outputStream = null;
            }
            catch (java.lang.Throwable)
            {
            }

            try
            {
                _socket?.close();
                _socket = null;
            }
            catch (java.lang.Throwable)
            {
            }

        }
    }

    public virtual void tick()
    {
        if (sendQueueSize > 1048576)
        {
            disconnect("disconnect.overflow");
        }

        if (readQueue.isEmpty())
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
        if (disconnected && readQueue.isEmpty())
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

        while (!readQueue.isEmpty() && maxPacketsPerTick-- >= 0)
        {
            Packet packet = (Packet)readQueue.remove(0);
            packet.apply(networkHandler);
        }
    }

    public virtual SocketAddress? getAddress()
    {
        return _address;
    }

    public virtual void disconnect()
    {
        interrupt();
        closed = true;
        new ThreadCloseConnection(this).start();
    }

    public int getDelayedSendQueueSize()
    {
        return delayedSendQueue.size();
    }

    public static bool isOpen(Connection conn)
    {
        return conn.open;
    }

    public static bool isClosed(Connection conn)
    {
        return conn.closed;
    }

    public static bool readPacket(Connection conn)
    {
        return conn.read();
    }

    public static bool writePacket(Connection conn)
    {
        return conn.write();
    }

    public static DataOutputStream getOutputStream(Connection conn)
    {
        if (conn._outputStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        return conn._outputStream;
    }

    public static bool isDisconnected(Connection conn)
    {
        return conn.disconnected;
    }

    public static void disconnect(Connection conn, java.lang.Exception ex)
    {
        conn.disconnect(ex);
    }

    public static java.lang.Thread getReader(Connection conn)
    {
        return conn._reader;
    }

    public static java.lang.Thread getWriter(Connection conn)
    {
        return conn._writer;
    }
}
