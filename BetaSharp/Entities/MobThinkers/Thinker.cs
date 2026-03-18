using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Entities.MobThinkers;

public class Thinker
{
    protected Gender gender;
    protected EntityCreature creature;
    protected virtual void Fight(Entity target)
    {

    }

    protected virtual void Eat(Item targetItem)
    {

    }

    protected virtual void Travel(Vec3D targetPos)
    {
        
    }

    protected virtual void Run(Vec3D targetPos)
    {
        creature.Run();
        Travel(targetPos);
    }
}
