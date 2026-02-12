using betareborn.Blocks;
using betareborn.Client.Rendering.Core;
using betareborn.Client.Rendering.Terrain;
using betareborn.Util;
using betareborn.Worlds;
using Silk.NET.Maths;
using System.Collections.Concurrent;

namespace betareborn.Client.Rendering.Chunks
{
    public class ChunkMeshGenerator
    {
        public class MeshBuildResult
        {
            public PooledList<ChunkVertex>? Solid;
            public PooledList<ChunkVertex>? Translucent;
            public bool IsLit;
            public Vector3D<int> Pos;
            public long Version;
        }

        private class MeshBuildTask
        {
            public required WorldRegionSnapshot Cache;
            public Vector3D<int> Pos;
            public long Version;
        }

        private readonly BlockingCollection<MeshBuildTask> buildTasks = [];
        private readonly BlockingCollection<MeshBuildTask> priorityBuildTasks = [];
        private readonly BlockingCollection<MeshBuildResult> results = [];
        private readonly Thread[] workers;
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public int PendingTaskCount => buildTasks.Count + priorityBuildTasks.Count;
        public int CompletedMeshCount => results.Count;

        public ChunkMeshGenerator(int workerCount)
        {
            workers = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                workers[i] = new Thread(WorkerThread)
                {
                    IsBackground = true
                };
                workers[i].Start();
            }
        }

        public void MeshChunk(World world, Vector3D<int> pos, long version, bool priority = false)
        {
            if (buildTasks.IsAddingCompleted)
            {
                return;
            }

            WorldRegionSnapshot cache = new(world, pos.X - 1, pos.Y - 1, pos.Z - 1,
                pos.X + SubChunkRenderer.SIZE + 1, pos.Y + SubChunkRenderer.SIZE + 1, pos.Z + SubChunkRenderer.SIZE + 1);

            MeshBuildTask task = new() { Cache = cache, Pos = pos, Version = version };

            try
            {
                if (priority)
                {
                    priorityBuildTasks.Add(task);
                }
                else
                {
                    buildTasks.Add(task);
                }
            }
            catch (InvalidOperationException)
            {
                // Collection was marked as complete, dispose the cache
                cache.Dispose();
            }
        }

        public void Stop()
        {
            buildTasks.CompleteAdding();
            priorityBuildTasks.CompleteAdding();

            cancellationTokenSource.Cancel();

            foreach (var thread in workers)
            {
                thread.Join();
            }

            buildTasks.Dispose();
            priorityBuildTasks.Dispose();
            results.Dispose();
            cancellationTokenSource.Dispose();
        }

        public MeshBuildResult? GetMesh()
        {
            results.TryTake(out MeshBuildResult? result);
            return result;
        }

        private void WorkerThread()
        {
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (priorityBuildTasks.TryTake(out MeshBuildTask? task, 0, cancellationTokenSource.Token))
                    {
                        results.Add(MeshChunk(task.Pos, task.Version, task.Cache), cancellationTokenSource.Token);
                    }
                    else if (buildTasks.TryTake(out task, 100, cancellationTokenSource.Token))
                    {
                        results.Add(MeshChunk(task.Pos, task.Version, task.Cache), cancellationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static MeshBuildResult MeshChunk(Vector3D<int> pos, long version, WorldRegionSnapshot cache)
        {
            int minX = pos.X;
            int minY = pos.Y;
            int minZ = pos.Z;
            int maxX = pos.X + SubChunkRenderer.SIZE;
            int maxY = pos.Y + SubChunkRenderer.SIZE;
            int maxZ = pos.Z + SubChunkRenderer.SIZE;

            var result = new MeshBuildResult
            {
                Pos = pos,
                Version = version
            };

            var tess = new Tessellator();
            var rb = new BlockRenderer(cache, tess);

            for (int pass = 0; pass < 2; pass++)
            {
                bool hasNextPass = false;

                tess.startCapture(TesselatorCaptureVertexFormat.Chunk);
                tess.startDrawingQuads();
                tess.setTranslationD(-pos.X, -pos.Y, -pos.Z);

                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        for (int x = minX; x < maxX; x++)
                        {
                            int id = cache.getBlockId(x, y, z);
                            if (id <= 0) continue;

                            Block b = Block.BLOCKS[id];
                            int blockPass = b.getRenderLayer();

                            if (blockPass != pass)
                            {
                                hasNextPass = true;
                            }
                            else
                            {
                                rb.renderBlockByRenderType(b, x, y, z);
                            }
                        }
                    }
                }

                tess.draw();
                tess.setTranslationD(0, 0, 0);

                var verts = tess.endCaptureChunkVertices();
                if (verts.Count > 0)
                {
                    if (pass == 0)
                    {
                        result.Solid = verts;
                    }
                    else
                    {
                        result.Translucent = verts;
                    }
                }

                if (!hasNextPass)
                {
                    break;
                }
            }

            result.IsLit = cache.getIsLit();
            cache.Dispose();
            return result;
        }
    }
}