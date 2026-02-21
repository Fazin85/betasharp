using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public struct VertexData
{
    public float X, Y, Z;
    public float R, G, B, A;
    public float U, V;
    public float NX, NY, NZ;
}

public class ImmediateGeometryBuffer
{
    private readonly GL _gl;
    private readonly List<VertexData> _vertices = new(4096);
    private GLEnum _currentMode;
    private VertexData _activeState;

    public class DisplayList
    {
        public uint Vao;
        public uint Vbo;
        public int VertexCount;
        public GLEnum DrawMode;
    }

    private readonly Dictionary<uint, List<DisplayList>> _compiledLists = new();

    public ImmediateGeometryBuffer(GL gl)
    {
        _gl = gl;
        _activeState = new VertexData { R = 1, G = 1, B = 1, A = 1 };
    }

    public void Begin(GLEnum mode)
    {
        _currentMode = mode;
        _vertices.Clear();
    }

    public void Color(float r, float g, float b, float a = 1.0f)
    {
        _activeState.R = r;
        _activeState.G = g;
        _activeState.B = b;
        _activeState.A = a;
    }

    public void TexCoord(float u, float v)
    {
        _activeState.U = u;
        _activeState.V = v;
    }

    public void Normal(float nx, float ny, float nz)
    {
        _activeState.NX = nx;
        _activeState.NY = ny;
        _activeState.NZ = nz;
    }

    public void Vertex(float x, float y, float z)
    {
        _activeState.X = x;
        _activeState.Y = y;
        _activeState.Z = z;
        _vertices.Add(_activeState);
    }

    public unsafe DisplayList End()
    {
        if (_vertices.Count == 0) return null;

        uint vao = _gl.GenVertexArray();
        uint vbo = _gl.GenBuffer();

        _gl.BindVertexArray(vao);
        _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);

        var span = CollectionsMarshal.AsSpan(_vertices);
        fixed (VertexData* d = span)
        {
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)(_vertices.Count * sizeof(VertexData)), d, GLEnum.StaticDraw);
        }

        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)sizeof(VertexData), (void*)0);

        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(1, 4, GLEnum.Float, false, (uint)sizeof(VertexData), (void*)(3 * sizeof(float)));

        _gl.EnableVertexAttribArray(2);
        _gl.VertexAttribPointer(2, 2, GLEnum.Float, false, (uint)sizeof(VertexData), (void*)(7 * sizeof(float)));

        _gl.EnableVertexAttribArray(3);
        _gl.VertexAttribPointer(3, 3, GLEnum.Float, false, (uint)sizeof(VertexData), (void*)(9 * sizeof(float)));

        _gl.BindVertexArray(0);

        var list = new DisplayList
        {
            Vao = vao,
            Vbo = vbo,
            VertexCount = _vertices.Count,
            DrawMode = _currentMode
        };

        _vertices.Clear();
        return list;
    }

    public void DrawList(DisplayList list)
    {
        if (list == null) return;
        _gl.BindVertexArray(list.Vao);
        _gl.DrawArrays(list.DrawMode, 0, (uint)list.VertexCount);
        _gl.BindVertexArray(0);
    }

    public void DeleteList(DisplayList list)
    {
        if (list == null) return;
        _gl.DeleteBuffer(list.Vbo);
        _gl.DeleteVertexArray(list.Vao);
    }

    public void SaveDisplayListChunk(uint listId, DisplayList chunk)
    {
        if (chunk == null) return;
        if (!_compiledLists.ContainsKey(listId))
        {
            _compiledLists[listId] = new List<DisplayList>();
        }
        _compiledLists[listId].Add(chunk);
    }

    public void CallDisplayList(uint listId)
    {
        if (_compiledLists.TryGetValue(listId, out var chunks))
        {
            foreach (var chunk in chunks)
            {
                DrawList(chunk);
            }
        }
    }

    public void DeleteDisplayList(uint listId)
    {
        if (_compiledLists.TryGetValue(listId, out var chunks))
        {
            foreach (var chunk in chunks)
            {
                DeleteList(chunk);
            }
            _compiledLists.Remove(listId);
        }
    }
}
