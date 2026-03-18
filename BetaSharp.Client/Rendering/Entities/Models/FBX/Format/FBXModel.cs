using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities.Models.FBX.Format;

public class FbxModel
{
    public List<FbxMesh> Meshes { get; } = new();
    private Dictionary<string, uint> vaos = new(); // un VAO par mesh

    // Charge depuis un fichier .fbx
    public static FbxModel load(string path)
    {
        return FbxLoader.load(path);
    }

    // Compile les VAOs une seule fois
    public void compile(GL gl)
    {
        foreach (FbxMesh mesh in Meshes)
        {
            if (vaos.ContainsKey(mesh.Name)) continue;

            // Interleave vertices : X Y Z U V NX NY NZ
            float[] vboData = new float[mesh.Vertices.Length * 8];
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                FbxVertex v = mesh.Vertices[i];
                vboData[i * 8 + 0] = v.X; vboData[i * 8 + 1] = v.Y; vboData[i * 8 + 2] = v.Z;
                vboData[i * 8 + 3] = v.U; vboData[i * 8 + 4] = v.V;
                vboData[i * 8 + 5] = v.NX; vboData[i * 8 + 6] = v.NY; vboData[i * 8 + 7] = v.NZ;
            }

            uint[] indices = new uint[mesh.Triangles.Length * 3];
            for (int i = 0; i < mesh.Triangles.Length; i++)
            {
                indices[i * 3 + 0] = (uint)mesh.Triangles[i].V0;
                indices[i * 3 + 1] = (uint)mesh.Triangles[i].V1;
                indices[i * 3 + 2] = (uint)mesh.Triangles[i].V2;
            }

            uint vao = gl.GenVertexArray();
            uint vbo = gl.GenBuffer();
            uint ebo = gl.GenBuffer();

            gl.BindVertexArray(vao);

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData(BufferTargetARB.ArrayBuffer, vboData, BufferUsageARB.StaticDraw);

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

            int stride = 8 * sizeof(float);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, 0);               // position
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)stride, 3 * sizeof(float)); // UV
            gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)stride, 5 * sizeof(float)); // normale
            gl.EnableVertexAttribArray(0);
            gl.EnableVertexAttribArray(1);
            gl.EnableVertexAttribArray(2);

            gl.BindVertexArray(0);
            vaos[mesh.Name] = vao;
        }
    }

    // Rendu d'un mesh avec sa transformation courante
    public void renderMesh(GL gl, FbxMesh mesh)
    {
        if (!mesh.Visible || !vaos.ContainsKey(mesh.Name)) return;

        gl.PushMatrix();
        gl.Translate(mesh.PivotX, mesh.PivotY, mesh.PivotZ);
        if (mesh.RotateZ != 0) gl.Rotate(mesh.RotateZ * (180f / MathF.PI), 0, 0, 1);
        if (mesh.RotateY != 0) gl.Rotate(mesh.RotateY * (180f / MathF.PI), 0, 1, 0);
        if (mesh.RotateX != 0) gl.Rotate(mesh.RotateX * (180f / MathF.PI), 1, 0, 0);

        gl.BindVertexArray(vaos[mesh.Name]);
        gl.DrawElements(PrimitiveType.Triangles,
            (uint)(mesh.Triangles.Length * 3),
            DrawElementsType.UnsignedInt, 0);
        gl.BindVertexArray(0);
        gl.PopMatrix();
    }

    public void render(GL gl)
    {
        foreach (FbxMesh mesh in Meshes)
            renderMesh(gl, mesh);
    }
}
