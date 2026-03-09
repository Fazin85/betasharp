using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntitySplashFX : EntityRainFX
{
    public EntitySplashFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : base(world, x, y, z)
    {
        particleGravity = 0.04F;
        ++particleTextureIndex;
        if (velocityY == 0.0 && (velocityX != 0.0 || velocityZ != 0.0))
        {
            this.velocityX = velocityX;
            this.velocityY = velocityY + 0.1;
            this.velocityZ = velocityZ;
        }
    }
}
