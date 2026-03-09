using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Items;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntitySlimeFX : EntityFX
{
    public EntitySlimeFX(IBlockWorldContext world, double x, double y, double z, Item item) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        particleTextureIndex = item.getTextureId(0);
        particleRed = particleGreen = particleBlue = 1.0F;
        particleGravity = Block.SnowBlock.particleFallSpeedModifier;
        particleScale /= 2.0F;
    }

    public override int getFXLayer() => 2;

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float minU = (particleTextureIndex % 16 + particleTextureJitterX / 4.0F) / 16.0F;
        float maxU = minU + 0.999F / 64.0F;
        float minV = (particleTextureIndex / 16 + particleTextureJitterY / 4.0F) / 16.0F;
        float maxV = minV + 0.999F / 64.0F;
        float size = 0.1F * particleScale;
        float renderX = (float)(prevX + (x - prevX) * partialTick - interpPosX);
        float renderY = (float)(prevY + (y - prevY) * partialTick - interpPosY);
        float renderZ = (float)(prevZ + (z - prevZ) * partialTick - interpPosZ);
        float brightness = getBrightnessAtEyes(partialTick);
        t.setColorOpaque_F(brightness * particleRed, brightness * particleGreen, brightness * particleBlue);
        t.addVertexWithUV(renderX - rotX * size - upX * size, renderY - rotY * size, renderZ - rotZ * size - upZ * size, minU, maxV);
        t.addVertexWithUV(renderX - rotX * size + upX * size, renderY + rotY * size, renderZ - rotZ * size + upZ * size, minU, minV);
        t.addVertexWithUV(renderX + rotX * size + upX * size, renderY + rotY * size, renderZ + rotZ * size + upZ * size, maxU, minV);
        t.addVertexWithUV(renderX + rotX * size - upX * size, renderY - rotY * size, renderZ + rotZ * size - upZ * size, maxU, maxV);
    }
}
