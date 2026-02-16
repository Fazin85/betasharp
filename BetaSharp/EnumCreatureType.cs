using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using java.lang;

namespace BetaSharp;

public class EnumCreatureType
{
    public static readonly EnumCreatureType monster = new EnumCreatureType(Monster.Class, 70, Material.Air, false);
    public static readonly EnumCreatureType creature = new EnumCreatureType(typeof(EntityAnimal), 15, Material.Air, true);
    public static readonly EnumCreatureType waterCreature = new EnumCreatureType(typeof(EntityWaterMob), 5, Material.Water, true);

    private readonly Class creatureClass;
    private readonly int maxAllowed;
    private readonly Material material;
    private readonly bool peaceful;

    public static readonly EnumCreatureType[] values = [monster, creature, waterCreature];

    private EnumCreatureType(Class creatureClass, int maxAllowed, Material material, bool peaceful)
    {
        this.creatureClass = creatureClass;
        this.maxAllowed = maxAllowed;
        this.material = material;
        this.peaceful = peaceful;
    }

    public Class getCreatureClass()
    {
        return creatureClass;
    }

    public int getMaxAllowed()
    {
        return maxAllowed;
    }

    public Material getMaterial()
    {
        return material;
    }

    public bool isPeaceful()
    {
        return peaceful;
    }
}