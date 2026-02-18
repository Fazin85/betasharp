namespace BetaSharp.Items;

public class ItemArmor : Item
{

    private static readonly int[] damageReduceAmountArray = [3, 8, 6, 3];
    private static readonly int[] maxDamageArray = [11, 16, 15, 13];
    public readonly int armorLevel;
    public readonly int armorType;
    public readonly int damageReduceAmount;
    public readonly int renderIndex;

    public ItemArmor(int id, int armorLevel, int renderIndex, int armorType) : base(id)
    {
        this.armorLevel = armorLevel;
        this.armorType = armorType;
        this.renderIndex = renderIndex;
        damageReduceAmount = damageReduceAmountArray[armorType];
        setMaxDamage(maxDamageArray[armorType] * 3 << armorLevel);
        maxCount = 1;
    }
}