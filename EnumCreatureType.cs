using betareborn.Blocks.Materials;
using betareborn.Entities;
using java.lang;

namespace betareborn
{
    public class EnumCreatureType
    {
        public static readonly EnumCreatureType monster = new EnumCreatureType(Monster.Class, 70, Material.AIR, false);
        public static readonly EnumCreatureType creature = new EnumCreatureType(typeof(EntityAnimal), 15, Material.AIR, true);
        public static readonly EnumCreatureType waterCreature = new EnumCreatureType(typeof(EntityWaterMob), 5, Material.WATER, true);

        private readonly Class creatureClass;
        private readonly int maxNumberOfCreature;
        private readonly Material creatureMaterial;
        private readonly bool isPeacefulCreature;

        public static readonly EnumCreatureType[] values = [monster, creature, waterCreature];

        private EnumCreatureType(Class var3, int var4, Material var5, bool var6)
        {
            creatureClass = var3;
            maxNumberOfCreature = var4;
            creatureMaterial = var5;
            isPeacefulCreature = var6;
        }

        public Class getCreatureClass()
        {
            return creatureClass;
        }

        public int getMaxNumberOfCreature()
        {
            return maxNumberOfCreature;
        }

        public Material getCreatureMaterial()
        {
            return creatureMaterial;
        }

        public bool getPeacefulCreature()
        {
            return isPeacefulCreature;
        }
    }
}