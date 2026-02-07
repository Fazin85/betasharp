using betareborn.Blocks;
using betareborn.Items;
using java.lang;

namespace betareborn.Recipes
{
    public class RecipesArmor
    {
        private string[][] recipePatterns = [["XXX", "X X"], ["X X", "XXX", "XXX"], ["XXX", "X X", "X X"], ["X X", "X X"]];
        private object[][] recipeItems = [new object[] { Item.leather, Block.FIRE, Item.ingotIron, Item.diamond, Item.ingotGold }, [Item.helmetLeather, Item.helmetChain, Item.helmetSteel, Item.helmetDiamond, Item.helmetGold], [Item.plateLeather, Item.plateChain, Item.plateSteel, Item.plateDiamond, Item.plateGold], [Item.legsLeather, Item.legsChain, Item.legsSteel, Item.legsDiamond, Item.legsGold], [Item.bootsLeather, Item.bootsChain, Item.bootsSteel, Item.bootsDiamond, Item.bootsGold]];

        public void addRecipes(CraftingManager var1)
        {
            for (int var2 = 0; var2 < recipeItems[0].Length; ++var2)
            {
                object var3 = recipeItems[0][var2];

                for (int var4 = 0; var4 < recipeItems.Length - 1; ++var4)
                {
                    Item var5 = (Item)recipeItems[var4 + 1][var2];
                    var1.addRecipe(new ItemStack(var5), [recipePatterns[var4], Character.valueOf('X'), var3]);
                }
            }

        }
    }

}