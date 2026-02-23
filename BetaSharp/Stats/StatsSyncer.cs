using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BetaSharp.Util;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Stats;

public class StatsSyncer
{
    private static readonly ILogger<StatsSyncer> s_logger = Log.Instance.For<StatsSyncer>();

    private volatile bool _busy;
    private volatile Dictionary<StatBase, int> _mergedData;
    private volatile Dictionary<StatBase, int> _downloadedData;
    
    private readonly StatFileWriter _statFileWriter;
    private readonly Session _session;
    
    private readonly string _unsentStatsFile;
    private readonly string _statsFile;
    private readonly string _tempUnsentStatsFile;
    private readonly string _tempStatsFile;
    private readonly string _oldUnsentStatsFile;
    private readonly string _oldStatsFile;
    
    private int _syncTimeout;
    private int _timeoutCounter;

    public StatsSyncer(Session session, StatFileWriter statFileWriter, string statsFolder)
    {
        string usernameLower = session.username.ToLowerInvariant();
        
        _unsentStatsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.dat");
        _statsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}.dat");
        _oldUnsentStatsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.old");
        _oldStatsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}.old");
        _tempUnsentStatsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}_unsent.tmp");
        _tempStatsFile = System.IO.Path.Combine(statsFolder, $"stats_{usernameLower}.tmp");

        if (usernameLower != session.username)
        {
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.dat", _unsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.dat", _statsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.old", _oldUnsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.old", _oldStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}_unsent.tmp", _tempUnsentStatsFile);
            EnsureStatFileIsLowercase(statsFolder, $"stats_{session.username}.tmp", _tempStatsFile);
        }

        _statFileWriter = statFileWriter;
        _session = session;

        if (File.Exists(_unsentStatsFile))
        {
            statFileWriter.LoadStats(GetNewestAvailableStats(_unsentStatsFile, _tempUnsentStatsFile, _oldUnsentStatsFile));
        }

        ReceiveStats();
    }

    private void EnsureStatFileIsLowercase(string statsFolder, string fileNameNotLowercase, string targetFile)
    {
        string otherFile = System.IO.Path.Combine(statsFolder, fileNameNotLowercase);
        if (File.Exists(otherFile) && !File.Exists(targetFile))
        {
            File.Move(otherFile, targetFile);
        }
    }

    private Dictionary<StatBase, int> GetNewestAvailableStats(string unsent, string tempUnsent, string oldUnsent)
    {
        if (File.Exists(unsent)) return CreateStatsMapFromFile(unsent);
        if (File.Exists(oldUnsent)) return CreateStatsMapFromFile(oldUnsent);
        if (File.Exists(tempUnsent)) return CreateStatsMapFromFile(tempUnsent);
        return null;
    }

    private Dictionary<StatBase, int> CreateStatsMapFromFile(string filePath)
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
        
        new Threading.ThreadStatSyncerReceive(this).run();
    }

    public void SendStats(Dictionary<StatBase, int> statsMap)
    {
        if (_busy)
        {
            throw new InvalidOperationException("Can't save stats while StatsSyncher is busy!");
        }

        _syncTimeout = 100;
        _busy = true;

        // TODO:convert to Java Map
        // when Threading gets refactored, this can be removed
        var statsMapJava = new java.util.HashMap();
        foreach (var kvp in statsMap)
        {
            statsMapJava.put(kvp.Key, kvp.Value);
        }

        new Threading.ThreadStatSyncerSend(this, statsMapJava).run();
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
            SaveStatsToFile(statsMap, _unsentStatsFile, _tempUnsentStatsFile, _oldUnsentStatsFile);
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

    internal string StatsFile => _statsFile;
    internal string TempStatsFile => _tempStatsFile;
    internal string OldStatsFile => _oldStatsFile;
    internal string UnsentStatsFile => _unsentStatsFile;
    internal string TempUnsentStatsFile => _tempUnsentStatsFile;
    internal string OldUnsentStatsFile => _oldUnsentStatsFile;

    internal Dictionary<StatBase, int> FetchNewestAvailableStats(string unsent, string tempUnsent, string oldUnsent)
    {
        return GetNewestAvailableStats(unsent, tempUnsent, oldUnsent);
    }
}