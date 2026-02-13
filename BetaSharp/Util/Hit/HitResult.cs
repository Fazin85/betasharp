using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Util.Hit;

public class HitResult : java.lang.Object
{
    public HitResultType type;
    public int blockX;
    public int blockY;
    public int blockZ;
    public int side;
    public Vec3D pos;
    public Entity entity;

    public HitResult(int blockX, int blockY, int blockZ, int side, Vec3D pos)
    {
        type = HitResultType.TILE;
        this.blockX = blockX;
        this.blockY = blockY;
        this.blockZ = blockZ;
        this.side = side;
        this.pos = pos;
    }

    public HitResult(Entity entity)
    {
        type = HitResultType.ENTITY;
        this.entity = entity;
        pos = new Vec3D(entity.x, entity.y, entity.z);
    }
}