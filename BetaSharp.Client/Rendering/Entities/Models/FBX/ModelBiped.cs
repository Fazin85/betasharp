using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models.FBX.Format;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities.Models.FBX;

public class FbxModelBiped : ModelBase
{
    private FbxModel model;

    public FbxMesh head => model.Meshes.FirstOrDefault(m => m.Name == "Head");
    public FbxMesh body => model.Meshes.FirstOrDefault(m => m.Name == "Body");
    public FbxMesh armR => model.Meshes.FirstOrDefault(m => m.Name == "ArmRight");
    public FbxMesh armL => model.Meshes.FirstOrDefault(m => m.Name == "ArmLeft");
    public FbxMesh legR => model.Meshes.FirstOrDefault(m => m.Name == "LegRight");
    public FbxMesh legL => model.Meshes.FirstOrDefault(m => m.Name == "LegLeft");

    public FbxModelBiped(string fbxPath, GL gl)
    {
        model = FbxModel.load(fbxPath);
        model.compile(gl);
    }

    public override void setRotationAngles(float limbSwing, float limbSwingAmount,
        float ageInTicks, float yaw, float pitch, float scale)
    {
        if (head != null)
        {
            head.RotateY = yaw / (180f / MathF.PI);
            head.RotateX = pitch / (180f / MathF.PI);
        }
        if (armR != null)
            armR.RotateX = MathF.Cos(limbSwing * 0.6662f + MathF.PI) * 2f * limbSwingAmount * 0.5f;
        if (armL != null)
            armL.RotateX = MathF.Cos(limbSwing * 0.6662f) * 2f * limbSwingAmount * 0.5f;
        if (legR != null)
            legR.RotateX = MathF.Cos(limbSwing * 0.6662f) * 1.4f * limbSwingAmount;
        if (legL != null)
            legL.RotateX = MathF.Cos(limbSwing * 0.6662f + MathF.PI) * 1.4f * limbSwingAmount;
    }

    public override void render(float v1, float v2, float v3, float v4, float v5, float scale)
    {
        setRotationAngles(v1, v2, v3, v4, v5, scale);
        GL igl = GLManager.GL.GetGl();
        model.render(igl);
    }
}
