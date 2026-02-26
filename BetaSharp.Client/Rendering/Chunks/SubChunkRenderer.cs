using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;
using Shader = BetaSharp.Client.Rendering.Core.Shader;
using VertexArray = BetaSharp.Client.Rendering.Core.VertexArray;

namespace BetaSharp.Client.Rendering.Chunks;

public class SubChunkRenderer : IDisposable
{
    public const int Size = 16;
    public bool HasTranslucentMesh => vertexCounts[1] > 0;
    public Vector3D<int> Position { get; }
    public Vector3D<int> PositionPlus { get; }
    public Vector3D<int> PositionMinus { get; }
    public Vector3D<int> ClipPosition { get; }
    public Box BoundingBox { get; }

    public float Age { get; private set; } = 0.0f;
    public bool HasFadedIn => Age >= FadeDuration;
    public const float FadeDuration = 1.0f;

    public Occlusion.ChunkVisibilityStore VisibilityData;
    public Occlusion.ChunkDirectionMask IncomingDirections;
    public int LastVisibleFrame = -1;

    public SubChunkRenderer? AdjacentDown;
    public SubChunkRenderer? AdjacentUp;
    public SubChunkRenderer? AdjacentNorth;
    public SubChunkRenderer? AdjacentSouth;
    public SubChunkRenderer? AdjacentWest;
    public SubChunkRenderer? AdjacentEast;

    private RenderRegion? _region;
    private int _regionIndex;
    private readonly int[] vertexCounts = new int[2];
    private bool disposed;

    public SubChunkRenderer(Vector3D<int> position)
    {
        Position = position;

        PositionPlus = new(position.X + Size / 2, position.Y + Size / 2, position.Z + Size / 2);
        ClipPosition = new(position.X & 1023, position.Y, position.Z & 1023);
        PositionMinus = position - ClipPosition;

        const float padding = 6.0f;

        BoundingBox = new Box
        (
            position.X - padding,
            position.Y - padding,
            position.Z - padding,
            position.X + Size + padding,
            position.Y + Size + padding,
            position.Z + Size + padding
        );

        vertexCounts[0] = 0;
        vertexCounts[1] = 0;
    }

    public void SetRegion(RenderRegion region, int index)
    {
        _region = region;
        _regionIndex = index;
    }

    public bool IsVisible(Culler camera, Vector3D<double> viewPos, float renderDistance)
    {
        if (!camera.isBoundingBoxInFrustum(BoundingBox)) return false;

        double dx = PositionPlus.X - viewPos.X;
        double dy = PositionPlus.Y - viewPos.Y;
        double dz = PositionPlus.Z - viewPos.Z;

        return (dx * dx + dz * dz) < (renderDistance * renderDistance) && Math.Abs(dy) < renderDistance;
    }

    public void UploadMeshData(PooledList<ChunkVertex>? solidMesh, PooledList<ChunkVertex>? translucentMesh, long version)
    {
        if (_region == null) return;

        vertexCounts[0] = 0;
        vertexCounts[1] = 0;

        if (solidMesh != null)
        {
            if (solidMesh.Count > 0)
            {
                _region.UploadSection(_regionIndex, 0, solidMesh.Span, version);
                vertexCounts[0] = solidMesh.Count;
            }
            else
            {
                _region.UploadSection(_regionIndex, 0, Span<ChunkVertex>.Empty, version);
            }
            solidMesh.Dispose();
        }

        if (translucentMesh != null)
        {
            if (translucentMesh.Count > 0)
            {
                _region.UploadSection(_regionIndex, 1, translucentMesh.Span, version);
                vertexCounts[1] = translucentMesh.Count;
            }
            else
            {
                _region.UploadSection(_regionIndex, 1, Span<ChunkVertex>.Empty, version);
            }
            translucentMesh.Dispose();
        }
    }

    public void Update(float deltaTime)
    {
        if (!HasFadedIn)
        {
            Age += deltaTime;
        }
    }

    public int GetVertexCount(int pass) => vertexCounts[pass];

    public int GetOffset(int pass, int index)
    {
        return _region?.GetOffset(pass, index) ?? -1;
    }

    public void Render(Shader shader, int pass, Vector3D<double> viewPos, Matrix4X4<float> modelViewMatrix)
    {
        if (pass < 0 || pass > 1)
            throw new ArgumentException("Pass must be 0 or 1");

        if (_region == null || vertexCounts[pass] == 0)
            return;

        int offset = _region.GetOffset(pass, _regionIndex);
        int vertexCount = vertexCounts[pass];

        if (offset == -1 || vertexCount == 0)
            return;

        Vector3D<int> origin = _region?.Origin ?? Position;
        Vector3D<double> pos = new(origin.X - viewPos.X, origin.Y - viewPos.Y, origin.Z - viewPos.Z);

        modelViewMatrix = Matrix4X4.CreateTranslation(new Vector3D<float>((float)pos.X, (float)pos.Y, (float)pos.Z)) * modelViewMatrix;

        shader.SetUniformMatrix4("modelViewMatrix", modelViewMatrix);
        shader.SetUniform3("chunkPos", (float)origin.X, (float)origin.Y, (float)origin.Z);

        _region?.Bind(pass);

        GLManager.GL.DrawArrays(GLEnum.Triangles, offset, (uint)vertexCount);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        GC.SuppressFinalize(this);

        // Region manages its own resources. We don't need to free anything here
        // other than marking as empty in region if we wanted to be thorough.
        _region?.UploadSection(_regionIndex, 0, Span<ChunkVertex>.Empty, -1);
        _region?.UploadSection(_regionIndex, 1, Span<ChunkVertex>.Empty, -1);

        disposed = true;
    }
}
