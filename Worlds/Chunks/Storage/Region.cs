using betareborn.NBT;
using betareborn.Util.Maths;
using java.io;
using K4os.Compression.LZ4;
using Silk.NET.Maths;
using System.Buffers;

namespace betareborn.Worlds.Chunks.Storage
{
    public class Region
    {
        public class ChunkData(int uncompressedSize, byte[] compressedData, ModifiedFlag modified)
        {
            public int uncompressedSize { get; set; } = uncompressedSize;
            public byte[] compressedData { get; set; } = compressedData;
            public readonly ModifiedFlag modified = modified;
        }

        private class RegionSaveHandler(java.io.File worldDir)
        {
            private class ChunkDataToSave(ChunkData chunkData, long snapshotEpoch)
            {
                public ChunkData chunkData = chunkData;
                public long snapshotEpoch = snapshotEpoch;

                public static ChunkDataToSave FromChunkData(ChunkData chunkData)
                {
                    //TODO: OPTIMIZE
                    byte[] c = new byte[chunkData.compressedData.Length];
                    Buffer.BlockCopy(chunkData.compressedData, 0, c, 0, chunkData.compressedData.Length);
                    return new(new(chunkData.uncompressedSize, c, chunkData.modified), chunkData.modified.snapshot());
                }
            }

            private readonly Dictionary<Vector2D<int>, List<Task>> pendingSaves = [];
            private readonly java.io.File worldDir = worldDir;

            public bool saveRegion(Region region, int regionX, int regionZ, int chunkLimit = -1)
            {
                foreach (var regionSaveList in pendingSaves.Values)
                {
                    regionSaveList.RemoveAll(x => x.IsCompleted);
                }

                removeEmptyPendingSaves();

                Dictionary<ChunkPos, ChunkDataToSave> chunkState = [];

                foreach (var kvm in region.chunkData)
                {
                    if (!kvm.Value.modified.isModified())
                    {
                        continue;
                    }

                    chunkState[kvm.Key] = ChunkDataToSave.FromChunkData(kvm.Value);

                    if (chunkLimit != -1 && chunkState.Count >= chunkLimit)
                    {
                        break;
                    }
                }

                if (chunkState.Count == 0)
                {
                    return false;
                }

                Task save = Task.Run(() =>
                {
                    foreach (var kvm in chunkState)
                    {
                        saveChunk(kvm.Key, chunkState[kvm.Key]);
                    }
                });

                Vector2D<int> key = new(regionX, regionZ);
                if (pendingSaves.TryGetValue(key, out var pendingSavesList))
                {
                    pendingSavesList.Add(save);
                }
                else
                {
                    pendingSaves.Add(key, [save]);
                }

                return true;
            }

            private void saveChunk(ChunkPos chunkCoord, ChunkDataToSave chunk)
            {
                if (!chunk.chunkData.modified.isDirtyAt(chunk.snapshotEpoch))
                {
                    return;
                }

                DataOutputStream? outputStream = null;

                try
                {
                    outputStream = RegionIo.getChunkOutputStream(worldDir, chunkCoord.x, chunkCoord.z);

                    writeChunkData(outputStream, chunk.chunkData);
                    chunk.chunkData.modified.completeSave(chunk.snapshotEpoch);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Exception during chunk save: {e}");
                }
                finally
                {
                    outputStream?.close();
                }
            }

            public bool isBlocked(out int toSave)
            {
                int notCompleted = 0;
                foreach (var regionSaveList in pendingSaves.Values)
                {
                    regionSaveList.RemoveAll(x => x.IsCompleted);
                    notCompleted += regionSaveList.Where(x => !x.IsCompleted).Count();
                }

                if (notCompleted > 0)
                {
                    toSave = notCompleted;
                    return true;
                }

                removeEmptyPendingSaves();

                toSave = 0;
                return false;
            }

            public bool isAsyncLoadBlocked(int regionX, int regionZ)
            {
                pendingSaves.TryGetValue(new(regionX, regionZ), out var regionSaveList);
                if (regionSaveList == null)
                {
                    return false;
                }

                return regionSaveList.Any(x => !x.IsCompleted);
            }

            public void blockUntilRegionSavesCompleted(int regionX, int regionZ)
            {
                if (pendingSaves.TryGetValue(new(regionX, regionZ), out var regionSaveList))
                {
                    Task.WaitAll([.. regionSaveList]);
                }

                removeEmptyPendingSaves();
            }

            private void removeEmptyPendingSaves()
            {
                List<Vector2D<int>> savesToRemove = [];

                foreach (var kvp in pendingSaves)
                {
                    if (kvp.Value.Count == 0)
                    {
                        savesToRemove.Add(kvp.Key);
                    }
                }

                foreach (var saveToRemove in savesToRemove)
                {
                    pendingSaves.Remove(saveToRemove);
                }
            }
        }

        public class Cache
        {
            private class LoadedRegion(Region region, Vector2D<int> position)
            {
                public long lastAccess = 0;
                public readonly Region region = region;
                public readonly Vector2D<int> position = position;
            }

            private readonly Dictionary<Vector2D<int>, LoadedRegion> loadedRegions = [];
            private readonly object l = new();
            private int maxLoadedRegions;
            private RegionSaveHandler? saveHandler;
            private long currentAccess = 0;
            private long regionsLoadedSync = 0;
            private long regionsLoadedAsync = 0;

            public Cache(int maxLoadedRegions)
            {
                if (maxLoadedRegions < 4)
                {
                    throw new ArgumentException("maxLoadedRegions must be >= 4");
                }

                this.maxLoadedRegions = maxLoadedRegions;
            }

            public void setMaxLoadedRegions(java.io.File worldDir, int newMax)
            {
                if (newMax < 4)
                {
                    throw new ArgumentException("maxLoadedRegions must be >= 4");
                }

                lock (l)
                {
                    maxLoadedRegions = newMax;

                    while (loadedRegions.Count > maxLoadedRegions)
                    {
                        evictOldestRegion(worldDir);
                    }
                }
            }

            public Region getRegion(java.io.File file, int regionX, int regionZ)
            {
                lock (l)
                {
                    return getRegionUnsafe(file, regionX, regionZ);
                }
            }

            private Region getRegionUnsafe(java.io.File worldDir, int regionX, int regionZ)
            {
                Vector2D<int> key = new(regionX, regionZ);
                if (loadedRegions.TryGetValue(key, out LoadedRegion? region))
                {
                    region.lastAccess = currentAccess++;
                    return region.region;
                }

                if (loadedRegions.Count + 1 > maxLoadedRegions)
                {
                    evictOldestRegion(worldDir);
                }

                saveHandler ??= new(worldDir);
                saveHandler.blockUntilRegionSavesCompleted(regionX, regionZ);

                region = new(new(worldDir, regionX, regionZ), key)
                {
                    lastAccess = currentAccess++
                };

                loadedRegions[key] = region;
                regionsLoadedSync++;

                return region.region;
            }

            public bool addRegion(java.io.File worldDir, Region region, int regionX, int regionZ)
            {
                lock (l)
                {
                    saveHandler ??= new(worldDir);

                    if (saveHandler.isAsyncLoadBlocked(regionX, regionZ))
                    {
                        return false;
                    }

                    Vector2D<int> key = new(regionX, regionZ);
                    if (!loadedRegions.TryGetValue(key, out _))
                    {
                        if (loadedRegions.Count + 1 > maxLoadedRegions)
                        {
                            evictOldestRegion(worldDir);
                        }

                        LoadedRegion r = new(region, key)
                        {
                            lastAccess = currentAccess++
                        };

                        loadedRegions[key] = r;
                        regionsLoadedAsync++;
                        return true;
                    }

                    return false;
                }
            }

            public long getRegionsLoadedSync()
            {
                lock (l)
                {
                    return regionsLoadedSync;
                }
            }

            public long getRegionsLoadedAsync()
            {
                lock (l)
                {
                    return regionsLoadedAsync;
                }
            }

            public void resetLoadedCounters()
            {
                lock (l)
                {
                    regionsLoadedAsync = regionsLoadedSync = 0;
                }
            }

            public bool isRegionLoaded(int regionX, int regionZ)
            {
                lock (l)
                {
                    return loadedRegions.ContainsKey(new(regionX, regionZ));
                }
            }

            //returns 0 if chunk is not present
            public int getCompressedChunkSize(int chunkX, int chunkZ)
            {
                int rx = chunkX >> 5;
                int rz = chunkZ >> 5;
                Vector2D<int> key = new(rx, rz);

                lock (l)
                {
                    if (loadedRegions.TryGetValue(key, out LoadedRegion? region))
                    {
                        ChunkData? chunkData = region.region.getChunkData(chunkX, chunkZ);
                        if (chunkData != null)
                        {
                            return chunkData.compressedData.Length;
                        }
                    }
                }

                return 0;
            }

            public bool getChunkData(int chunkX, int chunkZ, byte[] compressedChunkBytes, out int uncompressedChunkSize)
            {
                int rx = chunkX >> 5;
                int rz = chunkZ >> 5;
                Vector2D<int> key = new(rx, rz);

                lock (l)
                {
                    if (loadedRegions.TryGetValue(key, out LoadedRegion? region))
                    {
                        ChunkData? chunkData = region.region.getChunkData(chunkX, chunkZ);
                        if (chunkData != null)
                        {
                            Buffer.BlockCopy(chunkData.compressedData, 0, compressedChunkBytes, 0, chunkData.compressedData.Length);
                            uncompressedChunkSize = chunkData.uncompressedSize;
                            return true;
                        }
                    }
                }

                uncompressedChunkSize = 0;
                return false;
            }

            public NbtTagCompound? readChunkNBT(java.io.File file, int chunkX, int chunkZ)
            {
                int rx = chunkX >> 5;
                int rz = chunkZ >> 5;

                lock (l)
                {
                    Region region = getRegionUnsafe(file, rx, rz);
                    return region.read(chunkX, chunkZ);
                }
            }

            public void writeChunkNBT(java.io.File file, int chunkX, int chunkZ, NbtTagCompound chunkNBT)
            {
                int rx = chunkX >> 5;
                int rz = chunkZ >> 5;

                lock (l)
                {
                    Region region = getRegionUnsafe(file, rx, rz);
                    region.write(chunkX, chunkZ, chunkNBT);
                }
            }

            public void unloadAllRegions(java.io.File worldDir)
            {
                lock (l)
                {
                    saveHandler ??= new(worldDir);

                    foreach (var region in loadedRegions.Values)
                    {
                        saveHandler.saveRegion(region.region, region.position.X, region.position.Y);
                    }

                    loadedRegions.Clear();
                }
            }

            public bool isBlocked(out int toSave)
            {
                lock (l)
                {
                    if (saveHandler == null)
                    {
                        toSave = 0;
                        return false;
                    }

                    return saveHandler.isBlocked(out toSave);
                }
            }

            public void saveRegion(java.io.File worldDir, int regionX, int regionZ, int chunkLimit = -1)
            {
                lock (l)
                {
                    if (loadedRegions.TryGetValue(new(regionX, regionZ), out LoadedRegion? region))
                    {
                        saveHandler ??= new(worldDir);
                        saveHandler.saveRegion(region.region, region.position.X, region.position.Y, chunkLimit);
                    }
                }
            }

            public bool autosaveChunks(java.io.File worldDir, int chunkLimit = -1)
            {
                lock (l)
                {
                    foreach (var region in loadedRegions.Values)
                    {
                        saveHandler ??= new(worldDir);
                        if (saveHandler.saveRegion(region.region, region.position.X, region.position.Y, chunkLimit))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public void deleteSaveHandler()
            {
                if (saveHandler == null)
                {
                    return;
                }

                while (saveHandler.isBlocked(out _)) ;

                saveHandler = null;
            }

            public void loadNearbyRegions(java.io.File worldDir, int playerX, int playerZ, int renderDistance)
            {
                HashSet<Vector2D<int>> regions = getLoadedRegions(playerX, playerZ, renderDistance, 1);
                List<Vector2D<int>> regionsToLoad = [];

                lock (l)
                {
                    foreach (var pos in regions)
                    {
                        if (!loadedRegions.ContainsKey(pos))
                        {
                            regionsToLoad.Add(pos);
                        }
                    }
                }

                foreach (var regionPos in regionsToLoad)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            java.io.File var3 = new(worldDir, "region");
                            java.io.File regionFile = new(var3, "r." + regionPos.X + "." + regionPos.Y + ".mcr");

                            if (regionFile.exists())
                            {
                                Region region = new(worldDir, regionPos.X, regionPos.Y);
                                addRegion(worldDir, region, regionPos.X, regionPos.Y);
                            }
                            else
                            {
                                addRegion(worldDir, new(), regionPos.X, regionPos.Y);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine($"Exception while loading region {regionPos.X},{regionPos.Y}: {e}");
                        }
                    });
                }
            }

            private double getDistanceToRegion(int playerChunkX, int playerChunkZ, int regionX, int regionZ)
            {
                int regionMinX = regionX * 32;
                int regionMaxX = regionMinX + 31;
                int regionMinZ = regionZ * 32;
                int regionMaxZ = regionMinZ + 31;

                int closestX = Math.Max(regionMinX, Math.Min(playerChunkX, regionMaxX));
                int closestZ = Math.Max(regionMinZ, Math.Min(playerChunkZ, regionMaxZ));

                int dx = playerChunkX - closestX;
                int dz = playerChunkZ - closestZ;
                return Math.Sqrt(dx * dx + dz * dz);
            }

            private HashSet<Vector2D<int>> getLoadedRegions(int playerWorldX, int playerWorldZ, int renderDistance, int bufferSize)
            {
                var regions = new HashSet<Vector2D<int>>();

                int playerChunkX = playerWorldX >> 4;
                int playerChunkZ = playerWorldZ >> 4;
                int playerRegionX = playerChunkX >> 5;
                int playerRegionZ = playerChunkZ >> 5;

                int actualRenderRadius = renderDistance;
                int bufferRadius = actualRenderRadius + bufferSize * 32;
                int searchRadius = (int)Math.Ceiling(bufferRadius / 32.0) + 2;

                for (int x = playerRegionX - searchRadius; x <= playerRegionX + searchRadius; x++)
                {
                    for (int z = playerRegionZ - searchRadius; z <= playerRegionZ + searchRadius; z++)
                    {
                        double distance = getDistanceToRegion(playerChunkX, playerChunkZ, x, z);

                        if (distance <= bufferRadius)
                        {
                            regions.Add(new(x, z));
                        }
                    }
                }

                return regions;
            }

            private void evictOldestRegion(java.io.File worldDir)
            {
                LoadedRegion? oldestRegion = null;
                foreach (var loadedRegion in loadedRegions.Values)
                {
                    if (oldestRegion == null || loadedRegion.lastAccess < oldestRegion.lastAccess)
                    {
                        oldestRegion = loadedRegion;
                    }
                }

                if (oldestRegion == null)
                {
                    System.Console.WriteLine("We shouldn't try to evict old regions if there are no regions");
                }
                else
                {
                    loadedRegions.Remove(oldestRegion.position);

                    saveHandler ??= new(worldDir);
                    saveHandler.saveRegion(oldestRegion.region, oldestRegion.position.X, oldestRegion.position.Y);
                }
            }
        }

        public class ModifiedFlag
        {
            private long epoch = 0;
            private long lastSaved = 0;
            private readonly object l = new();

            public void markModified()
            {
                lock (l)
                {
                    epoch++;
                }
            }

            public long snapshot()
            {
                lock (l)
                {
                    return epoch;
                }
            }

            public void completeSave(long snapshotEpoch)
            {
                lock (l)
                {
                    if (epoch == snapshotEpoch)
                    {
                        lastSaved = snapshotEpoch;
                    }
                }
            }

            public bool isModified()
            {
                lock (l)
                {
                    return epoch != lastSaved;
                }
            }

            public bool isDirtyAt(long snapshotEpoch)
            {
                lock (l)
                {
                    return snapshotEpoch > lastSaved;
                }
            }
        }

        public static readonly Cache RegionCache = new(32);
        private readonly Dictionary<ChunkPos, ChunkData> chunkData = [];

        public Region(java.io.File worldDir, int regionX, int regionZ)
        {
            int xOffset = regionX * 32;
            int zOffset = regionZ * 32;

            for (int x = 0; x < 32; x++)
            {
                for (int z = 0; z < 32; z++)
                {
                    ChunkPos chunkPos = new(xOffset + x, zOffset + z);
                    ChunkDataStream chunkStream = RegionIo.getChunkInputStream(worldDir, chunkPos.x, chunkPos.z);

                    if (chunkStream != null)
                    {
                        try
                        {
                            insertChunkData(chunkStream, chunkPos);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine($"Failed to load chunk at: {chunkPos.x}, {chunkPos.z}, {e}");
                        }
                        finally
                        {
                            chunkStream.getInputStream().close();
                        }
                    }
                }
            }
        }

        public Region()
        {
        }

        private static ChunkData readChunkData(DataInputStream inputStream)
        {
            int uncompressedSize = inputStream.readInt();
            int compressedSize = inputStream.readInt();

            byte[] compressedData = new byte[compressedSize];
            inputStream.readFully(compressedData);

            return new(uncompressedSize, compressedData, new());
        }

        private static void writeChunkData(DataOutputStream dataOutput, ChunkData chunkData)
        {
            dataOutput.writeInt(chunkData.uncompressedSize);
            dataOutput.writeInt(chunkData.compressedData.Length);
            dataOutput.write(chunkData.compressedData);
        }

        private static byte[] getNbtByteArray(NbtTagCompound nbt)
        {
            ByteArrayOutputStream outputStream = new();
            DataOutputStream dataOutput = new(outputStream);
            dataOutput.flush();
            NbtIo.write(nbt, dataOutput);
            return outputStream.toByteArray();
        }

        private static NbtTagCompound getTagCompoundFromBytes(byte[] bytes, int length)
        {
            ByteArrayInputStream inputStream = new(bytes, 0, length);
            DataInputStream dataInput = new(inputStream);
            return NbtIo.read((DataInput)dataInput);
        }

        //chunkX and chunkZ are not relative to region, they are world chunk coordinates
        public NbtTagCompound? read(int chunkX, int chunkZ)
        {
            try
            {
                if (!chunkData.TryGetValue(new(chunkX, chunkZ), out ChunkData? chunk))
                {
                    return null;
                }

                byte[] decompressBuffer = ArrayPool<byte>.Shared.Rent(chunk.uncompressedSize);
                try
                {
                    LZ4Codec.Decode(
                        chunk.compressedData, 0, chunk.compressedData.Length,
                        decompressBuffer, 0, chunk.uncompressedSize
                    );

                    return getTagCompoundFromBytes(decompressBuffer, chunk.uncompressedSize);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(decompressBuffer);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //chunkX and chunkZ are not relative to region, they are world chunk coordinates
        public void write(int chunkX, int chunkZ, NbtTagCompound nbt)
        {
            writeInternal(chunkX, chunkZ, nbt, true);
        }

        //chunkX and chunkZ are not relative to region, they are world coordinates
        public ChunkData? getChunkData(int chunkX, int chunkZ)
        {
            chunkData.TryGetValue(new(chunkX, chunkZ), out ChunkData? chunk);
            return chunk;
        }

        private void writeInternal(int chunkX, int chunkZ, NbtTagCompound nbt, bool markModified)
        {
            byte[] nbtBytes = getNbtByteArray(nbt);
            ChunkPos chunkKey = new(chunkX, chunkZ);
            byte[] compressBuffer = ArrayPool<byte>.Shared.Rent(LZ4Codec.MaximumOutputSize(nbtBytes.Length));
            byte[] compressedBytes;
            try
            {
                int size = LZ4Codec.Encode(nbtBytes, 0, nbtBytes.Length, compressBuffer, 0, compressBuffer.Length, LZ4Level.L00_FAST);
                compressedBytes = new byte[size];
                Buffer.BlockCopy(compressBuffer, 0, compressedBytes, 0, size);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(compressBuffer);
            }

            if (chunkData.TryGetValue(chunkKey, out ChunkData? chunk))
            {
                chunk.uncompressedSize = nbtBytes.Length;
                chunk.compressedData = compressedBytes;

                if (markModified)
                {
                    chunk.modified.markModified();
                }
            }
            else
            {
                chunk = new(nbtBytes.Length, compressedBytes, new());

                if (markModified)
                {
                    chunk.modified.markModified();
                }

                chunkData[chunkKey] = chunk;
            }
        }

        private void insertChunkData(ChunkDataStream chunkStream, ChunkPos chunkPos)
        {
            byte compressionType = chunkStream.getCompressionType();

            if (compressionType == 3)
            {
                chunkData[chunkPos] = readChunkData(chunkStream.getInputStream());
            }
            else if (compressionType == 2)
            {
                NbtTagCompound chunkNBT = NbtIo.read((DataInput)chunkStream.getInputStream());
                writeInternal(chunkPos.x, chunkPos.z, chunkNBT, false);
            }
            else
            {
                throw new Exception($"Invalid compression type: {compressionType}");
            }
        }
    }
}
