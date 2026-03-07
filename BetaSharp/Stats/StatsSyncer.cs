using Microsoft.Extensions.Logging;

namespace BetaSharp.Stats;

internal class StatsSynchronizer
{
    private static readonly ILogger<StatsSynchronizer> s_logger = Log.Instance.For<StatsSynchronizer>();

    private volatile bool _busy;
    private volatile Dictionary<StatBase, int> _mergedData;
    private volatile Dictionary<StatBase, int> _downloadedData;

    private readonly StatFileWriter _statFileWriter;
    private readonly Session _session;
    private int _syncTimeout;
    private int _timeoutCounter;

    public StatsSynchronizer(Session session, StatFileWriter statFileWriter, string statsFolder)
    {
        string usernameLower = session.username.ToLowerInvariant();

        UnsentStatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.dat");
        StatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}.dat");
        OldUnsentStatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.old");
        OldStatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}.old");
        TempUnsentStatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.tmp");
        TempStatsFile = Path.Combine(statsFolder, $"stats_{usernameLower}.tmp");

        if (usernameLower != session.username)
        {
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.dat", UnsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.dat", StatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.old", OldUnsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.old", OldStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.tmp", TempUnsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.tmp", TempStatsFile);
        }

        _statFileWriter = statFileWriter;
        _session = session;

        if (File.Exists(UnsentStatsFile))
        {
            statFileWriter.LoadStats(GetNewestAvailableStats(UnsentStatsFile, TempUnsentStatsFile, OldUnsentStatsFile));
        }

        ReceiveStats();
    }

    private static void EnsureStatFileIsLowercase(string statsFolder, string fileNameNotLowercase, string targetFile)
    {
        string otherFile = Path.Combine(statsFolder, fileNameNotLowercase);
        if (File.Exists(otherFile) && !File.Exists(targetFile))
        {
            File.Move(otherFile, targetFile);
        }
    }

    private static Dictionary<StatBase, int> GetNewestAvailableStats(string unsent, string tempUnsent, string oldUnsent)
    {
        if (File.Exists(unsent)) return CreateStatsMapFromFile(unsent);
        if (File.Exists(oldUnsent)) return CreateStatsMapFromFile(oldUnsent);
        if (File.Exists(tempUnsent)) return CreateStatsMapFromFile(tempUnsent);
        return null;
    }

    private static Dictionary<StatBase, int> CreateStatsMapFromFile(string filePath)
    {
        try
        {
            string fileContents = File.ReadAllText(filePath);
            return StatFileWriter.CreateStatsMap(fileContents);
        }
        catch (Exception ex)
        {
            s_logger.LogError(ex, $"Exception reading stats from {filePath}");
        }

        return null;
    }

    internal void SaveStatsToFile(Dictionary<StatBase, int> statsMap, string unsentFile, string tempUnsentFile, string oldUnsentFile)
    {
        try
        {
            string jsonContent = StatFileWriter.SerializeStats(_session.username, "local", statsMap);
            File.WriteAllText(tempUnsentFile, jsonContent);

            if (File.Exists(oldUnsentFile))
            {
                File.Delete(oldUnsentFile);
            }

            if (File.Exists(unsentFile))
            {
                File.Move(unsentFile, oldUnsentFile);
            }

            File.Move(tempUnsentFile, unsentFile);
        }
        catch (Exception ex)
        {
            s_logger.LogError(ex, "Failed to save stats file.");
        }
    }

    public void ReceiveStats()
    {
        if (_busy)
        {
            throw new InvalidOperationException("Can't get stats from server while StatsSyncher is busy!");
        }

        _syncTimeout = 100;
        _busy = true;

        new Threading.ThreadStatSynchronizerReceive(this).Start();
    }

    public void SendStats(Dictionary<StatBase, int> statsMap)
    {
        if (_busy)
        {
            throw new InvalidOperationException("Can't save stats while StatsSyncher is busy!");
        }

        _syncTimeout = 100;
        _busy = true;

        new Threading.ThreadStatSynchronizerSend(this, statsMap).Start();
    }

    public void SyncStatsFileWithMap(Dictionary<StatBase, int> statsMap)
    {
        int waitCycles = 30;

        while (_busy)
        {
            --waitCycles;
            if (waitCycles <= 0) break;

            try
            {
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                s_logger.LogError(ex, "Interrupted while waiting for sync.");
            }
        }

        _busy = true;

        try
        {
            SaveStatsToFile(statsMap, UnsentStatsFile, TempUnsentStatsFile, OldUnsentStatsFile);
        }
        finally
        {
            _busy = false;
        }
    }

    public bool IsReadyToSync()
    {
        return _syncTimeout <= 0 && !_busy && _downloadedData == null;
    }

    public void Tick()
    {
        if (_syncTimeout > 0) --_syncTimeout;
        if (_timeoutCounter > 0) --_timeoutCounter;

        if (_downloadedData != null)
        {
            _statFileWriter.SetStats(_downloadedData);
            _downloadedData = null;
        }

        if (_mergedData != null)
        {
            _statFileWriter.AddStats(_mergedData);
            _mergedData = null;
        }
    }

    internal Dictionary<StatBase, int> MergedData
    {
        get => _mergedData;
        set => _mergedData = value;
    }

    internal bool Busy
    {
        get => _busy;
        set => _busy = value;
    }

    internal string StatsFile { get; }
    internal string TempStatsFile { get; }
    internal string OldStatsFile { get; }
    internal string UnsentStatsFile { get; }
    internal string TempUnsentStatsFile { get; }
    internal string OldUnsentStatsFile { get; }

    internal static Dictionary<StatBase, int> FetchNewestAvailableStats(string unsent, string tempUnsent, string oldUnsent)
    {
        return GetNewestAvailableStats(unsent, tempUnsent, oldUnsent);
    }
}
