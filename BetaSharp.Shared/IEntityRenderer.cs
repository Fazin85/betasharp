namespace BetaSharp.Shared
{
    public interface IEntityRenderer
    {
        void Render(object entity, double x, double y, double z,
            float yaw, float partialTicks);
    }


}
