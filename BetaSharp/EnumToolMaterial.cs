namespace BetaSharp;

public class EnumToolMaterial
{
    public static readonly EnumToolMaterial WOOD = new(0, 59, 2.0F, 0);
    public static readonly EnumToolMaterial STONE = new(1, 131, 4.0F, 1);
    public static readonly EnumToolMaterial IRON = new(2, 250, 6.0F, 2);
    public static readonly EnumToolMaterial EMERALD = new(3, 1561, 8.0F, 3);
    public static readonly EnumToolMaterial GOLD = new(0, 32, 12.0F, 0);

    private readonly int harvestLevel;
    private readonly int maxUses;
    private readonly float efficiencyOnProperMaterial;
    private readonly int damageVsEntity;

    private EnumToolMaterial(int var3, int var4, float var5, int var6)
    {
        harvestLevel = var3;
        maxUses = var4;
        efficiencyOnProperMaterial = var5;
        damageVsEntity = var6;
    }

    public int getMaxUses()
    {
        return maxUses;
    }

    public float getEfficiencyOnProperMaterial()
    {
        return efficiencyOnProperMaterial;
    }

    public int getDamageVsEntity()
    {
        return damageVsEntity;
    }

    public int getHarvestLevel()
    {
        return harvestLevel;
    }
}