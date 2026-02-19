using BetaSharp.Client.Rendering.Core;
using BetaSharp.Profiling;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Chunks;

public class ChunkRenderer
{
    static ChunkRenderer()
    {
        var offsets = new List<Vector3D<int>>();

        for (int x = -MAX_RENDER_DISTANCE; x <= MAX_RENDER_DISTANCE; x++)
        {
            for (int y = -8; y <= 8; y++)
            {
                for (int z = -MAX_RENDER_DISTANCE; z <= MAX_RENDER_DISTANCE; z++)
                {
                    offsets.Add(new Vector3D<int>(x, y, z));
                }
            }
        }

        offsets.Sort((a, b) =>
            (a.X * a.X + a.Y * a.Y + a.Z * a.Z).CompareTo(b.X * b.X + b.Y * b.Y + b.Z * b.Z));

        spiralOffsets = [.. offsets];
    }

    private class SubChunkState(bool isLit, SubChunkRenderer renderer)
    {
        public bool IsLit { get; set; } = isLit;
        public SubChunkRenderer Renderer { get; } = renderer;
    }

    private struct ChunkToMeshInfo(Vector3D<int> pos, long version, bool priority)
    {
        public Vector3D<int> Pos = pos;
        public long Version = version;
        public bool priority = priority;
    }

    private static readonly Vector3D<int>[] spiralOffsets;
    private const int MAX_RENDER_DISTANCE = 32 + 1;
    private readonly Dictionary<Vector3D<int>, SubChunkState> renderers = [];
    private readonly List<SubChunkRenderer> translucentRenderers = [];
    private readonly List<SubChunkRenderer> renderersToRemove = [];
    private readonly ChunkMeshGenerator meshGenerator;
    private readonly World world;
    private readonly Dictionary<Vector3D<int>, ChunkMeshVersion> chunkVersions = [];
    private readonly List<Vector3D<int>> chunkVersionsToRemove = [];
    private readonly List<ChunkToMeshInfo> dirtyChunks = [];
    private readonly List<ChunkToMeshInfo> lightingUpdates = [];
    private readonly Core.Shader chunkShader;
    private int lastRenderDistance;
    private Vector3D<double> lastViewPos;
    private int currentIndex;
    private Matrix4X4<float> modelView;
    private Matrix4X4<float> projection;
    private int fogMode;
    private float fogDensity;
    private float fogStart;
    private float fogEnd;
    private Vector4D<float> fogColor;

    public int LoadedMeshes => renderers.Count;
    public int TranslucentMeshes { get; private set; }

    public ChunkRenderer(World world)
    {
        meshGenerator = new();
        this.world = world;

        chunkShader = new(AssetManager.Instance.getAsset("shaders/chunk.vert").getTextContent(), AssetManager.Instance.getAsset("shaders/chunk.frag").getTextContent());
        Log.Info("Loaded chunk shader");

        GLManager.GL.UseProgram(0);
    }

    private static int CalculateRealRenderDistance(int val) // TODO: Maybe it's better to flip the values?
    {
        return val switch
        {
            0 => 16,
            1 => 8,
            2 => 4,
            3 => 2,
            _ => 0,
        };
    }

    public void Render(Culler camera, Vector3D<double> viewPos, int renderDistance, long ticks, float partialTicks, bool envAnim)
    {
        lastRenderDistance = CalculateRealRenderDistance(renderDistance);
        lastViewPos = viewPos;

        chunkShader.Bind();
        chunkShader.SetUniform1("textureSampler", 0);
        chunkShader.SetUniform1("fogMode", fogMode);
        chunkShader.SetUniform1("fogDensity", fogDensity);
        chunkShader.SetUniform1("fogStart", fogStart);
        chunkShader.SetUniform1("fogEnd", fogEnd);
        chunkShader.SetUniform4("fogColor", fogColor);

        int wrappedTicks = (int)(ticks % 24000);
        chunkShader.SetUniform1("time", (wrappedTicks + partialTicks) / 20.0f);
        chunkShader.SetUniform1("envAnim", envAnim ? 1 : 0);

        var modelView = new Matrix4X4<float>();
        var projection = new Matrix4X4<float>();

        unsafe
        {
            GLManager.GL.GetFloat(GLEnum.ModelviewMatrix, (float*)&modelView);
        }

        unsafe
        {
            GLManager.GL.GetFloat(GLEnum.ProjectionMatrix, (float*)&projection);
        }

        this.modelView = modelView;
        this.projection = projection;

        chunkShader.SetUniformMatrix4("projectionMatrix", projection);

        int translucentCount = 0;
        foreach (var state in renderers.Values)
        {
            if (!IsChunkInRenderDistance(state.Renderer.Position, viewPos))
            {
                renderersToRemove.Add(state.Renderer);
                continue;
            }

            if (state.Renderer.HasTranslucentMesh)
            {
                translucentCount++;
            }

            if (camera.isBoundingBoxInFrustum(state.Renderer.BoundingBox))
            {
                state.Renderer.Render(chunkShader, 0, viewPos, modelView);

                if (state.Renderer.HasTranslucentMesh)
                {
                    translucentRenderers.Add(state.Renderer);
                }
            }
        }
        TranslucentMeshes = translucentCount;

        foreach (var renderer in renderersToRemove)
        {
            renderers.Remove(renderer.Position);
            renderer.Dispose();

            chunkVersions.Remove(renderer.Position);
        }

        renderersToRemove.Clear();

        ProcessOneMeshUpdate(camera);
        ProcessOneLightingMeshUpdate();
        LoadNewMeshes(viewPos);

        GLManager.GL.UseProgram(0);
        Core.VertexArray.Unbind();
    }

    public void SetFogMode(int mode)
    {
        fogMode = mode;
    }

    public void SetFogDensity(float density)
    {
        fogDensity = density;
    }

    public void SetFogStart(float start)
    {
        fogStart = start;
    }

    public void SetFogEnd(float end)
    {
        fogEnd = end;
    }

    public void SetFogColor(float r, float g, float b, float a)
    {
        fogColor = new(r, g, b, a);
    }

    public void RenderTransparent(Vector3D<double> viewPos)
    {
        chunkShader.Bind();
        chunkShader.SetUniform1("textureSampler", 0);

        chunkShader.SetUniformMatrix4("projectionMatrix", projection);

        translucentRenderers.Sort((a, b) =>
        {
            double distA = Vector3D.DistanceSquared(ToDoubleVec(a.Position), viewPos);
            double distB = Vector3D.DistanceSquared(ToDoubleVec(b.Position), viewPos);
            return distB.CompareTo(distA);
        });

        foreach (var renderer in translucentRenderers)
        {
            renderer.Render(chunkShader, 1, viewPos, modelView);
        }

        translucentRenderers.Clear();

        GLManager.GL.UseProgram(0);
        Core.VertexArray.Unbind();
    }

    private void LoadNewMeshes(Vector3D<double> viewPos, int maxChunks = 8)
    {
        for (int i = 0; i < maxChunks; i++)
        {
            if (meshGenerator.Mesh is MeshBuildResult mesh)
            {
                if (IsChunkInRenderDistance(mesh.Pos, viewPos))
                {
                    if (!chunkVersions.TryGetValue(mesh.Pos, out var version))
                    {
                        version = new ChunkMeshVersion();
                        chunkVersions[mesh.Pos] = version;
                    }

                    version.CompleteMesh(mesh.Version);

                    if (version.IsStale(mesh.Version))
                    {
                        long? snapshot = version.SnapshotIfNeeded();
                        if (snapshot.HasValue)
                        {
                            meshGenerator.MeshChunk(world, mesh.Pos, snapshot.Value);
                        }
                        continue;
                    }

                    if (renderers.TryGetValue(mesh.Pos, out SubChunkState? state))
                    {
                        state.Renderer.UploadMeshData(mesh.Solid, mesh.Translucent);
                        state.IsLit = mesh.IsLit;
                    }
                    else
                    {
                        var renderer = new SubChunkRenderer(mesh.Pos);
                        renderer.UploadMeshData(mesh.Solid, mesh.Translucent);
                        renderers[mesh.Pos] = new SubChunkState(mesh.IsLit, renderer);
                    }
                }
            }
        }
    }

    private void ProcessOneMeshUpdate(Culler camera)
    {
        dirtyChunks.Sort((a, b) =>
        {
            var distA = Vector3D.DistanceSquared(ToDoubleVec(a.Pos), lastViewPos);
            var distB = Vector3D.DistanceSquared(ToDoubleVec(b.Pos), lastViewPos);
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < dirtyChunks.Count; i++)
        {
            var info = dirtyChunks[i];

            if (!IsChunkInRenderDistance(info.Pos, lastViewPos))
            {
                dirtyChunks.RemoveAt(i);
                i--;
                continue;
            }

            var aabb = new Box(
                info.Pos.X, info.Pos.Y, info.Pos.Z,
                info.Pos.X + SubChunkRenderer.Size,
                info.Pos.Y + SubChunkRenderer.Size,
                info.Pos.Z + SubChunkRenderer.Size
            );

            if (!camera.isBoundingBoxInFrustum(aabb))
            {
                continue;
            }

            meshGenerator.MeshChunk(world, info.Pos, info.Version);
            dirtyChunks.RemoveAt(i);
            return;
        }
    }

    private void ProcessOneLightingMeshUpdate()
    {
        lightingUpdates.Sort((a, b) =>
        {
            var distA = Vector3D.DistanceSquared(ToDoubleVec(a.Pos), lastViewPos);
            var distB = Vector3D.DistanceSquared(ToDoubleVec(b.Pos), lastViewPos);
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < lightingUpdates.Count; i++)
        {
            ChunkToMeshInfo update = lightingUpdates[i];

            if (!IsChunkInRenderDistance(update.Pos, lastViewPos))
            {
                lightingUpdates.RemoveAt(i);
                i--;
                continue;
            }

            meshGenerator.MeshChunk(world, update.Pos, update.Version);
            lightingUpdates.RemoveAt(i);
            return;
        }
    }

    public void UpdateAllRenderers()
    {
        foreach (var state in renderers.Values)
        {
            if (IsChunkInRenderDistance(state.Renderer.Position, lastViewPos) && state.IsLit)
            {
                if (!chunkVersions.TryGetValue(state.Renderer.Position, out var version))
                {
                    version = new();
                    chunkVersions[state.Renderer.Position] = version;
                }

                version.MarkDirty();

                long? snapshot = version.SnapshotIfNeeded();
                if (snapshot.HasValue)
                {
                    lightingUpdates.Add(new(state.Renderer.Position, snapshot.Value, false));
                }
            }
        }
    }

    public void Tick(Vector3D<double> viewPos)
    {
        Profiler.Start("WorldRenderer.Tick");

        lastViewPos = viewPos;

        Vector3D<int> currentChunk = new(
            (int)Math.Floor(viewPos.X / SubChunkRenderer.Size),
            (int)Math.Floor(viewPos.Y / SubChunkRenderer.Size),
            (int)Math.Floor(viewPos.Z / SubChunkRenderer.Size)
        );

        int radiusSq = lastRenderDistance * lastRenderDistance;
        int enqueuedCount = 0;
        bool priorityPassClean = true;

        //TODO: MAKE THESE CONFIGURABLE
        const int MAX_CHUNKS_PER_FRAME = 32;
        const int PRIORITY_PASS_LIMIT = 1024;
        const int BACKGROUND_PASS_LIMIT = 2048;

        for (int i = 0; i < PRIORITY_PASS_LIMIT && i < spiralOffsets.Length; i++)
        {
            var offset = spiralOffsets[i];
            int distSq = offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z;

            if (distSq > radiusSq)
                break;

            var chunkPos = (currentChunk + offset) * SubChunkRenderer.Size;

            if (chunkPos.Y < 0 || chunkPos.Y >= 128)
                continue;

            if (renderers.ContainsKey(chunkPos) || chunkVersions.ContainsKey(chunkPos))
                continue;

            if (MarkDirty(chunkPos))
            {
                enqueuedCount++;
                priorityPassClean = false;
            }
            else
            {
                priorityPassClean = false;
            }

            if (enqueuedCount >= MAX_CHUNKS_PER_FRAME)
                break;
        }

        if (priorityPassClean && enqueuedCount < MAX_CHUNKS_PER_FRAME)
        {
            for (int i = 0; i < BACKGROUND_PASS_LIMIT; i++)
            {
                var offset = spiralOffsets[currentIndex];
                int distSq = offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z;

                if (distSq <= radiusSq)
                {
                    var chunkPos = (currentChunk + offset) * SubChunkRenderer.Size;
                    if (!renderers.ContainsKey(chunkPos) && !chunkVersions.ContainsKey(chunkPos))
                    {
                        if (MarkDirty(chunkPos))
                        {
                            enqueuedCount++;
                        }
                    }
                }

                currentIndex = (currentIndex + 1) % spiralOffsets.Length;

                if (enqueuedCount >= MAX_CHUNKS_PER_FRAME)
                    break;
            }
        }

        Profiler.Start("WorldRenderer.Tick.RemoveVersions");
        foreach (var version in chunkVersions)
        {
            if (!IsChunkInRenderDistance(version.Key, lastViewPos))
            {
                chunkVersionsToRemove.Add(version.Key);
            }
        }

        foreach (var pos in chunkVersionsToRemove)
        {
            chunkVersions.Remove(pos);
        }

        chunkVersionsToRemove.Clear();
        Profiler.Stop("WorldRenderer.Tick.RemoveVersions");

        Profiler.Stop("WorldRenderer.Tick");
    }

    public bool MarkDirty(Vector3D<int> chunkPos, bool priority = false)
    {
        if (!world.isRegionLoaded(chunkPos.X - 1, chunkPos.Y - 1, chunkPos.Z - 1, chunkPos.X + SubChunkRenderer.Size + 1, chunkPos.Y + SubChunkRenderer.Size + 1, chunkPos.Z + SubChunkRenderer.Size + 1) | !IsChunkInRenderDistance(chunkPos, lastViewPos))
            return false;

        if (!chunkVersions.TryGetValue(chunkPos, out var version))
        {
            version = new();
            chunkVersions[chunkPos] = version;
        }
        version.MarkDirty();

        long? snapshot = version.SnapshotIfNeeded();
        if (snapshot.HasValue)
        {
            for (int i = 0; i < dirtyChunks.Count; i++)
            {
                if (dirtyChunks[i].Pos == chunkPos)
                {
                    dirtyChunks[i] = new(chunkPos, snapshot.Value, priority || dirtyChunks[i].priority);
                    return true;
                }
            }

            dirtyChunks.Add(new(chunkPos, snapshot.Value, priority));
            return true;
        }

        return false;
    }

    private bool IsChunkInRenderDistance(Vector3D<int> chunkWorldPos, Vector3D<double> viewPos)
    {
        int chunkX = chunkWorldPos.X / SubChunkRenderer.Size;
        int chunkZ = chunkWorldPos.Z / SubChunkRenderer.Size;

        int viewChunkX = (int)Math.Floor(viewPos.X / SubChunkRenderer.Size);
        int viewChunkZ = (int)Math.Floor(viewPos.Z / SubChunkRenderer.Size);

        int dist = Vector2D.Distance(new Vector2D<int>(chunkX, chunkZ), new Vector2D<int>(viewChunkX, viewChunkZ));
        bool isIn = dist <= lastRenderDistance;
        return isIn;
    }

    private static Vector3D<double> ToDoubleVec(Vector3D<int> vec) => new(vec.X, vec.Y, vec.Z);

    public void Dispose()
    {
        foreach (var state in renderers.Values)
        {
            state.Renderer.Dispose();
        }

        chunkShader.Dispose();

        renderers.Clear();

        translucentRenderers.Clear();
        renderersToRemove.Clear();
        chunkVersions.Clear();
    }
}