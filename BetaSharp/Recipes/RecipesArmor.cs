using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesArmor
{
    private string[][] recipePatterns =
    [
        [
            "XXX",
            "X X"
        ],
        [
            "X X",
            "XXX",
            "XXX"
        ],
        [
            "XXX",
            "X X",
            "X X"
        ],
        [
            "X X",
            "X X"
        ]
    ];
    private object[][] recipeItems =
    [
        [Item.Leather,              Block.Fire,             Item.IronIngot,         Item.Diamond,               Item.GoldIngot],
        [Item.LeatherHelmet,        Item.ChainHelmet,       Item.IronHelmet,        Item.DiamondHelmet,         Item.GoldenHelmet],
        [Item.LeatherChestplate,    Item.ChainChestplate,   Item.IronChestplate,    Item.DiamondChestplate,     Item.GoldenChestplate],
        [Item.LeatherLeggings,      Item.ChainLeggings,     Item.IronLeggings,      Item.DiamondLeggings,       Item.GoldenLeggings],
        [Item.LeatherBoots,         Item.ChainBoots,        Item.IronBoots,         Item.DiamondBoots,          Item.GoldenBoots],
    ];

    public void AddRecipes(CraftingManager manager)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            object material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item armorItem = (Item)recipeItems[j + 1][i];
                manager.AddRecipe(new ItemStack(armorItem), recipePatterns[j], 'X', material);
            }
        }

    }
}
