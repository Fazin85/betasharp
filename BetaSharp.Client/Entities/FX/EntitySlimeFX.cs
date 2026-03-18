using System.Net;
using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Textures;
using BetaSharp.Items;
using BetaSharp.Worlds;
using static com.sun.management.VMOption;

namespace BetaSharp.Client.Entities.FX;

public class EntitySlimeFX : EntityFX
{
    // Remplace particleTextureIndex (int legacy) par la région UV dans l'atlas
    private UVRegion _uvRegion;

    public EntitySlimeFX(World world, double x, double y, double z, Item item)
        : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        _uvRegion = TextureAtlasManager.Instance.Items.GetUV(item.getTextureId(0));

        particleRed = particleGreen = particleBlue = 1.0F;
        particleGravity = Block.SnowBlock.particleFallSpeedModifier;
        particleScale /= 2.0F;
    }

    public override int getFXLayer() => 2;

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
