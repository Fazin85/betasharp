namespace BetaSharp.Shared
{
    public interface IModel
    {
    void Render(float limbSwing, float limbSwingAmount,
                float ageInTicks, float yaw, float pitch, float scale);
    }
}

