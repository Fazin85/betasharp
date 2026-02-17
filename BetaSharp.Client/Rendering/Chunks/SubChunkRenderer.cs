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
    public static int Size = 16;
    public bool HasTranslucentMesh => vertexCounts[1] > 0;
    public Vector3D<int> Position { get; }
    public Vector3D<int> PositionPlus { get; }
    public Vector3D<int> PositionMinus { get; }
    public Vector3D<int> ClipPosition { get; }
    public Box BoundingBox { get; }

    private readonly VertexBuffer<ChunkVertex>[] vertexBuffers = new VertexBuffer<ChunkVertex>[2];
    private readonly VertexArray[] vertexArrays = new VertexArray[2];
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

    public void UploadMeshData(PooledList<ChunkVertex>? solidMesh, PooledList<ChunkVertex>? translucentMesh)
    {
        vertexCounts[0] = 0;
        vertexCounts[1] = 0;

        if (solidMesh != null)
        {
            if (solidMesh.Count > 0)
            {
                Span<ChunkVertex> solidMeshData = solidMesh.Span;
                UploadMesh(vertexBuffers, 0, solidMeshData);
            }

            solidMesh.Dispose();
        }

        if (translucentMesh != null)
        {
            if (translucentMesh.Count > 0)
            {
                Span<ChunkVertex> translucentMeshData = translucentMesh.Span;
                UploadMesh(vertexBuffers, 1, translucentMeshData);
            }

            translucentMesh.Dispose();
        }
    }

    private unsafe void UploadMesh(VertexBuffer<ChunkVertex>[] buffers, int bufferIdx, Span<ChunkVertex> meshData)
    {
        if (buffers[bufferIdx] == null)
        {
            buffers[bufferIdx] = new(meshData);
        }
        else
        {
            buffers[bufferIdx].BufferData(meshData);
        }

        vertexCounts[bufferIdx] = meshData.Length;

        if (vertexArrays[bufferIdx] == null)
        {
            vertexArrays[bufferIdx] = new();
            vertexArrays[bufferIdx].Bind();
            buffers[bufferIdx].Bind();

            const uint stride = 16;

            GLManager.GL.EnableVertexAttribArray(0);
            GLManager.GL.VertexAttribPointer(
                0,
                3,
                GLEnum.Short,
                false,
                stride,
                (void*)4
            );

            GLManager.GL.EnableVertexAttribArray(1);
            GLManager.GL.VertexAttribIPointer(
                1,
                2,
                GLEnum.UnsignedShort,
                stride,
                (void*)10
            );

            GLManager.GL.EnableVertexAttribArray(2);
            GLManager.GL.VertexAttribPointer(
                2,
                4,
                GLEnum.UnsignedByte,
                true,
                stride,
                (void*)0
            );

            GLManager.GL.EnableVertexAttribArray(3);
            GLManager.GL.VertexAttribIPointer(
                3,
                1,
                GLEnum.UnsignedByte,
                stride,
                (void*)14
            );

            VertexArray.Unbind();
        }
    }

    public void Render(Shader shader, int pass, Vector3D<double> viewPos, Matrix4X4<float> modelViewMatrix)
    {
        if (pass < 0 || pass > 1)
            throw new ArgumentException("Pass must be 0 or 1");

        int vertexCount = vertexCounts[pass];

        if (vertexCount == 0)
            return;

        Vector3D<double> pos = new(PositionMinus.X - viewPos.X, PositionMinus.Y - viewPos.Y, PositionMinus.Z - viewPos.Z);
        pos += new Vector3D<double>(ClipPosition.X, ClipPosition.Y, ClipPosition.Z);

        modelViewMatrix = Matrix4X4.CreateTranslation(new Vector3D<float>((float)pos.X, (float)pos.Y, (float)pos.Z)) * modelViewMatrix;

        shader.SetUniformMatrix4("modelViewMatrix", modelViewMatrix);
        shader.SetUniform2("chunkPos", Position.X, Position.Z);

        vertexArrays[pass].Bind();

        GLManager.GL.DrawArrays(GLEnum.Triangles, 0, (uint)vertexCount);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        GC.SuppressFinalize(this);

        vertexBuffers[0]?.Dispose();
        vertexBuffers[1]?.Dispose();

        vertexArrays[0]?.Dispose();
        vertexArrays[1]?.Dispose();

        disposed = true;
    }
}