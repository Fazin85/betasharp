using BetaSharp.Client.Guis;
using BetaSharp.Stats;
using java.util;
using java.util.function;

namespace BetaSharp.Client;

public class SorterStatsItem : Comparator
{
    public readonly GuiStats field_27373_a;
    readonly GuiSlotStatsItem field_27372_b;

    public SorterStatsItem(GuiSlotStatsItem var1, GuiStats var2)
    {
        field_27372_b = var1;
        field_27373_a = var2;
    }


    public int func_27371_a(StatCrafting var1, StatCrafting var2)
    {
        int var3 = var1.getItemId();
        int var4 = var2.getItemId();
        StatBase var5 = null;
        StatBase var6 = null;
        if (field_27372_b.field_27271_e == 0)
        {
            var5 = Stats.Stats.BROKEN[var3];
            var6 = Stats.Stats.BROKEN[var4];
        }
        else if (field_27372_b.field_27271_e == 1)
        {
            var5 = Stats.Stats.CRAFTED[var3];
            var6 = Stats.Stats.CRAFTED[var4];
        }
        else if (field_27372_b.field_27271_e == 2)
        {
            var5 = Stats.Stats.USED[var3];
            var6 = Stats.Stats.USED[var4];
        }

        if (var5 != null || var6 != null)
        {
            if (var5 == null)
            {
                return 1;
            }

            if (var6 == null)
            {
                return -1;
            }

            int var7 = field_27373_a.statFileWriter.writeStat(var5);
            int var8 = field_27373_a.statFileWriter.writeStat(var6);
            if (var7 != var8)
            {
                return (var7 - var8) * field_27372_b.field_27270_f;
            }
        }

        return var3 - var4;
    }

    public int compare(object var1, object var2)
    {
        return func_27371_a((StatCrafting)var1, (StatCrafting)var2);
    }

    public Comparator thenComparing(Comparator other)
    {
        throw new NotImplementedException();
    }

    public bool equals(object obj)
    {
        throw new NotImplementedException();
    }

    public Comparator reversed()
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparing(Function keyExtractor, Comparator keyComparator)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparing(Function keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingInt(ToIntFunction keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingLong(ToLongFunction keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingDouble(ToDoubleFunction keyExtractor)
    {
        throw new NotImplementedException();
    }
}
