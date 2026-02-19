using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionFile : IDisposable
{
    public enum CompressionType : byte
    {
        GZipUnused = 1,
        ZLibDeflate = 2,
        OldRegionUnused = 3
    }

    private static readonly byte[] EmptySector = new byte[4096];
    private readonly string _fileName;
    private readonly FileStream _dataFile;
    private readonly int[] _offsets = new int[1024];
    private readonly int[] _chunkSaveTimes = new int[1024];
    private readonly List<bool> _sectorFree;
    private int _sizeDelta;

    public RegionFile(string path)
    {
        _fileName = path;
        _sizeDelta = 0;

        try
        {
            _dataFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (_dataFile.Length < 4096)
            {
                for (int i = 0; i < 2048; i++) WriteBigEndianInt(0); // Offsets + Timestamps
                _sizeDelta += 8192;
            }
            if ((_dataFile.Length & 4095) != 0)
            {
                _dataFile.Position = _dataFile.Length;
                int paddingNeeded = (int)(4096 - (_dataFile.Length & 4095));
                _dataFile.Write(new byte[paddingNeeded], 0, paddingNeeded);
            }

            int sectorCount = (int)_dataFile.Length / 4096;
            _sectorFree = new List<bool>(sectorCount);
            for (int i = 0; i < sectorCount; i++) _sectorFree.Add(true);

            _sectorFree[0] = false;
            _sectorFree[1] = false; 

            _dataFile.Position = 0;
            for (int i = 0; i < 1024; i++)
            {
                int offset = ReadBigEndianInt();
                _offsets[i] = offset;
                if (offset != 0 && (offset >> 8) + (offset & 255) <= _sectorFree.Count)
                {
                    for (int sector = 0; sector < (offset & 255); sector++)
                    {
                        _sectorFree[(offset >> 8) + sector] = false;
                    }
                }
            }

            for (int i = 0; i < 1024; i++)
            {
                _chunkSaveTimes[i] = ReadBigEndianInt();
            }
        }
        catch (IOException ex)
        {
            Log.Error($"Failed to load RegionFile {path}: {ex.Message}");
        }
    }

    public int GetSizeDelta()
    {
        lock (this)
        {
            int delta = _sizeDelta;
            _sizeDelta = 0;
            return delta;
        }
    }

    public ChunkDataStream GetChunkDataInputStream(int x, int z)
    {
        lock (this)
        {
            if (IsOutOfBounds(x, z)) return null;

            int offset = GetOffset(x, z);
            if (offset == 0) return null;

            int sectorOffset = offset >> 8;
            int sectorCount = offset & 255;

            if (sectorOffset + sectorCount > _sectorFree.Count) return null;

            _dataFile.Position = sectorOffset * 4096;
            int length = ReadBigEndianInt();

            if (length > 4096 * sectorCount) return null;

            CompressionType type = (CompressionType)_dataFile.ReadByte();
            if (type == CompressionType.ZLibDeflate)
            {
                byte[] data = new byte[length - 1];
                _dataFile.Read(data, 0, data.Length);
                var ms = new MemoryStream(data);
                var zlib = new ZLibStream(ms, CompressionMode.Decompress);
                return new ChunkDataStream(zlib, type);
            }
            return null;
        }
    }

    public Stream GetChunkDataOutputStream(int x, int z)
    {
        if (IsOutOfBounds(x, z)) return null;
        var buffer = new RegionFileChunkBuffer(this, x, z);
        return new ZLibStream(buffer, CompressionMode.Compress);
    }

    public void Write(int x, int z, byte[] data, int length)
    {
        lock (this)
        {
            try
            {
                int offset = GetOffset(x, z);
                int sectorOffset = offset >> 8;
                int sectorsOccupied = offset & 255;
                int sectorsNeeded = (length + 5) / 4096 + 1;

                if (sectorsNeeded >= 256) return;

                if (sectorOffset != 0 && sectorsOccupied == sectorsNeeded)
                {
                    WriteInternal(sectorOffset, data, length);
                }
                else
                {
                    for (int i = 0; i < sectorsOccupied; i++)
                    {
                        _sectorFree[sectorOffset + i] = true;
                    }

                    int firstFree = _sectorFree.IndexOf(true);
                    int runStart = firstFree;
                    int runLength = 0;

                    if (runStart != -1)
                    {
                        for (int i = runStart; i < _sectorFree.Count; i++)
                        {
                            if (_sectorFree[i]) runLength++;
                            else { runLength = 0; runStart = i + 1; }

                            if (runLength >= sectorsNeeded) break;
                        }
                    }

                    if (runLength >= sectorsNeeded)
                    {
                        sectorOffset = runStart;
                        SetOffset(x, z, (sectorOffset << 8) | sectorsNeeded);
                        for (int i = 0; i < sectorsNeeded; i++) _sectorFree[sectorOffset + i] = false;
                        WriteInternal(sectorOffset, data, length);
                    }
                    else
                    {
                        _dataFile.Position = _dataFile.Length;
                        sectorOffset = _sectorFree.Count;
                        for (int i = 0; i < sectorsNeeded; i++)
                        {
                            _dataFile.Write(EmptySector, 0, 4096);
                            _sectorFree.Add(false);
                        }
                        _sizeDelta += 4096 * sectorsNeeded;
                        WriteInternal(sectorOffset, data, length);
                        SetOffset(x, z, (sectorOffset << 8) | sectorsNeeded);
                    }
                }
                SetTimestamp(x, z, (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }
    }

    private void WriteInternal(int sectorOffset, byte[] data, int length)
    {
        _dataFile.Position = sectorOffset * 4096;
        WriteBigEndianInt(length + 1);
        _dataFile.WriteByte((byte)CompressionType.ZLibDeflate);
        _dataFile.Write(data, 0, length);
    }

    private void WriteBigEndianInt(int val)
    {
        byte[] bytes = BitConverter.GetBytes(val);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        _dataFile.Write(bytes, 0, 4);
    }

    private int ReadBigEndianInt()
    {
        byte[] bytes = new byte[4];
        _dataFile.Read(bytes, 0, 4);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    private bool IsOutOfBounds(int x, int z) => x < 0 || x >= 32 || z < 0 || z >= 32;

    private int GetOffset(int x, int z) => _offsets[x + z * 32];

    private void SetOffset(int x, int z, int offset)
    {
        _offsets[x + z * 32] = offset;
        _dataFile.Position = (x + z * 32) * 4;
        WriteBigEndianInt(offset);
    }

    private void SetTimestamp(int x, int z, int time)
    {
        _chunkSaveTimes[x + z * 32] = time;
        _dataFile.Position = 4096 + (x + z * 32) * 4;
        WriteBigEndianInt(time);
    }

    public void Close() => _dataFile.Close();

    public void Dispose()
    {
        _dataFile?.Dispose();
        GC.SuppressFinalize(this);
    }
}
