namespace BetaSharp;

public class EnumArt
{
    public static readonly EnumArt Kebab = new("Kebab", 16, 16, 0, 0);
    public static readonly EnumArt Aztec = new("Aztec", 16, 16, 16, 0);
    public static readonly EnumArt Alban = new("Alban", 16, 16, 32, 0);
    public static readonly EnumArt Aztec2 = new("Aztec2", 16, 16, 48, 0);
    public static readonly EnumArt Bomb = new("Bomb", 16, 16, 64, 0);
    public static readonly EnumArt Plant = new("Plant", 16, 16, 80, 0);
    public static readonly EnumArt Wasteland = new("Wasteland", 16, 16, 96, 0);
    public static readonly EnumArt Pool = new("Pool", 32, 16, 0, 32);
    public static readonly EnumArt Courbet = new("Courbet", 32, 16, 32, 32);
    public static readonly EnumArt Sea = new("Sea", 32, 16, 64, 32);
    public static readonly EnumArt Sunset = new("Sunset", 32, 16, 96, 32);
    public static readonly EnumArt Creebet = new("Creebet", 32, 16, 128, 32);
    public static readonly EnumArt Wanderer = new("Wanderer", 16, 32, 0, 64);
    public static readonly EnumArt Graham = new("Graham", 16, 32, 16, 64);
    public static readonly EnumArt Match = new("Match", 32, 32, 0, 128);
    public static readonly EnumArt Bust = new("Bust", 32, 32, 32, 128);
    public static readonly EnumArt Stage = new("Stage", 32, 32, 64, 128);
    public static readonly EnumArt Void = new("Void", 32, 32, 96, 128);
    public static readonly EnumArt SkullAndRoses = new("SkullAndRoses", 32, 32, 128, 128);
    public static readonly EnumArt Fighters = new("Fighters", 64, 32, 0, 96);
    public static readonly EnumArt Pointer = new("Pointer", 64, 64, 0, 192);
    public static readonly EnumArt Pigscene = new("Pigscene", 64, 64, 64, 192);
    public static readonly EnumArt BurningSkull = new("BurningSkull", 64, 64, 128, 192);
    public static readonly EnumArt Skeleton = new("Skeleton", 64, 48, 192, 64);
    public static readonly EnumArt DonkeyKong = new("DonkeyKong", 64, 48, 192, 112);

    public static readonly int maxArtTitleLength = "SkullAndRoses".Length;
    public readonly string title;
    public readonly int sizeX;
    public readonly int sizeY;
    public readonly int offsetX;
    public readonly int offsetY;
    public static readonly EnumArt[] values =
    [
        Kebab,
        Aztec,
        Alban,
        Aztec2,
        Bomb,
        Plant,
        Wasteland,
        Pool,
        Courbet,
        Sea,
        Sunset,
        Creebet,
        Wanderer,
        Graham,
        Match,
        Bust,
        Stage,
        Void,
        SkullAndRoses,
        Fighters,
        Pointer,
        Pigscene,
        BurningSkull,
        Skeleton,
        DonkeyKong
    ];

    private EnumArt(string var3, int var4, int var5, int var6, int var7)
    {
        title = var3;
        sizeX = var4;
        sizeY = var5;
        offsetX = var6;
        offsetY = var7;
    }
}