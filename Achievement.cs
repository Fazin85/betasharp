using betareborn.Blocks;
using betareborn.Items;
using betareborn.Stats;

namespace betareborn
{
    public class Achievement : StatBase
    {
        public readonly int column;
        public readonly int row;
        public readonly Achievement parent;
        private readonly String translationKey;
        private IStatStringFormat statStringFormatter;
        public readonly ItemStack icon;
        private bool _isChallenge;

        public Achievement(int id, string key, int column, int row, Item displayItem, Achievement parent) : this(id, key, column, row, new ItemStack(displayItem), parent)
        {
        }

        public Achievement(int id, string key, int column, int row, Block displayBlock, Achievement parent) : this(id, key, column, row, new ItemStack(displayBlock), parent)
        {
        }

        public Achievement(int id, string key, int column, int row, ItemStack icon, Achievement parent) : base(5242880 + id, StatCollector.translateToLocal("achievement." + key))
        {
            this.icon = icon;
            translationKey = StatCollector.translateToLocal("achievement." + key + ".desc");
            this.column = column;
            this.row = row;
            if (column < Achievements.minColumn)
            {
                Achievements.minColumn = column;
            }

            if (row < Achievements.minRow)
            {
                Achievements.minRow = row;
            }

            if (column > Achievements.maxColumn)
            {
                Achievements.maxColumn = column;
            }

            if (row > Achievements.maxRow)
            {
                Achievements.maxRow = row;
            }

            this.parent = parent;
        }

        public Achievement m_66876377()
        {
            localOnly = true;
            return this;
        }

        public Achievement challenge()
        {
            _isChallenge = true;
            return this;
        }

        public Achievement registerAchievement()
        {
            base.registerStat();
            Achievements.ACHIEVEMENTS.add(this);
            return this;
        }

        public override bool isAchievement()
        {
            return true;
        }

        public string getTranslatedDescription()
        {
            return statStringFormatter != null ? statStringFormatter.formatString(translationKey) : translationKey;
        }

        public Achievement setStatStringFormatter(IStatStringFormat var1)
        {
            statStringFormatter = var1;
            return this;
        }

        public bool isChallenge()
        {
            return _isChallenge;
        }

        public override StatBase registerStat()
        {
            return registerAchievement();
        }

        public override StatBase func_27082_h()
        {
            return m_66876377();
        }
    }

}