using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public class CameraController
{
    private readonly Minecraft _mc;
    private readonly float _thirdPersonDistance = 4.0F;
    private readonly float _frontThirdPersonDistance = 4.0F;
    public float ViewBob { get; private set; }
    public float LastViewBob { get; private set; }

    private float _prevThirdPersonDistance = 4.0F;
    private float _prevFrontThirdPersonDistance = 4.0F;
    private readonly float _thirdPersonYaw = 0.0F;
    private float _prevThirdPersonYaw;
    private readonly float _thirdPersonPitch = 0.0F;
    private float _prevThirdPersonPitch;
    private readonly float _cameraRoll = 0.0F;
    private float _prevCameraRoll;
    private readonly float _cameraRollAmount = 0.0F;
    private float _prevCameraRollAmount;

    public double CameraZoom { get; } = 1.0D;
    public double CameraYaw { get; } = 0.0D;
    public double CameraPitch { get; } = 0.0D;

    public CameraController(Minecraft mc)
    {
        _mc = mc;
    }

    public void UpdateCamera()
    {
        LastViewBob = ViewBob;
        _prevThirdPersonDistance = _thirdPersonDistance;
        _prevFrontThirdPersonDistance = _frontThirdPersonDistance;
        _prevThirdPersonYaw = _thirdPersonYaw;
        _prevThirdPersonPitch = _thirdPersonPitch;
        _prevCameraRoll = _cameraRoll;
        _prevCameraRollAmount = _cameraRollAmount;

        _mc.camera ??= _mc.player;

        float luminance = _mc.world.getLuminance(MathHelper.floor_double(_mc.camera.x), MathHelper.floor_double(_mc.camera.y), MathHelper.floor_double(_mc.camera.z));
        float renderDistFactor = (3 - _mc.options.renderDistance) / 3.0F;
        float targetBob = luminance * (1.0F - renderDistFactor) + renderDistFactor;
        ViewBob += (targetBob - ViewBob) * 0.1F;
    }

    public float GetFov(float tickDelta, bool isHand = false)
    {
        EntityLiving cameraEntity = _mc.camera;
        float fov = isHand ? 70.0F : (30.0F + _mc.options.fov * 90.0F);

        if (cameraEntity.isInFluid(Material.Water))
        {
            fov = 60.0F;
        }

        if (cameraEntity.health <= 0)
        {
            float deathTimeF = cameraEntity.deathTime + tickDelta;
            fov /= (1.0F - 500.0F / (deathTimeF + 500.0F)) * 2.0F + 1.0F;
        }

        return fov + _prevCameraRoll + (_cameraRoll - _prevCameraRoll) * tickDelta;
    }

    public void ApplyDamageTiltEffect(float tickDelta)
    {
        EntityLiving cameraEntity = _mc.camera;
        float hurtTimeF = cameraEntity.hurtTime - tickDelta;

        if (cameraEntity.health <= 0)
        {
            float deathTimeF = cameraEntity.deathTime + tickDelta;
            GLManager.GL.Rotate(40.0F - 8000.0F / (deathTimeF + 200.0F), 0.0F, 0.0F, 1.0F);
        }

        if (hurtTimeF >= 0.0F)
        {
            hurtTimeF /= cameraEntity.maxHurtTime;
            hurtTimeF = MathHelper.sin(hurtTimeF * hurtTimeF * hurtTimeF * hurtTimeF * (float)Math.PI);
            float attackedYaw = cameraEntity.attackedAtYaw;
            GLManager.GL.Rotate(-attackedYaw, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-hurtTimeF * 14.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(attackedYaw, 0.0F, 1.0F, 0.0F);
        }
    }

    public void ApplyViewBobbing(float tickDelta)
    {
        if (_mc.camera is EntityPlayer player)
        {
            float speedDelta = player.horizontalSpeed - player.prevHorizontalSpeed;
            float speed = -(player.horizontalSpeed + speedDelta * tickDelta);
            float bobAmount = player.prevStepBobbingAmount + (player.stepBobbingAmount - player.prevStepBobbingAmount) * tickDelta;
            float pitch = player.cameraPitch + (player.tilt - player.cameraPitch) * tickDelta;

            GLManager.GL.Translate(MathHelper.sin(speed * (float)Math.PI) * bobAmount * 0.5F, -Math.Abs(MathHelper.cos(speed * (float)Math.PI) * bobAmount), 0.0F);
            GLManager.GL.Rotate(MathHelper.sin(speed * (float)Math.PI) * bobAmount * 3.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(Math.Abs(MathHelper.cos(speed * (float)Math.PI - 0.2F) * bobAmount) * 5.0F, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(pitch, 1.0F, 0.0F, 0.0F);
        }
    }

    public void ApplyCameraTransform(float tickDelta)
    {
        EntityLiving cameraEntity = _mc.camera;
        float eyeHeightOffset = cameraEntity.standingEyeHeight - 1.62F;
        double x = cameraEntity.prevX + (cameraEntity.x - cameraEntity.prevX) * (double)tickDelta;
        double y = cameraEntity.prevY + (cameraEntity.y - cameraEntity.prevY) * (double)tickDelta - (double)eyeHeightOffset;
        double z = cameraEntity.prevZ + (cameraEntity.z - cameraEntity.prevZ) * (double)tickDelta;

        GLManager.GL.Rotate(_prevCameraRollAmount + (_cameraRollAmount - _prevCameraRollAmount) * tickDelta, 0.0F, 0.0F, 1.0F);

        if (cameraEntity.isSleeping())
        {
            eyeHeightOffset = (float)((double)eyeHeightOffset + 1.0D);
            GLManager.GL.Translate(0.0F, 0.3F, 0.0F);
            if (!_mc.options.debugCamera)
            {
                int blockId = _mc.world.getBlockId(MathHelper.floor_double(cameraEntity.x), MathHelper.floor_double(cameraEntity.y), MathHelper.floor_double(cameraEntity.z));
                if (blockId == Block.Bed.id)
                {
                    int meta = _mc.world.getBlockMeta(MathHelper.floor_double(cameraEntity.x), MathHelper.floor_double(cameraEntity.y), MathHelper.floor_double(cameraEntity.z));
                    int rotation = meta & 3;
                    GLManager.GL.Rotate(rotation * 90, 0.0F, 1.0F, 0.0F);
                }

                GLManager.GL.Rotate(cameraEntity.prevYaw + (cameraEntity.yaw - cameraEntity.prevYaw) * tickDelta + 180.0F, 0.0F, -1.0F, 0.0F);
                GLManager.GL.Rotate(cameraEntity.prevPitch + (cameraEntity.pitch - cameraEntity.prevPitch) * tickDelta, -1.0F, 0.0F, 0.0F);
            }
        }
        else if (_mc.options.cameraMode == EnumCameraMode.FrontThirdPerson || _mc.options.cameraMode == EnumCameraMode.ThirdPerson)
        {
            double currentDistance;
            if (_mc.options.cameraMode == EnumCameraMode.FrontThirdPerson)
            {
                currentDistance = _prevFrontThirdPersonDistance + (_frontThirdPersonDistance - _prevFrontThirdPersonDistance) * tickDelta;
            }
            else
            {
                currentDistance = _prevThirdPersonDistance + (_thirdPersonDistance - _prevThirdPersonDistance) * tickDelta;
            }

            float targetPitch;
            float targetYaw;

            if (_mc.options.debugCamera)
            {
                targetYaw = _prevThirdPersonYaw + (_thirdPersonYaw - _prevThirdPersonYaw) * tickDelta;
                targetPitch = _prevThirdPersonPitch + (_thirdPersonPitch - _prevThirdPersonPitch) * tickDelta;
                GLManager.GL.Translate(0.0F, 0.0F, (float)-currentDistance);
                GLManager.GL.Rotate(targetPitch, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(targetYaw, 0.0F, 1.0F, 0.0F);
            }
            else
            {
                targetYaw = cameraEntity.yaw;
                targetPitch = cameraEntity.pitch;

                double vecX = (double)(-MathHelper.sin(targetYaw / 180.0F * (float)Math.PI) * MathHelper.cos(targetPitch / 180.0F * (float)Math.PI)) * currentDistance;
                double vecZ = (double)(MathHelper.cos(targetYaw / 180.0F * (float)Math.PI) * MathHelper.cos(targetPitch / 180.0F * (float)Math.PI)) * currentDistance;
                double vecY = (double)(-MathHelper.sin(targetPitch / 180.0F * (float)Math.PI)) * currentDistance;

                for (int i = 0; i < 8; ++i)
                {
                    float offsetX = ((i & 1) * 2 - 1) * 0.1F;
                    float offsetY = ((i >> 1 & 1) * 2 - 1) * 0.1F;
                    float offsetZ = ((i >> 2 & 1) * 2 - 1) * 0.1F;

                    HitResult hit = new HitResult(HitResultType.MISS);

                    if (_mc.options.cameraMode == EnumCameraMode.FrontThirdPerson)
                    {
                        hit = _mc.world.raycast(
                            new Vec3D(x + offsetX, y + offsetY, z + offsetZ),
                            new Vec3D(x + vecX + offsetX + offsetZ, y + vecY + offsetY, z + vecZ + offsetZ)
                        );
                    }
                    else
                    {
                        hit = _mc.world.raycast(
                            new Vec3D(x + offsetX, y + offsetY, z + offsetZ),
                            new Vec3D(x - vecX + offsetX + offsetZ, y - vecY + offsetY, z - vecZ + offsetZ)
                        );
                    }

                    if (hit.Type != HitResultType.MISS)
                    {
                        double dist = hit.Pos.distanceTo(new Vec3D(x, y, z));
                        if (dist < currentDistance)
                        {
                            currentDistance = dist;
                        }
                    }
                }

                GLManager.GL.Rotate(cameraEntity.pitch - targetPitch, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(cameraEntity.yaw - targetYaw, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Translate(0.0F, 0.0F, (float)-currentDistance);
                if (_mc.options.cameraMode == EnumCameraMode.FrontThirdPerson)
                {
                    GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
                }
                GLManager.GL.Rotate(targetYaw - cameraEntity.yaw, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(targetPitch - cameraEntity.pitch, 1.0F, 0.0F, 0.0F);
            }
        }
        else
        {
            GLManager.GL.Translate(0.0F, 0.0F, -0.1F);
        }

        if (!_mc.options.debugCamera)
        {
            GLManager.GL.Rotate(cameraEntity.prevPitch + (cameraEntity.pitch - cameraEntity.prevPitch) * tickDelta, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(cameraEntity.prevYaw + (cameraEntity.yaw - cameraEntity.prevYaw) * tickDelta + 180.0F, 0.0F, 1.0F, 0.0F);
        }

        GLManager.GL.Translate(0.0F, eyeHeightOffset, 0.0F);
    }
}
