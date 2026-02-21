using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Stats;

namespace BetaSharp;

public class Achievement : StatBase
{
    public readonly int column;
    public readonly int row;
    public readonly Achievement parent;
    public string TranslationKey { get; }
    public readonly ItemStack icon;
    private bool _isChallenge;
    public Func<string>? GetTranslatedDescription { get; set; }

    public Achievement(int id, string key, int column, int row, Item displayItem, Achievement parent) : this(id, key, column, row, new ItemStack(displayItem), parent)
    {
    }

    public Achievement(int id, string key, int column, int row, Block displayBlock, Achievement parent) : this(id, key, column, row, new ItemStack(displayBlock), parent)
    {
    }

    public Achievement(int id, string key, int column, int row, ItemStack icon, Achievement parent) : base(5242880 + id, StatCollector.translateToLocal("achievement." + key))
    {
        this.icon = icon;
        TranslationKey = StatCollector.translateToLocal("achievement." + key + ".desc");
        this.column = column;
        this.row = row;
        if (column < Achievements.MinColumn)
        {
            Achievements.MinColumn = column;
        }

        if (row < Achievements.MinRow)
        {
            Achievements.MinRow = row;
        }

        if (column > Achievements.MaxColumn)
        {
            Achievements.MaxColumn = column;
        }

        if (row > Achievements.MaxRow)
        {
            Achievements.MaxRow = row;
        }

        this.parent = parent;
    }

    public Achievement m_66876377()
    {
        base.SetLocalOnly();
        return this;
    }

    public Achievement Challenge()
    {
        _isChallenge = true;
        return this;
    }

    public Achievement RegisterAchievement()
    {
        base.RegisterStat();
        Achievements.AllAchievements.Add(this);
        return this;
    }

    public override bool IsAchievement()
    {
        return true;
    }

    public string? getTranslatedDescription()
    {
        if (GetTranslatedDescription == null)
        {
            return TranslationKey;
        }

        return GetTranslatedDescription();
    }

    public bool isChallenge()
    {
        return _isChallenge;
    }

    public override StatBase RegisterStat()
    {
        return RegisterAchievement();
    }

    public override StatBase SetLocalOnly()
    {
        return m_66876377();
    }
}
