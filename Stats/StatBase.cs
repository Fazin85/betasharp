using java.lang;
using java.text;
using java.util;

namespace betareborn.Stats
{
    public class StatBase : java.lang.Object
    {
        public readonly int statId;
        public readonly string statName;
        public bool localOnly;
        public string statGuid;
        private readonly IStatType field_26902_a;
        private static NumberFormat field_26903_b = NumberFormat.getIntegerInstance(Locale.US);
        public static IStatType field_27087_i = new StatTypeSimple();
        private static DecimalFormat field_26904_c = new DecimalFormat("########0.00");
        public static IStatType field_27086_j = new StatTypeTime();
        public static IStatType field_27085_k = new StatTypeDistance();

        public StatBase(int var1, string var2, IStatType var3)
        {
            localOnly = false;
            statId = var1;
            statName = var2;
            field_26902_a = var3;
        }

        public StatBase(int var1, string var2) : this(var1, var2, field_27087_i)
        {
        }

        public virtual StatBase func_27082_h()
        {
            localOnly = true;
            return this;
        }

        public virtual StatBase registerStat()
        {
            if (StatList.field_25169_C.containsKey(Integer.valueOf(statId)))
            {
                throw new RuntimeException("Duplicate stat id: \"" + ((StatBase)StatList.field_25169_C.get(Integer.valueOf(statId))).statName + "\" and \"" + statName + "\" at id " + statId);
            }
            else
            {
                StatList.field_25188_a.add(this);
                StatList.field_25169_C.put(Integer.valueOf(statId), this);
                statGuid = AchievementMap.getGuid(statId);
                return this;
            }
        }

        public virtual bool isAchievement()
        {
            return false;
        }

        public string func_27084_a(int var1)
        {
            return field_26902_a.func_27192_a(var1);
        }

        public override string toString()
        {
            return statName;
        }

        public static NumberFormat func_27083_i()
        {
            return field_26903_b;
        }

        public static DecimalFormat func_27081_j()
        {
            return field_26904_c;
        }
    }

}