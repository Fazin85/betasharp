using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityDiggingFX : EntityFX
{
    private readonly int hitFace;

    private readonly Block targetedBlock;

    public EntityDiggingFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ, Block targetedBlock, int hitFace, int meta) : base(world, x, y, z, velocityX, velocityY, velocityZ)
    {
        this.targetedBlock = targetedBlock;
        particleTextureIndex = targetedBlock.getTexture(0, meta);
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

        int color = targetedBlock.getColorMultiplier(_level.BlocksReader, x, y, z);
        particleRed *= ((color >> 16) & 255) / 255.0F;
        particleGreen *= ((color >> 8) & 255) / 255.0F;
        particleBlue *= (color & 255) / 255.0F;
        return this;
    }

    public override int getFXLayer() => 1;

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
