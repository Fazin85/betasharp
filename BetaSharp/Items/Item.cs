using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Stats;
using BetaSharp.Worlds;
using BetaSharp.Util.Maths;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Items;

public class Item : java.lang.Object
{
    protected readonly ILogger<Item> _logger = Log.Instance.For<Item>();
    
    static Item()
    {
        Stats.Stats.InitializeExtendedItemStats();

    }

    protected static JavaRandom itemRand = new();
    public static Item[] ITEMS = new Item[32000];

    public virtual void RegisterItems()
    {
        // utilisé par les items moddés pour directement se registrer
    }
    

    public static Item IronShovel =
        (new ItemSpade(0, EnumToolMaterial.IRON)).setTextureId("iron_shovel").setItemName("shovelIron");

    public static Item IronPickaxe =
        (new ItemPickaxe(1, EnumToolMaterial.IRON)).setTextureId("iron_pickaxe").setItemName("pickaxeIron");

    public static Item IronAxe =
        (new ItemAxe(2, EnumToolMaterial.IRON)).setTextureId("iron_axe").setItemName("hatchetIron");

    public static Item FlintAndSteel = (new ItemFlintAndSteel(3)).setTextureId("makeshift_lighter").setItemName("flintAndSteel");
    public static Item Apple = (new ItemFood(4, 4, false)).setTextureId("apple").setItemName("apple");
    public static Item BOW = (new ItemBow(5)).setTextureId("bow").setItemName("bow");
    public static Item ARROW = (new Item(6)).setTextureId("arrow").setItemName("arrow");
    public static Item Coal = (new ItemCoal(7)).setTextureId("coal").setItemName("coal");
    public static Item Diamond = (new Item(8)).setTextureId("diamond").setItemName("emerald");
    public static Item IronIngot = (new Item(9)).setTextureId("iron_ingot").setItemName("ingotIron");
    public static Item GoldIngot = (new Item(10)).setTextureId("gold_ingot").setItemName("ingotGold");

    public static Item IronSword =
        (new ItemSword(11, EnumToolMaterial.IRON)).setTextureId("iron_sword").setItemName("swordIron");

    public static Item WoodenSword =
        (new ItemSword(12, EnumToolMaterial.WOOD)).setTextureId("wood_sword").setItemName("swordWood");

    public static Item WoodenShovel =
        (new ItemSpade(13, EnumToolMaterial.WOOD)).setTextureId("wood_shovel").setItemName("shovelWood");

    public static Item WoodenPickaxe = (new ItemPickaxe(14, EnumToolMaterial.WOOD)).setTextureId("wood_pickaxe")
        .setItemName("pickaxeWood");

    public static Item WoodenAxe =
        (new ItemAxe(15, EnumToolMaterial.WOOD)).setTextureId("wood_axe").setItemName("hatchetWood");

    public static Item StoneSword =
        (new ItemSword(16, EnumToolMaterial.STONE)).setTextureId("stone_sword").setItemName("swordStone");

    public static Item StoneShovel =
        (new ItemSpade(17, EnumToolMaterial.STONE)).setTextureId("stone_shovel").setItemName("shovelStone");

    public static Item StonePickaxe = (new ItemPickaxe(18, EnumToolMaterial.STONE)).setTextureId("stone_pickaxe")
        .setItemName("pickaxeStone");

    public static Item StoneAxe =
        (new ItemAxe(19, EnumToolMaterial.STONE)).setTextureId("stone_sword").setItemName("hatchetStone");

    public static Item DiamondSword = (new ItemSword(20, EnumToolMaterial.EMERALD)).setTextureId("diamond_sword")
        .setItemName("swordDiamond");

    public static Item DiamondShovel = (new ItemSpade(21, EnumToolMaterial.EMERALD)).setTextureId("diamond_shovel")
        .setItemName("shovelDiamond");

    public static Item DiamondPickaxe = (new ItemPickaxe(22, EnumToolMaterial.EMERALD)).setTextureId("diamond_pickaxe")
        .setItemName("pickaxeDiamond");

    public static Item DiamondAxe = (new ItemAxe(23, EnumToolMaterial.EMERALD)).setTextureId("diamond_axe")
        .setItemName("hatchetDiamond");

    public static Item Stick = (new Item(24)).setTextureId("stick").setHandheld().setItemName("stick");
    public static Item Bowl = (new Item(25)).setTextureId("bowl").setItemName("bowl");
    public static Item MushroomStew = (new ItemSoup(26, 10)).setTextureId("mushroom_stew").setItemName("mushroomStew");

    public static Item GoldenSword =
        (new ItemSword(27, EnumToolMaterial.GOLD)).setTextureId("golden_sword").setItemName("swordGold");

    public static Item GoldenShovel =
        (new ItemSpade(28, EnumToolMaterial.GOLD)).setTextureId("golden_shovel").setItemName("shovelGold");

    public static Item GoldenPickaxe = (new ItemPickaxe(29, EnumToolMaterial.GOLD)).setTextureId("golden_pickaxe")
        .setItemName("pickaxeGold");

    public static Item GoldenAxe =
        (new ItemAxe(30, EnumToolMaterial.GOLD)).setTextureId("gold_axe").setItemName("hatchetGold");

    public static Item String = (new Item(31)).setTextureId("string").setItemName("string");
    public static Item Feather = (new Item(32)).setTextureId("feather").setItemName("feather");
    public static Item Gunpowder = (new Item(33)).setTextureId("gunpowder").setItemName("sulphur");

    public static Item WoodenHoe =
        (new ItemHoe(34, EnumToolMaterial.WOOD)).setTextureId("wood_hoe").setItemName("hoeWood");

    public static Item StoneHoe =
        (new ItemHoe(35, EnumToolMaterial.STONE)).setTextureId("stone_hoe").setItemName("hoeStone");

    public static Item IronHoe =
        (new ItemHoe(36, EnumToolMaterial.IRON)).setTextureId("iron_hoe").setItemName("hoeIron");

    public static Item DiamondHoe =
        (new ItemHoe(37, EnumToolMaterial.EMERALD)).setTextureId("diamond_hoe").setItemName("hoeDiamond");

    public static Item GoldenHoe =
        (new ItemHoe(38, EnumToolMaterial.GOLD)).setTextureId("gold_hoe").setItemName("hoeGold");

    public static Item Seeds = (new ItemSeeds(39, Block.Wheat.id)).setTextureId("seeds").setItemName("seeds");
    public static Item Wheat = (new Item(40)).setTextureId("wheat").setItemName("wheat");
    public static Item Bread = (new ItemFood(41, 5, false)).setTextureId(9, 2).setItemName("bread");
    public static Item LeatherHelmet = (new ItemArmor(42, 0, 0, 0)).setTextureId("leather_helmet").setItemName("helmetCloth");

    public static Item LeatherChestplate =
        (new ItemArmor(43, 0, 0, 1)).setTextureId("leather_chestplate").setItemName("chestplateCloth");

    public static Item LeatherLeggings =
        (new ItemArmor(44, 0, 0, 2)).setTextureId("leather_leggings").setItemName("leggingsCloth");

    public static Item LeatherBoots = (new ItemArmor(45, 0, 0, 3)).setTextureId("leather_boots").setItemName("bootsCloth");
    public static Item ChainHelmet = (new ItemArmor(46, 1, 1, 0)).setTextureId("chainmail_helmet").setItemName("chainmail_helmet");

    public static Item ChainChestplate =
        (new ItemArmor(47, 1, 1, 1)).setTextureId("chainmail_chestplate").setItemName("chestplateChain");

    public static Item ChainLeggings =
        (new ItemArmor(48, 1, 1, 2)).setTextureId("chainmail_legging").setItemName("leggingsChain");

    public static Item ChainBoots = (new ItemArmor(49, 1, 1, 3)).setTextureId("chainmail_boots").setItemName("bootsChain");
    public static Item IronHelmet = (new ItemArmor(50, 2, 2, 0)).setTextureId("iron_helmet").setItemName("helmetIron");

    public static Item IronChestplate =
        (new ItemArmor(51, 2, 2, 1)).setTextureId("iron_chestplate").setItemName("chestplateIron");

    public static Item IronLeggings = (new ItemArmor(52, 2, 2, 2)).setTextureId("iron_leggings").setItemName("leggingsIron");
    public static Item IronBoots = (new ItemArmor(53, 2, 2, 3)).setTextureId("iron_boots").setItemName("bootsIron");

    public static Item DiamondHelmet =
        (new ItemArmor(54, 3, 3, 0)).setTextureId("diamond_helmet").setItemName("helmetDiamond");

    public static Item DiamondChestplate =
        (new ItemArmor(55, 3, 3, 1)).setTextureId("diamond_chestplate").setItemName("chestplateDiamond");

    public static Item DiamondLeggings =
        (new ItemArmor(56, 3, 3, 2)).setTextureId("diamond_leggings").setItemName("leggingsDiamond");

    public static Item DiamondBoots = (new ItemArmor(57, 3, 3, 3)).setTextureId("diamond_boots").setItemName("bootsDiamond");
    public static Item GoldenHelmet = (new ItemArmor(58, 1, 4, 0)).setTextureId("gold_helmet").setItemName("helmetGold");

    public static Item GoldenChestplate =
        (new ItemArmor(59, 1, 4, 1)).setTextureId("gold_chestplate").setItemName("chestplateGold");

    public static Item GoldenLeggings =
        (new ItemArmor(60, 1, 4, 2)).setTextureId("gold_leggings").setItemName("leggingsGold");

    public static Item GoldenBoots = (new ItemArmor(61, 1, 4, 3)).setTextureId("gold_boots").setItemName("bootsGold");
    public static Item Flint = (new Item(62)).setTextureId("flint").setItemName("flint");
    public static Item RawPorkchop = (new ItemFood(63, 3, true)).setTextureId("porkchop").setItemName("porkchopRaw");

    public static Item CookedPorkchop =
        (new ItemFood(64, 8, true)).setTextureId("cooked_porkchop").setItemName("porkchopCooked");

    public static Item Painting = (new ItemPainting(65)).setTextureId("painting").setItemName("painting");
    public static Item GoldenApple = (new ItemFood(66, 42, false)).setTextureId("appleGold").setItemName("appleGold");
    public static Item Sign = (new ItemSign(67)).setTextureId("sign").setItemName("sign");
    public static Item WoodenDoor = (new ItemDoor(68, Material.Wood)).setTextureId("doorWood").setItemName("doorWood");
    public static Item Bucket = (new ItemBucket(69, 0)).setTextureId("bucket").setItemName("bucket");

    public static Item WaterBucket = (new ItemBucket(70, Block.FlowingWater.id)).setTextureId("bucketWater")
        .setItemName("bucketWater").setCraftingReturnItem(Bucket);

    public static Item LavaBucket = (new ItemBucket(71, Block.FlowingLava.id)).setTextureId("bucketLava")
        .setItemName("bucketLava").setCraftingReturnItem(Bucket);

    public static Item Minecart = (new ItemMinecart(72, 0)).setTextureId("minecart").setItemName("minecart");
    public static Item Saddle = (new ItemSaddle(73)).setTextureId("saddle").setItemName("saddle");
    public static Item IronDoor = (new ItemDoor(74, Material.Metal)).setTextureId("doorIron").setItemName("doorIron");
    public static Item Redstone = (new ItemRedstone(75)).setTextureId("redstone_dust").setItemName("redstone");
    public static Item Snowball = (new ItemSnowball(76)).setTextureId("snowball").setItemName("snowball");
    public static Item Boat = (new ItemBoat(77)).setTextureId("boat").setItemName("boat");
    public static Item Leather = (new Item(78)).setTextureId("leather").setItemName("Leather");

    public static Item MilkBucket = (new ItemBucket(79, -1)).setTextureId("milk").setItemName("milk")
        .setCraftingReturnItem(Bucket);

    public static Item Brick = (new Item(80)).setTextureId("brick").setItemName("brick");
    public static Item Clay = (new Item(81)).setTextureId("clay").setItemName("clay");
    public static Item SugarCane = (new ItemReed(82, Block.SugarCane)).setTextureId("reeds").setItemName("reeds");
    public static Item Paper = (new Item(83)).setTextureId("paper").setItemName("paper");
    public static Item Book = (new Item(84)).setTextureId("book").setItemName("book");
    public static Item Slimeball = (new Item(85)).setTextureId("slimeball").setItemName("slimeball");
    public static Item ChestMinecart = (new ItemMinecart(86, 1)).setTextureId("minecartChest").setItemName("minecartChest");

    public static Item FurnaceMinecart =
        (new ItemMinecart(87, 2)).setTextureId("minecartFurnace").setItemName("minecartFurnace");

    public static Item Egg = (new ItemEgg(88)).setTextureId("egg").setItemName("egg");
    public static Item Compass = (new Item(89)).setTextureId("compass").setItemName("compass");
    public static Item FishingRod = (new ItemFishingRod(90)).setTextureId("fishingRod").setItemName("fishingRod");
    public static Item Clock = (new Item(91)).setTextureId("clock").setItemName("clock");
    public static Item GlowstoneDust = (new Item(92)).setTextureId("yellowDust").setItemName("yellowDust");
    public static Item RawFish = (new ItemFood(93, 2, false)).setTextureId(9, 5).setItemName("fishRaw");
    public static Item CookedFish = (new ItemFood(94, 5, false)).setTextureId(10, 5).setItemName("fishCooked");
    public static Item Dye = (new ItemDye(95)).setTextureId("dye").setItemName("dyePowder");
    public static Item Bone = (new Item(96)).setTextureId("bone").setItemName("bone").setHandheld();
    public static Item Sugar = (new Item(97)).setTextureId("sugar").setItemName("sugar").setHandheld();

    public static Item Cake = (new ItemReed(98, Block.Cake)).setMaxCount(1).setTextureId("cake")
        .setItemName("cake");

    public static Item Bed = (new ItemBed(99)).setMaxCount(1).setTextureId("red_bed").setItemName("bed");
    public static Item Repeater = (new ItemReed(100, Block.Repeater)).setTextureId("repeater").setItemName("diode");
    public static Item Cookie = (new ItemCookie(101, 1, false, 8)).setTextureId("cookie").setItemName("cookie");
    public static ItemMap Map = (ItemMap)(new ItemMap(102)).setTextureId("map").setItemName("map");
    public static ItemShears Shears = (ItemShears)(new ItemShears(103)).setTextureId("shears").setItemName("shears");
    public static Item RecordThirteen = (new ItemRecord(2000, "13")).setTextureId("disk_13").setItemName("record");
    public static Item RecordCat = (new ItemRecord(2001, "cat")).setTextureId("disk_cat").setItemName("record");

    public static Item StructureSpawner = (new ItemStructureSaver(123)).setTextureId("flagRed").setItemName("structurespawner");

        


    
    
    
    private int cornerAX1940675987, cornerAY946304433, cornerAZ777210629;

    public readonly int id;
    public int maxCount = 99;
    private int maxDamage;
    protected string textureId;
    protected bool handheld;
    protected bool hasSubtypes;
    private Item craftingReturnItem;
    private string translationKey;

    protected Item(int id)
    {
        this.id = 256 + id;
        if (ITEMS[256 + id] != null)
        {
            _logger.LogInformation($"CONFLICT @ {id}");
        }
        RegisterItems();
        ITEMS[256 + id] = this;
    }

    public Item setTextureId(string textureId)
    {
        this.textureId = textureId;
        return this;
    }

    public Item setMaxCount(int maxCount)
    {
        this.maxCount = maxCount;
        return this;
    }

    public Item setTextureId(int x, int y)
    {
        return this;
    }

    public virtual string getTextureId(int damage)
    {
        return textureId;
    }

    public string getTextureId(ItemStack stack)
    {
        return getTextureId(stack.getDamage());
    }

    public virtual bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        return false;
    }

    public virtual float getMiningSpeedMultiplier(ItemStack itemStack, Block block)
    {
        return 1.0F;
    }

    public virtual ItemStack Fire(ItemStack itemStack, World world, EntityPlayer player)
    {
        return itemStack;
    }

    public virtual ItemStack AltFire(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        return itemStack;
    }

    public int getMaxCount()
    {
        return maxCount;
    }

    public virtual int getPlacementMetadata(int meta)
    {
        return 0;
    }

    public bool getHasSubtypes()
    {
        return hasSubtypes;
    }

    protected Item setHasSubtypes(bool has)
    {
        hasSubtypes = has;
        return this;
    }

    public int getMaxDamage()
    {
        return maxDamage;
    }

    protected Item setMaxDamage(int dmg)
    {
        maxDamage = dmg;
        return this;
    }

    public bool isDamagable()
    {
        return maxDamage > 0 && !hasSubtypes;
    }

    public virtual bool postHit(ItemStack itemStack, EntityLiving a, EntityLiving b)
    {
        return false;
    }

    public virtual bool postMine(ItemStack itemStack, int blockId, int x, int y, int z, EntityLiving entityLiving)
    {
        return false;
    }

    public virtual int getAttackDamage(Entity entity)
    {
        return 1;
    }

    public virtual bool isSuitableFor(Block block)
    {
        return false;
    }

    public virtual void useOnEntity(ItemStack itemStack, EntityLiving entityLiving)
    {
    }

    public Item setHandheld()
    {
        handheld = true;
        return this;
    }

    public virtual bool isHandheld()
    {
        return handheld;
    }

    public virtual bool isHandheldRod()
    {
        return false;
    }

    public Item setItemName(string name)
    {
        translationKey = "item." + name;
        return this;
    }

    public virtual string getItemName()
    {
        return translationKey;
    }

    public virtual string getItemNameIS(ItemStack itemStack)
    {
        return translationKey;
    }

    public Item setCraftingReturnItem(Item item)
    {
        if (maxCount > 1)
        {
            throw new ArgumentException("Max stack size must be 1 for items with crafting results");
        }
        else
        {
            craftingReturnItem = item;
            return this;
        }
    }

    public Item getContainerItem()
    {
        return craftingReturnItem;
    }

    public bool hasContainerItem()
    {
        return craftingReturnItem != null;
    }

    public string getStatName()
    {
        return StatCollector.TranslateToLocal(getItemName() + ".name");
    }

    public virtual int getColorMultiplier(int color)
    {
        return 0xFFFFFF;
    }

    public virtual void inventoryTick(ItemStack itemStack, World world, Entity entity, int slotIndex, bool shouldUpdate)
    {
    }

    public virtual void onCraft(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
    }

    public virtual bool isNetworkSynced()
    {
        return false;
    }
}
