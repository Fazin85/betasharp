namespace BetaSharp.Client.Rendering.Entities.Models.FBX.Format;

// Vertex complet avec position, UV, normale et poids d'os
public struct FbxVertex
{
    public float X, Y, Z;       // position
    public float U, V;          // UV
    public float NX, NY, NZ;    // normale
    // pour le skinning futur
    public int BoneIndex;
    public float BoneWeight;
}

// Triangle (FBX travaille en triangles, pas en quads)
public struct FbxTriangle
{
    public int V0, V1, V2; // indices dans le tableau de vertices
}

// Un mesh = une partie du modèle (bras, tête, etc.)
public class FbxMesh
{
    public string Name { get; set; }
    public FbxVertex[] Vertices { get; set; }
    public FbxTriangle[] Triangles { get; set; }

    // Pivot de rotation — équivalent de rotationPoint
    public float PivotX, PivotY, PivotZ;
    // Rotation courante — modifiable à l'instance pour les animations
    public float RotateX, RotateY, RotateZ;

    public bool Visible { get; set; } = true;
}
