using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Textures;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Entities.FX;

public class EntityDiggingFX : EntityFX
{

    private readonly Block targetedBlock;
    private readonly int hitFace;
    private UVRegion _uvRegion;

    public EntityDiggingFX(World world, double x, double y, double z, double velocityX, double velocityY, double velocityZ, Block targetedBlock, int hitFace, int meta) : base(world, x, y, z, velocityX, velocityY, velocityZ)
    {
        this.targetedBlock = targetedBlock;
        _uvRegion = TextureAtlasManager.Instance.Terrain.GetUV(this.targetedBlock.textureId);
        particleGravity = targetedBlock.particleFallSpeedModifier;
        particleRed = particleGreen = particleBlue = 0.6F;
        particleScale /= 2.0F;
        this.hitFace = hitFace;
    }

    public EntityDiggingFX func_4041_a(int x, int y, int z)
    {
        if (targetedBlock == Block.GrassBlock)
        {
            return this;
        }
        else
        {
            int color = targetedBlock.getColorMultiplier(world, x, y, z);
            particleRed *= (float)(color >> 16 & 255) / 255.0F;
            particleGreen *= (float)(color >> 8 & 255) / 255.0F;
            particleBlue *= (float)(color & 255) / 255.0F;
            return this;
        }
    }

    public override int getFXLayer()
    {
        return 1;
    }

    public override void renderParticle(
        Tessellator t, float partialTick,
        float rotX, float rotY, float rotZ,
        float upX, float upZ)
    {
        // Taille d'une tuile dans l'atlas (1/4 de la région = sous-pixel jitter)
        float tileW = _uvRegion.U1 - _uvRegion.U0;
        float tileH = _uvRegion.V1 - _uvRegion.V0;

        // Le jitter original était X/4 sur une grille de 16 cases → X/4 * tileW
        float minU = _uvRegion.U0 + (particleTextureJitterX / 4.0F) * tileW;
        float maxU = minU + 0.999F * tileW / 4.0F;
        float minV = _uvRegion.V0 + (particleTextureJitterY / 4.0F) * tileH;
        float maxV = minV + 0.999F * tileH / 4.0F;

        float size = 0.1F * particleScale;
        float renderX = (float)(prevX + (x - prevX) * (double)partialTick - interpPosX);
        float renderY = (float)(prevY + (y - prevY) * (double)partialTick - interpPosY);
        float renderZ = (float)(prevZ + (z - prevZ) * (double)partialTick - interpPosZ);

        float brightness = getBrightnessAtEyes(partialTick);
        t.setColorOpaque_F(brightness * particleRed, brightness * particleGreen, brightness * particleBlue);

        t.addVertexWithUV(renderX - rotX * size - upX * size, renderY - rotY * size, renderZ - rotZ * size - upZ * size, minU, maxV);
        t.addVertexWithUV(renderX - rotX * size + upX * size, renderY + rotY * size, renderZ - rotZ * size + upZ * size, minU, minV);
        t.addVertexWithUV(renderX + rotX * size + upX * size, renderY + rotY * size, renderZ + rotZ * size + upZ * size, maxU, minV);
        t.addVertexWithUV(renderX + rotX * size - upX * size, renderY - rotY * size, renderZ + rotZ * size - upZ * size, maxU, maxV);
    }
}
