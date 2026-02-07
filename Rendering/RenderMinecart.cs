using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Models;

namespace betareborn.Rendering
{
    public class RenderMinecart : Render
    {

        protected ModelBase modelMinecart;

        public RenderMinecart()
        {
            shadowSize = 0.5F;
            modelMinecart = new ModelMinecart();
        }

        public void func_152_a(EntityMinecart var1, double var2, double var4, double var6, float var8, float var9)
        {
            GLManager.GL.PushMatrix();
            double var10 = var1.lastTickPosX + (var1.posX - var1.lastTickPosX) * (double)var9;
            double var12 = var1.lastTickPosY + (var1.posY - var1.lastTickPosY) * (double)var9;
            double var14 = var1.lastTickPosZ + (var1.posZ - var1.lastTickPosZ) * (double)var9;
            double var16 = (double)0.3F;
            Vec3D var18 = var1.func_514_g(var10, var12, var14);
            float var19 = var1.prevRotationPitch + (var1.rotationPitch - var1.prevRotationPitch) * var9;
            if (var18 != null)
            {
                Vec3D var20 = var1.func_515_a(var10, var12, var14, var16);
                Vec3D var21 = var1.func_515_a(var10, var12, var14, -var16);
                if (var20 == null)
                {
                    var20 = var18;
                }

                if (var21 == null)
                {
                    var21 = var18;
                }

                var2 += var18.xCoord - var10;
                var4 += (var20.yCoord + var21.yCoord) / 2.0D - var12;
                var6 += var18.zCoord - var14;
                Vec3D var22 = var21.addVector(-var20.xCoord, -var20.yCoord, -var20.zCoord);
                if (var22.lengthVector() != 0.0D)
                {
                    var22 = var22.normalize();
                    var8 = (float)(java.lang.Math.atan2(var22.zCoord, var22.xCoord) * 180.0D / Math.PI);
                    var19 = (float)(java.lang.Math.atan(var22.yCoord) * 73.0D);
                }
            }

            GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
            GLManager.GL.Rotate(180.0F - var8, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-var19, 0.0F, 0.0F, 1.0F);
            float var23 = (float)var1.minecartTimeSinceHit - var9;
            float var24 = (float)var1.minecartCurrentDamage - var9;
            if (var24 < 0.0F)
            {
                var24 = 0.0F;
            }

            if (var23 > 0.0F)
            {
                GLManager.GL.Rotate(MathHelper.sin(var23) * var23 * var24 / 10.0F * (float)var1.minecartRockDirection, 1.0F, 0.0F, 0.0F);
            }

            if (var1.minecartType != 0)
            {
                loadTexture("/terrain.png");
                float var25 = 12.0F / 16.0F;
                GLManager.GL.Scale(var25, var25, var25);
                GLManager.GL.Translate(0.0F, 5.0F / 16.0F, 0.0F);
                GLManager.GL.Rotate(90.0F, 0.0F, 1.0F, 0.0F);
                //TODO: WTF WHY ARE WE MAKING A NEW RENDER BLOCKS EVERY TIME
                if (var1.minecartType == 1)
                {
                    (new RenderBlocks()).renderBlockOnInventory(Block.CHEST, 0, var1.getEntityBrightness(var9));
                }
                else if (var1.minecartType == 2)
                {
                    (new RenderBlocks()).renderBlockOnInventory(Block.FURNACE, 0, var1.getEntityBrightness(var9));
                }

                GLManager.GL.Rotate(-90.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Translate(0.0F, -(5.0F / 16.0F), 0.0F);
                GLManager.GL.Scale(1.0F / var25, 1.0F / var25, 1.0F / var25);
            }

            loadTexture("/item/cart.png");
            GLManager.GL.Scale(-1.0F, -1.0F, 1.0F);
            modelMinecart.render(0.0F, 0.0F, -0.1F, 0.0F, 0.0F, 1.0F / 16.0F);
            GLManager.GL.PopMatrix();
        }

        public override void doRender(Entity var1, double var2, double var4, double var6, float var8, float var9)
        {
            func_152_a((EntityMinecart)var1, var2, var4, var6, var8, var9);
        }
    }

}