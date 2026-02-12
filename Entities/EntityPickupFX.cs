using betareborn.Client.Rendering.Core;
using betareborn.Client.Rendering.Entitys;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityPickupFX : EntityFX
    {

        private Entity target;
        private Entity source;
        private int currentAge = 0;
        private int maxAge = 0;
        private float yOffset;

        public EntityPickupFX(World world, Entity target, Entity source, float yOffset) : base(world, target.x, target.y, target.z, target.velocityX, target.velocityY, target.velocityZ)
        {
            this.target = target;
            this.source = source;
            this.yOffset = yOffset;
            maxAge = 3;
        }

        public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
        {
            float var8 = ((float)field_678_p + var2) / (float)field_677_q;
            var8 *= var8;
            double var9 = field_675_a.x;
            double var11 = field_675_a.y;
            double var13 = field_675_a.z;
            double var15 = field_679_o.lastTickX + (field_679_o.x - field_679_o.lastTickX) * (double)var2;
            double var17 = field_679_o.lastTickY + (field_679_o.y - field_679_o.lastTickY) * (double)var2 + (double)field_676_r;
            double var19 = field_679_o.lastTickZ + (field_679_o.z - field_679_o.lastTickZ) * (double)var2;
            double var21 = var9 + (var15 - var9) * (double)var8;
            double var23 = var11 + (var17 - var11) * (double)var8;
            double var25 = var13 + (var19 - var13) * (double)var8;
            int var27 = MathHelper.floor_double(var21);
            int var28 = MathHelper.floor_double(var23 + (double)(standingEyeHeight / 2.0F));
            int var29 = MathHelper.floor_double(var25);
            float var30 = world.GetLuminance(var27, var28, var29);
            var21 -= interpPosX;
            var23 -= interpPosY;
            var25 -= interpPosZ;
            GLManager.GL.Color4(var30, var30, var30, 1.0F);
            EntityRenderDispatcher.instance.renderEntityWithPosYaw(field_675_a, (double)((float)var21), (double)((float)var23), (double)((float)var25), field_675_a.yaw, var2);
        }

        public override void tick()
        {
            ++currentAge;
            if (currentAge == maxAge)
            {
                markDead();
            }

        }

        public override int getFXLayer()
        {
            return 3;
        }
    }

}