namespace BetaSharp.Items;

public class ItemCookie : ItemFood
{

    public ItemCookie(int id, int healAmount, bool isWolfsFavoriteMeat, int maxCount) : base(id, healAmount, isWolfsFavoriteMeat)
    {
        base.maxCount = maxCount;
    }
}