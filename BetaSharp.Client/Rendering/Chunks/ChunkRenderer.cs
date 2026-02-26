using BetaSharp.Client.Rendering.Chunks.Occlusion;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Profiling;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Chunks;

public class ChunkRenderer : IChunkVisibilityVisitor
{
    private readonly ILogger<ChunkRenderer> _logger = Log.Instance.For<ChunkRenderer>();

    static ChunkRenderer()
    {
        var offsets = new List<Vector3D<int>>();

        for (int x = -MaxRenderDistance; x <= MaxRenderDistance; x++)
        {
            for (int y = -8; y <= 8; y++)
            {
                for (int z = -MaxRenderDistance; z <= MaxRenderDistance; z++)
                {
                    offsets.Add(new Vector3D<int>(x, y, z));
                }
            }
        }

        offsets.Sort((a, b) =>
            (a.X * a.X + a.Y * a.Y + a.Z * a.Z).CompareTo(b.X * b.X + b.Y * b.Y + b.Z * b.Z));

        s_spiralOffsets = [.. offsets];
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

    private static readonly Vector3D<int>[] s_spiralOffsets;
    private const int MaxRenderDistance = 32 + 1;
    private readonly Dictionary<Vector3D<int>, SubChunkState> _renderers = [];
    private readonly List<SubChunkRenderer> _translucentRenderers = [];
    private readonly List<SubChunkRenderer> _renderersToRemove = [];
    private readonly ChunkMeshGenerator _meshGenerator;
    private readonly World _world;
    private readonly Dictionary<Vector3D<int>, ChunkMeshVersion> _chunkVersions = [];
    private readonly List<Vector3D<int>> _chunkVersionsToRemove = [];
    private readonly List<ChunkToMeshInfo> _dirtyChunks = [];
    private readonly List<ChunkToMeshInfo> _lightingUpdates = [];
    private readonly Dictionary<Vector3D<int>, RenderRegion> _regions = [];
    private readonly Core.Shader _chunkShader;
    private int _lastRenderDistance;
    private Vector3D<double> _lastViewPos;
    private int _currentIndex;
    private Matrix4X4<float> _modelView;
    private Matrix4X4<float> _projection;
    private int _fogMode;
    private float _fogDensity;
    private float _fogStart;
    private float _fogEnd;
    private Vector4D<float> _fogColor;
    private readonly ChunkOcclusionCuller _occlusionCuller = new();
    private readonly List<SubChunkRenderer> _visibleRenderers = [];
    private int _frameIndex = 0;

    public bool UseOcclusionCulling { get; set; } = true;

    public int TotalChunks => _renderers.Count;
    public int ChunksInFrustum { get; private set; }
    public int ChunksOccluded { get; private set; }
    public int ChunksRendered { get; private set; }
    public int TranslucentMeshes { get; private set; }

    public ChunkRenderer(World world)
    {
        _meshGenerator = new();
        _world = world;

        _chunkShader = new(AssetManager.Instance.getAsset("shaders/chunk.vert").getTextContent(), AssetManager.Instance.getAsset("shaders/chunk.frag").getTextContent());
        _logger.LogInformation("Loaded chunk shader");

        GLManager.GL.UseProgram(0);
    }

    public void Render(ChunkRenderParams renderParams)
    {
        _lastRenderDistance = renderParams.RenderDistance;
        _lastViewPos = renderParams.ViewPos;

        _chunkShader.Bind();
        _chunkShader.SetUniform1("textureSampler", 0);
        _chunkShader.SetUniform1("fogMode", _fogMode);
        _chunkShader.SetUniform1("fogDensity", _fogDensity);
        _chunkShader.SetUniform1("fogStart", _fogStart);
        _chunkShader.SetUniform1("fogEnd", _fogEnd);
        _chunkShader.SetUniform4("fogColor", _fogColor);

        int wrappedTicks = (int)(renderParams.Ticks % 24000);
        _chunkShader.SetUniform1("time", (wrappedTicks + renderParams.PartialTicks) / 20.0f);
        _chunkShader.SetUniform1("envAnim", renderParams.EnvironmentAnimation ? 1 : 0);
        _chunkShader.SetUniform1("chunkFadeEnabled", renderParams.ChunkFade ? 1 : 0);

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

        _modelView = modelView;
        _projection = projection;

        _chunkShader.SetUniformMatrix4("projectionMatrix", projection);

        _visibleRenderers.Clear();
        _frameIndex++;

        Vector3D<int> cameraChunkPos = new(
            (int)Math.Floor(renderParams.ViewPos.X / SubChunkRenderer.Size) * SubChunkRenderer.Size,
            (int)Math.Floor(renderParams.ViewPos.Y / SubChunkRenderer.Size) * SubChunkRenderer.Size,
            (int)Math.Floor(renderParams.ViewPos.Z / SubChunkRenderer.Size) * SubChunkRenderer.Size
        );

        _renderers.TryGetValue(cameraChunkPos, out SubChunkState? cameraState);

        if (cameraState == null)
        {
            int y = Math.Clamp(cameraChunkPos.Y, 0, 112);
            _renderers.TryGetValue(new Vector3D<int>(cameraChunkPos.X, y, cameraChunkPos.Z), out cameraState);
        }

        float renderDistWorld = renderParams.RenderDistance * SubChunkRenderer.Size;

        Profiler.Start("FindVisible");

        _occlusionCuller.FindVisible(
            this,
            cameraState?.Renderer,
            renderParams.ViewPos,
            renderParams.Camera,
            renderDistWorld,
            UseOcclusionCulling,
            _frameIndex
        );

        Profiler.Stop("FindVisible");

        AddNearbySections(cameraChunkPos, _frameIndex, renderParams.Camera);

        int frustumCount = 0;
        int visitedVisibleCount = _visibleRenderers.Count;

        foreach (SubChunkState state in _renderers.Values)
        {
            if (renderParams.Camera.isBoundingBoxInFrustum(state.Renderer.BoundingBox))
            {
                frustumCount++;
            }
        }

        ChunksInFrustum = frustumCount;
        ChunksOccluded = frustumCount - visitedVisibleCount;
        ChunksRendered = visitedVisibleCount;

        if (renderParams.RenderOccluded)
        {
            var occludedRenderers = new List<SubChunkRenderer>();
            foreach (SubChunkState state in _renderers.Values)
            {
                SubChunkRenderer renderer = state.Renderer;
                if (renderer.LastVisibleFrame != _frameIndex)
                {
                    if (renderer.IsVisible(renderParams.Camera, renderParams.ViewPos, renderDistWorld))
                    {
                        occludedRenderers.Add(renderer);
                    }
                }
            }
            _visibleRenderers.Clear();
            _visibleRenderers.AddRange(occludedRenderers);
            ChunksRendered = _visibleRenderers.Count;
        }

        int translucentCount = 0;
        foreach (SubChunkRenderer renderer in _visibleRenderers)
        {
            renderer.Update(renderParams.DeltaTime);

            if (renderer.HasTranslucentMesh)
            {
                translucentCount++;
                _translucentRenderers.Add(renderer);
            }
        }

        // --- BATCHED SOLID PASS ---
        Profiler.Start("RenderSolidBatched");
        
        // Group visible chunks by region
        var groupedVisible = new Dictionary<RenderRegion, List<SubChunkRenderer>>();
        foreach (var r in _visibleRenderers)
        {
            var region = GetRegionForRenderer(r);
            if (region == null) continue;
            if (!groupedVisible.TryGetValue(region, out var list))
            {
                list = new List<SubChunkRenderer>();
                groupedVisible[region] = list;
            }
            list.Add(r);
        }

        foreach (var entry in groupedVisible)
        {
            var region = entry.Key;
            var visibleInRegion = entry.Value;
            var batch = region.GetBatch(0);
            batch.Clear();

            foreach (var renderer in visibleInRegion)
            {
                int offset = region.GetOffset(0, GetSectionIndex(renderer.Position));
                int count = renderer.GetVertexCount(0);
                if (offset != -1 && count > 0)
                {
                    batch.Add(offset, (uint)count);
                }
            }

            if (!batch.IsEmpty)
            {
                // Set region-specific uniforms
                Vector3D<double> pos = new(region.Origin.X - renderParams.ViewPos.X, region.Origin.Y - renderParams.ViewPos.Y, region.Origin.Z - renderParams.ViewPos.Z);
                var regionModelView = Matrix4X4.CreateTranslation(new Vector3D<float>((float)pos.X, (float)pos.Y, (float)pos.Z)) * modelView;
                
                _chunkShader.SetUniformMatrix4("modelViewMatrix", regionModelView);
                _chunkShader.SetUniform3("chunkPos", (float)region.Origin.X, (float)region.Origin.Y, (float)region.Origin.Z);
                _chunkShader.SetUniform1("fadeProgress", 1.0f);

                region.Bind(0);
                unsafe
                {
                    GLManager.GL.MultiDrawArrays(GLEnum.Triangles, batch.Firsts, batch.Counts, (uint)batch.Size);
                }
            }
        }
        RenderRegion.Unbind();
        Profiler.Stop("RenderSolidBatched");

        foreach (SubChunkState state in _renderers.Values)
        {
            if (!IsChunkInRenderDistance(state.Renderer.Position, renderParams.ViewPos))
            {
                _renderersToRemove.Add(state.Renderer);
            }
        }
        TranslucentMeshes = translucentCount;

        foreach (SubChunkRenderer renderer in _renderersToRemove)
        {
            UpdateAdjacency(renderer, false);
            _renderers.Remove(renderer.Position);
            
            // If the section is being removed, we should also check if its region can be removed
            // Sodium has a more complex management, for now we'll just let them live or 
            // periodically sweep them.
            
            renderer.Dispose();

            _chunkVersions.Remove(renderer.Position);
        }

        _renderersToRemove.Clear();

        ProcessOneMeshUpdate(renderParams.Camera);
        ProcessOneLightingMeshUpdate();
        LoadNewMeshes(renderParams.ViewPos);

        GLManager.GL.UseProgram(0);
        Core.VertexArray.Unbind();
    }

    public void SetFogMode(int mode)
    {
        _fogMode = mode;
    }

    public void SetFogDensity(float density)
    {
        _fogDensity = density;
    }

    public void SetFogStart(float start)
    {
        _fogStart = start;
    }

    public void SetFogEnd(float end)
    {
        _fogEnd = end;
    }

    public void SetFogColor(float r, float g, float b, float a)
    {
        _fogColor = new(r, g, b, a);
    }

    public void RenderTransparent(ChunkRenderParams renderParams)
    {
        _chunkShader.Bind();
        _chunkShader.SetUniform1("textureSampler", 0);

        _chunkShader.SetUniformMatrix4("projectionMatrix", _projection);

        // Group visible translucent renderers by region
        var groupedTranslucent = new Dictionary<RenderRegion, List<SubChunkRenderer>>();
        foreach (var r in _translucentRenderers)
        {
            var region = GetRegionForRenderer(r);
            if (region == null) continue;
            if (!groupedTranslucent.TryGetValue(region, out var list))
            {
                list = new List<SubChunkRenderer>();
                groupedTranslucent[region] = list;
            }
            list.Add(r);
        }

        // Sort regions by distance for transparency
        var sortedRegions = groupedTranslucent.Keys.ToList();
        sortedRegions.Sort((a, b) =>
        {
            double distA = Vector3D.DistanceSquared(ToDoubleVec(a.Origin), renderParams.ViewPos);
            double distB = Vector3D.DistanceSquared(ToDoubleVec(b.Origin), renderParams.ViewPos);
            return distB.CompareTo(distA);
        });

        foreach (var region in sortedRegions)
        {
            var visibleInRegion = groupedTranslucent[region];
            var batch = region.GetBatch(1);
            batch.Clear();

            // Note: Within a region we might still want to sort sections, 
            // but for now we'll just batch them.
            foreach (var renderer in visibleInRegion)
            {
                int offset = region.GetOffset(1, GetSectionIndex(renderer.Position));
                int count = renderer.GetVertexCount(1);
                if (offset != -1 && count > 0)
                {
                    batch.Add(offset, (uint)count);
                }
            }

            if (!batch.IsEmpty)
            {
                Vector3D<double> pos = new(region.Origin.X - renderParams.ViewPos.X, region.Origin.Y - renderParams.ViewPos.Y, region.Origin.Z - renderParams.ViewPos.Z);
                var regionModelView = Matrix4X4.CreateTranslation(new Vector3D<float>((float)pos.X, (float)pos.Y, (float)pos.Z)) * _modelView;
                
                _chunkShader.SetUniformMatrix4("modelViewMatrix", regionModelView);
                _chunkShader.SetUniform3("chunkPos", (float)region.Origin.X, (float)region.Origin.Y, (float)region.Origin.Z);
                _chunkShader.SetUniform1("fadeProgress", 1.0f);

                region.Bind(1);
                unsafe
                {
                    GLManager.GL.MultiDrawArrays(GLEnum.Triangles, batch.Firsts, batch.Counts, (uint)batch.Size);
                }
            }
        }
        RenderRegion.Unbind();

        _translucentRenderers.Clear();

        GLManager.GL.UseProgram(0);
        Core.VertexArray.Unbind();
    }

    private void LoadNewMeshes(Vector3D<double> viewPos, int maxChunks = 8)
    {
        for (int i = 0; i < maxChunks; i++)
        {
            if (_meshGenerator.Mesh is MeshBuildResult mesh)
            {
                if (IsChunkInRenderDistance(mesh.Pos, viewPos))
                {
                    if (!_chunkVersions.TryGetValue(mesh.Pos, out ChunkMeshVersion? version))
                    {
                        version = ChunkMeshVersion.Get();
                        _chunkVersions[mesh.Pos] = version;
                    }

                    version.CompleteMesh(mesh.Version);

                    if (version.IsStale(mesh.Version))
                    {
                        long? snapshot = version.SnapshotIfNeeded();
                        if (snapshot.HasValue)
                        {
                            _meshGenerator.MeshChunk(_world, mesh.Pos, snapshot.Value);
                        }
                        continue;
                    }

                    if (_renderers.TryGetValue(mesh.Pos, out SubChunkState? state))
                    {
                        var region = GetOrCreateRegion(mesh.Pos);
                        state.Renderer.SetRegion(region, GetSectionIndex(mesh.Pos));
                        state.Renderer.UploadMeshData(mesh.Solid, mesh.Translucent);
                        state.IsLit = mesh.IsLit;
                        state.Renderer.VisibilityData = mesh.VisibilityData;
                    }
                    else
                    {
                        var renderer = new SubChunkRenderer(mesh.Pos);
                        var region = GetOrCreateRegion(mesh.Pos);
                        renderer.SetRegion(region, GetSectionIndex(mesh.Pos));
                        renderer.UploadMeshData(mesh.Solid, mesh.Translucent);
                        renderer.VisibilityData = mesh.VisibilityData;
                        _renderers[mesh.Pos] = new SubChunkState(mesh.IsLit, renderer);
                        UpdateAdjacency(renderer, true);
                    }
                }
            }
        }
    }

    private void UpdateAdjacency(SubChunkRenderer renderer, bool added)
    {
        Vector3D<int> pos = renderer.Position;
        int size = SubChunkRenderer.Size;

        SubChunkRenderer? Get(Vector3D<int> p) => _renderers.TryGetValue(p, out SubChunkState? s) ? s.Renderer : null;

        SubChunkRenderer? down = Get(pos + new Vector3D<int>(0, -size, 0));
        SubChunkRenderer? up = Get(pos + new Vector3D<int>(0, size, 0));
        SubChunkRenderer? north = Get(pos + new Vector3D<int>(0, 0, -size));
        SubChunkRenderer? south = Get(pos + new Vector3D<int>(0, 0, size));
        SubChunkRenderer? west = Get(pos + new Vector3D<int>(-size, 0, 0));
        SubChunkRenderer? east = Get(pos + new Vector3D<int>(size, 0, 0));

        if (added)
        {
            renderer.AdjacentDown = down;
            renderer.AdjacentUp = up;
            renderer.AdjacentNorth = north;
            renderer.AdjacentSouth = south;
            renderer.AdjacentWest = west;
            renderer.AdjacentEast = east;

            down?.AdjacentUp = renderer;
            up?.AdjacentDown = renderer;
            north?.AdjacentSouth = renderer;
            south?.AdjacentNorth = renderer;
            west?.AdjacentEast = renderer;
            east?.AdjacentWest = renderer;
        }
        else
        {
            down?.AdjacentUp = null;
            up?.AdjacentDown = null;
            north?.AdjacentSouth = null;
            south?.AdjacentNorth = null;
            west?.AdjacentEast = null;
            east?.AdjacentWest = null;
        }
    }

    public void Visit(SubChunkRenderer renderer)
    {
        _visibleRenderers.Add(renderer);
    }

    private void AddNearbySections(Vector3D<int> cameraChunkPos, int frame, Culler camera)
    {
        int size = SubChunkRenderer.Size;
        for (int x = -size; x <= size; x += size)
        {
            for (int y = -size; y <= size; y += size)
            {
                for (int z = -size; z <= size; z += size)
                {
                    Vector3D<int> pos = cameraChunkPos + new Vector3D<int>(x, y, z);
                    if (_renderers.TryGetValue(pos, out SubChunkState? state))
                    {
                        if (state.Renderer.LastVisibleFrame != frame)
                        {
                            state.Renderer.LastVisibleFrame = frame;
                            if (camera.isBoundingBoxInFrustum(state.Renderer.BoundingBox))
                            {
                                Visit(state.Renderer);
                            }
                        }
                    }
                }
            }
        }
    }

    private void ProcessOneMeshUpdate(Culler camera)
    {
        _dirtyChunks.Sort((a, b) =>
        {
            double distA = Vector3D.DistanceSquared(ToDoubleVec(a.Pos), _lastViewPos);
            double distB = Vector3D.DistanceSquared(ToDoubleVec(b.Pos), _lastViewPos);
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < _dirtyChunks.Count; i++)
        {
            ChunkToMeshInfo info = _dirtyChunks[i];

            if (!IsChunkInRenderDistance(info.Pos, _lastViewPos))
            {
                _dirtyChunks.RemoveAt(i);
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

            _meshGenerator.MeshChunk(_world, info.Pos, info.Version);
            _dirtyChunks.RemoveAt(i);
            return;
        }
    }

    private void ProcessOneLightingMeshUpdate()
    {
        _lightingUpdates.Sort((a, b) =>
        {
            double distA = Vector3D.DistanceSquared(ToDoubleVec(a.Pos), _lastViewPos);
            double distB = Vector3D.DistanceSquared(ToDoubleVec(b.Pos), _lastViewPos);
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < _lightingUpdates.Count; i++)
        {
            ChunkToMeshInfo update = _lightingUpdates[i];

            if (!IsChunkInRenderDistance(update.Pos, _lastViewPos))
            {
                _lightingUpdates.RemoveAt(i);
                i--;
                continue;
            }

            _meshGenerator.MeshChunk(_world, update.Pos, update.Version);
            _lightingUpdates.RemoveAt(i);
            return;
        }
    }

    public void UpdateAllRenderers()
    {
        foreach (SubChunkState state in _renderers.Values)
        {
            if (IsChunkInRenderDistance(state.Renderer.Position, _lastViewPos) && state.IsLit)
            {
                if (!_chunkVersions.TryGetValue(state.Renderer.Position, out ChunkMeshVersion? version))
                {
                    version = ChunkMeshVersion.Get();
                    _chunkVersions[state.Renderer.Position] = version;
                }

                version.MarkDirty();

                long? snapshot = version.SnapshotIfNeeded();
                if (snapshot.HasValue)
                {
                    _lightingUpdates.Add(new(state.Renderer.Position, snapshot.Value, false));
                }
            }
        }
    }

    public void Tick(Vector3D<double> viewPos)
    {
        Profiler.Start("WorldRenderer.Tick");

        _lastViewPos = viewPos;

        Vector3D<int> currentChunk = new(
            (int)Math.Floor(viewPos.X / SubChunkRenderer.Size),
            (int)Math.Floor(viewPos.Y / SubChunkRenderer.Size),
            (int)Math.Floor(viewPos.Z / SubChunkRenderer.Size)
        );

        int radiusSq = _lastRenderDistance * _lastRenderDistance;
        int enqueuedCount = 0;
        bool priorityPassClean = true;

        //TODO: MAKE THESE CONFIGURABLE
        const int MAX_CHUNKS_PER_FRAME = 32;
        const int PRIORITY_PASS_LIMIT = 1024;
        const int BACKGROUND_PASS_LIMIT = 2048;

        for (int i = 0; i < PRIORITY_PASS_LIMIT && i < s_spiralOffsets.Length; i++)
        {
            Vector3D<int> offset = s_spiralOffsets[i];
            int distSq = offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z;

            if (distSq > radiusSq)
                break;

            Vector3D<int> chunkPos = (currentChunk + offset) * SubChunkRenderer.Size;

            if (chunkPos.Y < 0 || chunkPos.Y >= 128)
                continue;

            if (_renderers.ContainsKey(chunkPos) || _chunkVersions.ContainsKey(chunkPos))
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
                Vector3D<int> offset = s_spiralOffsets[_currentIndex];
                int distSq = offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z;

                if (distSq <= radiusSq)
                {
                    Vector3D<int> chunkPos = (currentChunk + offset) * SubChunkRenderer.Size;
                    if (!_renderers.ContainsKey(chunkPos) && !_chunkVersions.ContainsKey(chunkPos))
                    {
                        if (MarkDirty(chunkPos))
                        {
                            enqueuedCount++;
                        }
                    }
                }

                _currentIndex = (_currentIndex + 1) % s_spiralOffsets.Length;

                if (enqueuedCount >= MAX_CHUNKS_PER_FRAME)
                    break;
            }
        }

        Profiler.Start("WorldRenderer.Tick.RemoveVersions");
        foreach (KeyValuePair<Vector3D<int>, ChunkMeshVersion> version in _chunkVersions)
        {
            if (!IsChunkInRenderDistance(version.Key, _lastViewPos))
            {
                _chunkVersionsToRemove.Add(version.Key);
            }
        }

        foreach (Vector3D<int> pos in _chunkVersionsToRemove)
        {
            _chunkVersions[pos].Release();
            _chunkVersions.Remove(pos);
        }

        _chunkVersionsToRemove.Clear();
        Profiler.Stop("WorldRenderer.Tick.RemoveVersions");

        Profiler.Stop("WorldRenderer.Tick");
    }

    public bool MarkDirty(Vector3D<int> chunkPos, bool priority = false)
    {
        if (!_world.isRegionLoaded(chunkPos.X - 1, chunkPos.Y - 1, chunkPos.Z - 1, chunkPos.X + SubChunkRenderer.Size + 1, chunkPos.Y + SubChunkRenderer.Size + 1, chunkPos.Z + SubChunkRenderer.Size + 1) | !IsChunkInRenderDistance(chunkPos, _lastViewPos))
            return false;

        if (!_chunkVersions.TryGetValue(chunkPos, out ChunkMeshVersion? version))
        {
            version = ChunkMeshVersion.Get();
            _chunkVersions[chunkPos] = version;
        }
        version.MarkDirty();

        long? snapshot = version.SnapshotIfNeeded();
        if (snapshot.HasValue)
        {
            for (int i = 0; i < _dirtyChunks.Count; i++)
            {
                if (_dirtyChunks[i].Pos == chunkPos)
                {
                    _dirtyChunks[i] = new(chunkPos, snapshot.Value, priority || _dirtyChunks[i].priority);
                    return true;
                }
            }

            _dirtyChunks.Add(new(chunkPos, snapshot.Value, priority));
            return true;
        }

        return false;
    }

    public RenderRegion? GetRegionForRenderer(SubChunkRenderer renderer)
    {
        Vector3D<int> regionPos = GetRegionPos(renderer.Position);
        _regions.TryGetValue(regionPos, out var region);
        return region;
    }

    private RenderRegion GetOrCreateRegion(Vector3D<int> pos)
    {
        Vector3D<int> regionPos = GetRegionPos(pos);
        if (!_regions.TryGetValue(regionPos, out var region))
        {
            region = new RenderRegion(regionPos);
            _regions[regionPos] = region;
        }
        return region;
    }

    private Vector3D<int> GetRegionPos(Vector3D<int> pos)
    {
        // Region size: 8x4x8 sections. Section size: 16.
        // Region size in blocks: 128x64x128.
        int rx = (int)Math.Floor(pos.X / 128.0) * 128;
        int ry = (int)Math.Floor(pos.Y / 64.0) * 64;
        int rz = (int)Math.Floor(pos.Z / 128.0) * 128;
        return new Vector3D<int>(rx, ry, rz);
    }

    private int GetSectionIndex(Vector3D<int> pos)
    {
        int sx = (pos.X / 16) % 8;
        int sy = (pos.Y / 16) % 4;
        int sz = (pos.Z / 16) % 8;
        
        // Handle negative modulo
        if (sx < 0) sx += 8;
        if (sy < 0) sy += 4;
        if (sz < 0) sz += 8;
        
        return sx + sy * 8 + sz * (8 * 4);
    }

    private bool IsChunkInRenderDistance(Vector3D<int> chunkWorldPos, Vector3D<double> viewPos)
    {
        int chunkX = chunkWorldPos.X / SubChunkRenderer.Size;
        int chunkZ = chunkWorldPos.Z / SubChunkRenderer.Size;

        int viewChunkX = (int)Math.Floor(viewPos.X / SubChunkRenderer.Size);
        int viewChunkZ = (int)Math.Floor(viewPos.Z / SubChunkRenderer.Size);

        int dist = Vector2D.Distance(new Vector2D<int>(chunkX, chunkZ), new Vector2D<int>(viewChunkX, viewChunkZ));
        bool isIn = dist <= _lastRenderDistance;
        return isIn;
    }

    private static Vector3D<double> ToDoubleVec(Vector3D<int> vec) => new(vec.X, vec.Y, vec.Z);

    public void Dispose()
    {
        foreach (SubChunkState state in _renderers.Values)
        {
            state.Renderer.Dispose();
        }

        _chunkShader.Dispose();

        _renderers.Clear();

        _translucentRenderers.Clear();
        _renderersToRemove.Clear();
        
        foreach (var region in _regions.Values)
        {
            region.Dispose();
        }
        _regions.Clear();

        foreach (ChunkMeshVersion version in _chunkVersions.Values)
        {
            version.Release();
        }
        _chunkVersions.Clear();
    }
}
