using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockCloth : Block
{
    // Ordre officiel Beta 1.7.3 — meta 0 à 15
    private static readonly string[] ColorNames =
    [
        "white", "orange", "magenta", "light_blue", "yellow", "lime", "pink",
        "gray", "light_gray", "cyan", "purple", "blue", "brown", "green", "red", "black"
    ];

    public BlockCloth() : base(35, "wool", Material.Wool) { }

    public override string getTexture(string side, int meta)
    {
        string color = ColorNames[meta & 15];
        return  $"{color}_{textureId}";
        // → "wool" pour blanc, "wool_orange", "wool_red", etc.
    }

    public override string getTexture(string side) => textureId; // blanc par défaut

    protected override int getDroppedItemMeta(int blockMeta) => blockMeta;

    // Conversions meta bloc ↔ meta item
    // Notch inversait les bits pour stocker la couleur — on garde la compatibilité
    public static int getBlockMeta(int itemMeta) => ~itemMeta & 15;
    public static int getItemMeta(int blockMeta) => ~blockMeta & 15;

    // Utilitaire si besoin ailleurs
    public static string GetColorName(int meta) => ColorNames[meta & 15];
}
