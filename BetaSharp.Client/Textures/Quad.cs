using BetaSharp.Client.Rendering.Core;
using Silk.NET.Maths;

namespace BetaSharp.Client.Textures;

public class Quad
{
    public PositionTextureVertex[] vertexPositions;
    public int nVertices;
    private readonly bool invertNormal;

    public Quad(PositionTextureVertex[] var1)
    {
        nVertices = 0;
        invertNormal = false;
        vertexPositions = var1;
        nVertices = var1.Length;
    }

    public Quad(PositionTextureVertex[] var1, int var2, int var3, int var4, int var5) : this(var1)
    {
        float var6 = 0.0015625F;
        float var7 = 0.003125F;
        var1[0] = var1[0].setTexturePosition(var4 / 64.0F - var6, var3 / 32.0F + var7);
        var1[1] = var1[1].setTexturePosition(var2 / 64.0F + var6, var3 / 32.0F + var7);
        var1[2] = var1[2].setTexturePosition(var2 / 64.0F + var6, var5 / 32.0F - var7);
        var1[3] = var1[3].setTexturePosition(var4 / 64.0F - var6, var5 / 32.0F - var7);
    }

    public void flipFace()
    {
        PositionTextureVertex[] var1 = new PositionTextureVertex[vertexPositions.Length];

        for (int var2 = 0; var2 < vertexPositions.Length; ++var2)
        {
            var1[var2] = vertexPositions[vertexPositions.Length - var2 - 1];
        }

        vertexPositions = var1;
    }

    public void draw(Tessellator var1, float var2)
    {
        Vector3D<double> var3 = vertexPositions[1].vector3D - vertexPositions[0].vector3D;
        Vector3D<double> var4 = vertexPositions[1].vector3D - vertexPositions[2].vector3D;
        Vector3D<double> var5 = Vector3D.Normalize(Vector3D.Cross(var4, var3));
        var1.startDrawingQuads();
        if (invertNormal)
        {
            var1.setNormal(-(float)var5.X, -(float)var5.Y, -(float)var5.Z);
        }
        else
        {
            var1.setNormal((float)var5.X, (float)var5.Y, (float)var5.Z);
        }

        for (int var6 = 0; var6 < 4; ++var6)
        {
            PositionTextureVertex var7 = vertexPositions[var6];
            var1.addVertexWithUV((double)((float)var7.vector3D.X * var2), (double)((float)var7.vector3D.Y * var2), (double)((float)var7.vector3D.Z * var2), var7.texturePositionX, var7.texturePositionY);
        }

        var1.draw();
    }
}
