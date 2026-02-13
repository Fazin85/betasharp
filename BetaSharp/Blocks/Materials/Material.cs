namespace BetaSharp.Blocks.Materials;

public class Material : java.lang.Object
{
    public static readonly Material AIR = new MaterialTransparent(MapColor.airColor);
    public static readonly Material SOLID_ORGANIC = new(MapColor.grassColor);
    public static readonly Material SOIL = new(MapColor.dirtColor);
    public static readonly Material WOOD = new Material(MapColor.woodColor).setBurning();
    public static readonly Material STONE = new Material(MapColor.stoneColor).setRequiresTool();
    public static readonly Material METAL = new Material(MapColor.ironColor).setRequiresTool();
    public static readonly Material WATER = new MaterialLiquid(MapColor.waterColor).setDestroyPistonBehavior();
    public static readonly Material LAVA = new MaterialLiquid(MapColor.tntColor).setDestroyPistonBehavior();
    public static readonly Material LEAVES = new Material(MapColor.foliageColor).setBurning().setTransparent().setDestroyPistonBehavior();
    public static readonly Material PLANT = new MaterialLogic(MapColor.foliageColor).setDestroyPistonBehavior();
    public static readonly Material SPONGE = new(MapColor.clothColor);
    public static readonly Material WOOL = new Material(MapColor.clothColor).setBurning();
    public static readonly Material FIRE = new MaterialTransparent(MapColor.airColor).setDestroyPistonBehavior();
    public static readonly Material SAND = new(MapColor.sandColor);
    public static readonly Material PISTON_BREAKABLE = new MaterialLogic(MapColor.airColor).setDestroyPistonBehavior();
    public static readonly Material GLASS = new Material(MapColor.airColor).setTransparent();
    public static readonly Material TNT = new Material(MapColor.tntColor).setBurning().setTransparent();
    public static readonly Material field_4262_q = new Material(MapColor.foliageColor).setDestroyPistonBehavior();
    public static readonly Material ICE = new Material(MapColor.iceColor).setTransparent();
    public static readonly Material SNOW_LAYER = new MaterialLogic(MapColor.snowColor).setReplaceable().setTransparent().setRequiresTool().setDestroyPistonBehavior();
    public static readonly Material SNOW_BLOCK = new Material(MapColor.snowColor).setRequiresTool();
    public static readonly Material CACTUS = new Material(MapColor.foliageColor).setTransparent().setDestroyPistonBehavior();
    public static readonly Material CLAY = new(MapColor.clayColor);
    public static readonly Material PUMPKIN = new Material(MapColor.foliageColor).setDestroyPistonBehavior();
    public static readonly Material NETHER_PORTAL = new MaterialPortal(MapColor.airColor).setUnpushablePistonBehavior();
    public static readonly Material CAKE = new Material(MapColor.airColor).setDestroyPistonBehavior();
    public static readonly Material COBWEB = new Material(MapColor.clothColor).setRequiresTool().setDestroyPistonBehavior();
    public static readonly Material PISTON = new Material(MapColor.stoneColor).setUnpushablePistonBehavior();
    private bool burnable;
    private bool replaceable;
    private bool transparent;
    public readonly MapColor mapColor;
    private bool handHarvestable = true;
    private int pistonBehavior;

    public Material(MapColor mapColor)
    {
        this.mapColor = mapColor;
    }

    public virtual bool isFluid()
    {
        return false;
    }

    public virtual bool isSolid()
    {
        return true;
    }

    public virtual bool blocksVision()
    {
        return true;
    }

    public virtual bool blocksMovement()
    {
        return true;
    }

    private Material setTransparent()
    {
        transparent = true;
        return this;
    }

    private Material setRequiresTool()
    {
        handHarvestable = false;
        return this;
    }

    private Material setBurning()
    {
        burnable = true;
        return this;
    }

    public bool isBurnable()
    {
        return burnable;
    }

    public Material setReplaceable()
    {
        replaceable = true;
        return this;
    }

    public bool isReplaceable()
    {
        return replaceable;
    }

    public bool suffocates()
    {
        return transparent ? false : blocksMovement();
    }

    public bool isHandHarvestable()
    {
        return handHarvestable;
    }

    public int getPistonBehavior()
    {
        return pistonBehavior;
    }

    protected Material setDestroyPistonBehavior()
    {
        pistonBehavior = 1;
        return this;
    }

    protected Material setUnpushablePistonBehavior()
    {
        pistonBehavior = 2;
        return this;
    }
}