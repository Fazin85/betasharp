using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Stats;
using betareborn.TileEntities;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Blocks
{
    public class Block : java.lang.Object
    {
        public static readonly BlockSoundGroup soundPowderFootstep = new("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundWoodFootstep = new("wood", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundGravelFootstep = new("gravel", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundGrassFootstep = new("grass", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundStoneFootstep = new("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundMetalFootstep = new("stone", 1.0F, 1.5F);
        public static readonly BlockSoundGroup soundGlassFootstep = new StepSoundStone("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundClothFootstep = new("cloth", 1.0F, 1.0F);
        public static readonly BlockSoundGroup soundSandFootstep = new StepSoundSand("sand", 1.0F, 1.0F);
        public static readonly Block[] BLOCKS = new Block[256];
        public static readonly bool[] BLOCKS_RANDOM_TICK = new bool[256];
        public static readonly bool[] BLOCKS_OPAQUE = new bool[256];
        public static readonly bool[] BLOCKS_WITH_ENTITY = new bool[256];
        public static readonly int[] BLOCK_LIGHT_OPACITY = new int[256];
        public static readonly bool[] BLOCKS_ALLOW_VISION = new bool[256];
        public static readonly int[] BLOCKS_LIGHT_LUMINANCE = new int[256];
        public static readonly bool[] BLOCKS_IGNORE_META_UPDATE = new bool[256];
        public static readonly Block STONE = (new BlockStone(1, 1)).setHardness(1.5F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("stone");
        public static readonly BlockGrass GRASS_BLOCK = (BlockGrass)(new BlockGrass(2)).setHardness(0.6F).setSoundGroup(soundGrassFootstep).setBlockName("grass");
        public static readonly Block DIRT = (new BlockDirt(3, 2)).setHardness(0.5F).setSoundGroup(soundGravelFootstep).setBlockName("dirt");
        public static readonly Block COBBLESTONE = (new Block(4, 16, Material.STONE)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("stonebrick");
        public static readonly Block PLANKS = (new Block(5, 4, Material.WOOD)).setHardness(2.0F).setResistance(5.0F).setSoundGroup(soundWoodFootstep).setBlockName("wood").ignoreMetaUpdates();
        public static readonly Block SAPLING = (new BlockSapling(6, 15)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("sapling").ignoreMetaUpdates();
        public static readonly Block BEDROCK = (new Block(7, 17, Material.STONE)).setUnbreakable().setResistance(6000000.0F).setSoundGroup(soundStoneFootstep).setBlockName("bedrock").disableStats();
        public static readonly Block FLOWING_WATER = (new BlockFlowing(8, Material.WATER)).setHardness(100.0F).setOpacity(3).setBlockName("water").disableStats().ignoreMetaUpdates();
        public static readonly Block WATER = (new BlockStationary(9, Material.WATER)).setHardness(100.0F).setOpacity(3).setBlockName("water").disableStats().ignoreMetaUpdates();
        public static readonly Block FLOWING_LAVA = (new BlockFlowing(10, Material.LAVA)).setHardness(0.0F).setLuminance(1.0F).setOpacity(255).setBlockName("lava").disableStats().ignoreMetaUpdates();
        public static readonly Block LAVA = (new BlockStationary(11, Material.LAVA)).setHardness(100.0F).setLuminance(1.0F).setOpacity(255).setBlockName("lava").disableStats().ignoreMetaUpdates();
        public static readonly Block SAND = (new BlockSand(12, 18)).setHardness(0.5F).setSoundGroup(soundSandFootstep).setBlockName("sand");
        public static readonly Block GRAVEL = (new BlockGravel(13, 19)).setHardness(0.6F).setSoundGroup(soundGravelFootstep).setBlockName("gravel");
        public static readonly Block GOLD_ORE = (new BlockOre(14, 32)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreGold");
        public static readonly Block IRON_ORE = (new BlockOre(15, 33)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreIron");
        public static readonly Block COAL_ORE = (new BlockOre(16, 34)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreCoal");
        public static readonly Block LOG = (new BlockLog(17)).setHardness(2.0F).setSoundGroup(soundWoodFootstep).setBlockName("log").ignoreMetaUpdates();
        public static readonly BlockLeaves LEAVES = (BlockLeaves)(new BlockLeaves(18, 52)).setHardness(0.2F).setOpacity(1).setSoundGroup(soundGrassFootstep).setBlockName("leaves").disableStats().ignoreMetaUpdates();
        public static readonly Block SPONGE = (new BlockSponge(19)).setHardness(0.6F).setSoundGroup(soundGrassFootstep).setBlockName("sponge");
        public static readonly Block GLASS = (new BlockGlass(20, 49, Material.GLASS, false)).setHardness(0.3F).setSoundGroup(soundGlassFootstep).setBlockName("glass");
        public static readonly Block LAPIS_ORE = (new BlockOre(21, 160)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreLapis");
        public static readonly Block LAPIS_BLOCK = (new Block(22, 144, Material.STONE)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("blockLapis");
        public static readonly Block DISPENSER = (new BlockDispenser(23)).setHardness(3.5F).setSoundGroup(soundStoneFootstep).setBlockName("dispenser").ignoreMetaUpdates();
        public static readonly Block SANDSTONE = (new BlockSandStone(24)).setSoundGroup(soundStoneFootstep).setHardness(0.8F).setBlockName("sandStone");
        public static readonly Block NOTE_BLOCK = (new BlockNote(25)).setHardness(0.8F).setBlockName("musicBlock").ignoreMetaUpdates();
        public static readonly Block BED = (new BlockBed(26)).setHardness(0.2F).setBlockName("bed").disableStats().ignoreMetaUpdates();
        public static readonly Block POWERED_RAIL = (new BlockRail(27, 179, true)).setHardness(0.7F).setSoundGroup(soundMetalFootstep).setBlockName("goldenRail").ignoreMetaUpdates();
        public static readonly Block DETECTOR_RAIL = (new BlockDetectorRail(28, 195)).setHardness(0.7F).setSoundGroup(soundMetalFootstep).setBlockName("detectorRail").ignoreMetaUpdates();
        public static readonly Block STICKY_PISTON = (new BlockPistonBase(29, 106, true)).setBlockName("pistonStickyBase").ignoreMetaUpdates();
        public static readonly Block COBWEB = (new BlockWeb(30, 11)).setOpacity(1).setHardness(4.0F).setBlockName("web");
        public static readonly BlockTallGrass GRASS = (BlockTallGrass)(new BlockTallGrass(31, 39)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("tallgrass");
        public static readonly BlockDeadBush DEAD_BUSH = (BlockDeadBush)(new BlockDeadBush(32, 55)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("deadbush");
        public static readonly Block PISTON = (new BlockPistonBase(33, 107, false)).setBlockName("pistonBase").ignoreMetaUpdates();
        public static readonly BlockPistonExtension PISTON_HEAD = (BlockPistonExtension)(new BlockPistonExtension(34, 107)).ignoreMetaUpdates();
        public static readonly Block WOOL = (new BlockCloth()).setHardness(0.8F).setSoundGroup(soundClothFootstep).setBlockName("cloth").ignoreMetaUpdates();
        public static readonly BlockPistonMoving MOVING_PISTON = new BlockPistonMoving(36);
        public static readonly BlockFlower DANDELION = (BlockFlower)(new BlockFlower(37, 13)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("flower");
        public static readonly BlockFlower ROSE = (BlockFlower)(new BlockFlower(38, 12)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("rose");
        public static readonly BlockFlower BROWN_MUSHROOM = (BlockFlower)(new BlockMushroom(39, 29)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setLuminance(2.0F / 16.0F).setBlockName("mushroom");
        public static readonly BlockFlower RED_MUSHROOM = (BlockFlower)(new BlockMushroom(40, 28)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("mushroom");
        public static readonly Block GOLD_BLOCK = (new BlockOreStorage(41, 23)).setHardness(3.0F).setResistance(10.0F).setSoundGroup(soundMetalFootstep).setBlockName("blockGold");
        public static readonly Block IRON_BLOCK = (new BlockOreStorage(42, 22)).setHardness(5.0F).setResistance(10.0F).setSoundGroup(soundMetalFootstep).setBlockName("blockIron");
        public static readonly Block DOUBLE_SLAB = (new BlockStep(43, true)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("stoneSlab");
        public static readonly Block SLAB = (new BlockStep(44, false)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("stoneSlab");
        public static readonly Block BRICKS = (new Block(45, 7, Material.STONE)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("brick");
        public static readonly Block TNT = (new BlockTNT(46, 8)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("tnt");
        public static readonly Block BOOKSHELF = (new BlockBookshelf(47, 35)).setHardness(1.5F).setSoundGroup(soundWoodFootstep).setBlockName("bookshelf");
        public static readonly Block MOSSY_COBBLESTONE = (new Block(48, 36, Material.STONE)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("stoneMoss");
        public static readonly Block OBSIDIAN = (new BlockObsidian(49, 37)).setHardness(10.0F).setResistance(2000.0F).setSoundGroup(soundStoneFootstep).setBlockName("obsidian");
        public static readonly Block TORCH = (new BlockTorch(50, 80)).setHardness(0.0F).setLuminance(15.0F / 16.0F).setSoundGroup(soundWoodFootstep).setBlockName("torch").ignoreMetaUpdates();
        public static readonly BlockFire FIRE = (BlockFire)(new BlockFire(51, 31)).setHardness(0.0F).setLuminance(1.0F).setSoundGroup(soundWoodFootstep).setBlockName("fire").disableStats().ignoreMetaUpdates();
        public static readonly Block SPAWNER = (new BlockMobSpawner(52, 65)).setHardness(5.0F).setSoundGroup(soundMetalFootstep).setBlockName("mobSpawner").disableStats();
        public static readonly Block WOODEN_STAIRS = (new BlockStairs(53, PLANKS)).setBlockName("stairsWood").ignoreMetaUpdates();
        public static readonly Block CHEST = (new BlockChest(54)).setHardness(2.5F).setSoundGroup(soundWoodFootstep).setBlockName("chest").ignoreMetaUpdates();
        public static readonly Block REDSTONE_WIRE = (new BlockRedstoneWire(55, 164)).setHardness(0.0F).setSoundGroup(soundPowderFootstep).setBlockName("redstoneDust").disableStats().ignoreMetaUpdates();
        public static readonly Block DIAMOND_ORE = (new BlockOre(56, 50)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreDiamond");
        public static readonly Block DIAMOND_BLOCK = (new BlockOreStorage(57, 24)).setHardness(5.0F).setResistance(10.0F).setSoundGroup(soundMetalFootstep).setBlockName("blockDiamond");
        public static readonly Block CRAFTING_TABLE = (new BlockWorkbench(58)).setHardness(2.5F).setSoundGroup(soundWoodFootstep).setBlockName("workbench");
        public static readonly Block WHEAT = (new BlockCrops(59, 88)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("crops").disableStats().ignoreMetaUpdates();
        public static readonly Block FARMLAND = (new BlockFarmland(60)).setHardness(0.6F).setSoundGroup(soundGravelFootstep).setBlockName("farmland");
        public static readonly Block FURNACE = (new BlockFurnace(61, false)).setHardness(3.5F).setSoundGroup(soundStoneFootstep).setBlockName("furnace").ignoreMetaUpdates();
        public static readonly Block LIT_FURNACE = (new BlockFurnace(62, true)).setHardness(3.5F).setSoundGroup(soundStoneFootstep).setLuminance(14.0F / 16.0F).setBlockName("furnace").ignoreMetaUpdates();
        public static readonly Block SIGN = (new BlockSign(63, TileEntitySign.Class, true)).setHardness(1.0F).setSoundGroup(soundWoodFootstep).setBlockName("sign").disableStats().ignoreMetaUpdates();
        public static readonly Block DOOR = (new BlockDoor(64, Material.WOOD)).setHardness(3.0F).setSoundGroup(soundWoodFootstep).setBlockName("doorWood").disableStats().ignoreMetaUpdates();
        public static readonly Block LADDER = (new BlockLadder(65, 83)).setHardness(0.4F).setSoundGroup(soundWoodFootstep).setBlockName("ladder").ignoreMetaUpdates();
        public static readonly Block RAIL = (new BlockRail(66, 128, false)).setHardness(0.7F).setSoundGroup(soundMetalFootstep).setBlockName("rail").ignoreMetaUpdates();
        public static readonly Block COBBLESTONE_STAIRS = (new BlockStairs(67, COBBLESTONE)).setBlockName("stairsStone").ignoreMetaUpdates();
        public static readonly Block WALL_SIGN = (new BlockSign(68, TileEntitySign.Class, false)).setHardness(1.0F).setSoundGroup(soundWoodFootstep).setBlockName("sign").disableStats().ignoreMetaUpdates();
        public static readonly Block LEVER = (new BlockLever(69, 96)).setHardness(0.5F).setSoundGroup(soundWoodFootstep).setBlockName("lever").ignoreMetaUpdates();
        public static readonly Block STONE_PRESSURE_PLATE = (new BlockPressurePlate(70, STONE.textureId, EnumMobType.mobs, Material.STONE)).setHardness(0.5F).setSoundGroup(soundStoneFootstep).setBlockName("pressurePlate").ignoreMetaUpdates();
        public static readonly Block IRON_DOOR = (new BlockDoor(71, Material.METAL)).setHardness(5.0F).setSoundGroup(soundMetalFootstep).setBlockName("doorIron").disableStats().ignoreMetaUpdates();
        public static readonly Block WOODEN_PRESSURE_PLATE = (new BlockPressurePlate(72, PLANKS.textureId, EnumMobType.everything, Material.WOOD)).setHardness(0.5F).setSoundGroup(soundWoodFootstep).setBlockName("pressurePlate").ignoreMetaUpdates();
        public static readonly Block REDSTONE_ORE = (new BlockRedstoneOre(73, 51, false)).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreRedstone").ignoreMetaUpdates();
        public static readonly Block LIT_REDSTONE_ORE = (new BlockRedstoneOre(74, 51, true)).setLuminance(10.0F / 16.0F).setHardness(3.0F).setResistance(5.0F).setSoundGroup(soundStoneFootstep).setBlockName("oreRedstone").ignoreMetaUpdates();
        public static readonly Block REDSTONE_TORCH = (new BlockRedstoneTorch(75, 115, false)).setHardness(0.0F).setSoundGroup(soundWoodFootstep).setBlockName("notGate").ignoreMetaUpdates();
        public static readonly Block LIT_REDSTONE_TORCH = (new BlockRedstoneTorch(76, 99, true)).setHardness(0.0F).setLuminance(0.5F).setSoundGroup(soundWoodFootstep).setBlockName("notGate").ignoreMetaUpdates();
        public static readonly Block BUTTON = (new BlockButton(77, STONE.textureId)).setHardness(0.5F).setSoundGroup(soundStoneFootstep).setBlockName("button").ignoreMetaUpdates();
        public static readonly Block SNOW = (new BlockSnow(78, 66)).setHardness(0.1F).setSoundGroup(soundClothFootstep).setBlockName("snow");
        public static readonly Block ICE = (new BlockIce(79, 67)).setHardness(0.5F).setOpacity(3).setSoundGroup(soundGlassFootstep).setBlockName("ice");
        public static readonly Block SNOW_BLOCK = (new BlockSnowBlock(80, 66)).setHardness(0.2F).setSoundGroup(soundClothFootstep).setBlockName("snow");
        public static readonly Block CACTUS = (new BlockCactus(81, 70)).setHardness(0.4F).setSoundGroup(soundClothFootstep).setBlockName("cactus");
        public static readonly Block CLAY = (new BlockClay(82, 72)).setHardness(0.6F).setSoundGroup(soundGravelFootstep).setBlockName("clay");
        public static readonly Block SUGAR_CANE = (new BlockReed(83, 73)).setHardness(0.0F).setSoundGroup(soundGrassFootstep).setBlockName("reeds").disableStats();
        public static readonly Block JUKEBOX = (new BlockJukeBox(84, 74)).setHardness(2.0F).setResistance(10.0F).setSoundGroup(soundStoneFootstep).setBlockName("jukebox").ignoreMetaUpdates();
        public static readonly Block FENCE = (new BlockFence(85, 4)).setHardness(2.0F).setResistance(5.0F).setSoundGroup(soundWoodFootstep).setBlockName("fence").ignoreMetaUpdates();
        public static readonly Block PUMPKIN = (new BlockPumpkin(86, 102, false)).setHardness(1.0F).setSoundGroup(soundWoodFootstep).setBlockName("pumpkin").ignoreMetaUpdates();
        public static readonly Block NETHERRACK = (new BlockNetherrack(87, 103)).setHardness(0.4F).setSoundGroup(soundStoneFootstep).setBlockName("hellrock");
        public static readonly Block SOUL_SAND = (new BlockSoulSand(88, 104)).setHardness(0.5F).setSoundGroup(soundSandFootstep).setBlockName("hellsand");
        public static readonly Block GLOWSTONE = (new BlockGlowStone(89, 105, Material.STONE)).setHardness(0.3F).setSoundGroup(soundGlassFootstep).setLuminance(1.0F).setBlockName("lightgem");
        public static readonly BlockPortal NETHER_PORTAL = (BlockPortal)(new BlockPortal(90, 14)).setHardness(-1.0F).setSoundGroup(soundGlassFootstep).setLuminance(12.0F / 16.0F).setBlockName("portal");
        public static readonly Block JACK_O_LANTERN = (new BlockPumpkin(91, 102, true)).setHardness(1.0F).setSoundGroup(soundWoodFootstep).setLuminance(1.0F).setBlockName("litpumpkin").ignoreMetaUpdates();
        public static readonly Block CAKE = (new BlockCake(92, 121)).setHardness(0.5F).setSoundGroup(soundClothFootstep).setBlockName("cake").disableStats().ignoreMetaUpdates();
        public static readonly Block REPEATER = (new BlockRedstoneRepeater(93, false)).setHardness(0.0F).setSoundGroup(soundWoodFootstep).setBlockName("diode").disableStats().ignoreMetaUpdates();
        public static readonly Block POWERED_REPEATER = (new BlockRedstoneRepeater(94, true)).setHardness(0.0F).setLuminance(10.0F / 16.0F).setSoundGroup(soundWoodFootstep).setBlockName("diode").disableStats().ignoreMetaUpdates();
        public static readonly Block TRAPDOOR = (new BlockTrapDoor(96, Material.WOOD)).setHardness(3.0F).setSoundGroup(soundWoodFootstep).setBlockName("trapdoor").disableStats().ignoreMetaUpdates();
        public int textureId;
        public readonly int id;
        public float hardness;
        public float resistance;
        protected bool shouldTrackStatistics;
        public double minX;
        public double minY;
        public double minZ;
        public double maxX;
        public double maxY;
        public double maxZ;
        public BlockSoundGroup soundGroup;
        public float particleFallSpeedModifier;
        public readonly Material material;
        public float slipperiness;
        private string blockName;

        protected Block(int id, Material material)
        {
            shouldTrackStatistics = true;
            soundGroup = soundPowderFootstep;
            particleFallSpeedModifier = 1.0F;
            slipperiness = 0.6F;
            if (BLOCKS[id] != null)
            {
                throw new IllegalArgumentException("Slot " + id + " is already occupied by " + BLOCKS[id] + " when adding " + this);
            }
            else
            {
                this.material = material;
                BLOCKS[id] = this;
                this.id = id;
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                BLOCKS_OPAQUE[id] = isOpaque();
                BLOCK_LIGHT_OPACITY[id] = isOpaque() ? 255 : 0;
                BLOCKS_ALLOW_VISION[id] = !material.blocksVision();
                BLOCKS_WITH_ENTITY[id] = false;
            }
        }

        protected Block ignoreMetaUpdates()
        {
            BLOCKS_IGNORE_META_UPDATE[id] = true;
            return this;
        }

        protected virtual void init()
        {
        }

        protected Block(int id, int textureId, Material material) : this(id, material)
        {
            this.textureId = textureId;
        }

        protected Block setSoundGroup(BlockSoundGroup soundGroup)
        {
            this.soundGroup = soundGroup;
            return this;
        }

        protected Block setOpacity(int opacity)
        {
            BLOCK_LIGHT_OPACITY[id] = opacity;
            return this;
        }

        protected Block setLuminance(float fractionalValue)
        {
            BLOCKS_LIGHT_LUMINANCE[id] = (int)(15.0F * fractionalValue);
            return this;
        }

        protected Block setResistance(float resistance)
        {
            this.resistance = resistance * 3.0F;
            return this;
        }

        public virtual bool isFullCube()
        {
            return true;
        }

        public virtual int getRenderType()
        {
            return 0;
        }

        protected Block setHardness(float hardness)
        {
            this.hardness = hardness;
            if (resistance < hardness * 5.0F)
            {
                resistance = hardness * 5.0F;
            }

            return this;
        }

        protected Block setUnbreakable()
        {
            setHardness(-1.0F);
            return this;
        }

        public float getHardness()
        {
            return hardness;
        }

        protected Block setTickRandomly(bool tickRandomly)
        {
            BLOCKS_RANDOM_TICK[id] = tickRandomly;
            return this;
        }

        public void setBoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            this.minX = minX;
            this.minY = minY;
            this.minZ = minZ;
            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
        }

        public virtual float getLuminance(BlockView blockView, int x, int y, int z)
        {
            return blockView.getNaturalBrightness(x, y, z, BLOCKS_LIGHT_LUMINANCE[id]);
        }

        public virtual bool isSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            return side == 0 && minY > 0.0D ? true : (side == 1 && maxY < 1.0D ? true : (side == 2 && minZ > 0.0D ? true : (side == 3 && maxZ < 1.0D ? true : (side == 4 && minX > 0.0D ? true : (side == 5 && maxX < 1.0D ? true : !blockView.isOpaque(x, y, z))))));
        }

        public virtual bool isSolidFace(BlockView blockView, int x, int y, int z, int face)
        {
            return blockView.getMaterial(x, y, z).isSolid();
        }

        public virtual int getTexture(BlockView blockView, int x, int y, int z, int side)
        {
            return getTexture(side, blockView.getBlockMeta(x, y, z));
        }

        public virtual int getTexture(int side, int meta)
        {
            return getTexture(side);
        }

        public virtual int getTexture(int side)
        {
            return textureId;
        }

        public virtual Box getBoundingBox(World world, int x, int y, int z)
        {
            return Box.createCached((double)x + minX, (double)y + minY, (double)z + minZ, (double)x + maxX, (double)y + maxY, (double)z + maxZ);
        }

        public virtual void addIntersectingBoundingBox(World world, int x, int y, int z, Box box, List<Box> boxes)
        {
            Box var7 = getCollisionShape(world, x, y, z);
            if (var7 != null && box.intersects(var7))
            {
                boxes.Add(var7);
            }

        }

        public virtual Box getCollisionShape(World world, int x, int y, int z)
        {
            return Box.createCached((double)x + minX, (double)y + minY, (double)z + minZ, (double)x + maxX, (double)y + maxY, (double)z + maxZ);
        }

        public virtual bool isOpaque()
        {
            return true;
        }

        public virtual bool hasCollision(int meta, bool allowLiquids)
        {
            return hasCollision();
        }

        public virtual bool hasCollision()
        {
            return true;
        }

        public virtual void onTick(World world, int x, int y, int z, java.util.Random random)
        {
        }

        public virtual void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
        }

        public virtual void onBlockDestroyedByPlayer(World var1, int var2, int var3, int var4, int var5)
        {
        }

        public virtual void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
        }

        public virtual int tickRate()
        {
            return 10;
        }

        public virtual void onBlockAdded(World var1, int var2, int var3, int var4)
        {
        }

        public virtual void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
        }

        public virtual int quantityDropped(java.util.Random var1)
        {
            return 1;
        }

        public virtual int getDroppedItemId(int var1, java.util.Random var2)
        {
            return id;
        }

        public float blockStrength(EntityPlayer var1)
        {
            return hardness < 0.0F ? 0.0F : (!var1.canHarvestBlock(this) ? 1.0F / hardness / 100.0F : var1.getCurrentPlayerStrVsBlock(this) / hardness / 30.0F);
        }

        public void dropBlockAsItem(World var1, int var2, int var3, int var4, int var5)
        {
            dropStacks(var1, var2, var3, var4, var5, 1.0F);
        }

        public virtual void dropStacks(World var1, int var2, int var3, int var4, int var5, float var6)
        {
            if (!var1.multiplayerWorld)
            {
                int var7 = quantityDropped(var1.random);

                for (int var8 = 0; var8 < var7; ++var8)
                {
                    if (var1.random.nextFloat() <= var6)
                    {
                        int var9 = getDroppedItemId(var5, var1.random);
                        if (var9 > 0)
                        {
                            dropBlockAsItem_do(var1, var2, var3, var4, new ItemStack(var9, 1, damageDropped(var5)));
                        }
                    }
                }

            }
        }

        protected void dropBlockAsItem_do(World var1, int var2, int var3, int var4, ItemStack var5)
        {
            if (!var1.multiplayerWorld)
            {
                float var6 = 0.7F;
                double var7 = (double)(var1.random.nextFloat() * var6) + (double)(1.0F - var6) * 0.5D;
                double var9 = (double)(var1.random.nextFloat() * var6) + (double)(1.0F - var6) * 0.5D;
                double var11 = (double)(var1.random.nextFloat() * var6) + (double)(1.0F - var6) * 0.5D;
                EntityItem var13 = new EntityItem(var1, (double)var2 + var7, (double)var3 + var9, (double)var4 + var11, var5);
                var13.delayBeforeCanPickup = 10;
                var1.spawnEntity(var13);
            }
        }

        protected virtual int damageDropped(int var1)
        {
            return 0;
        }

        public virtual float getExplosionResistance(Entity var1)
        {
            return resistance / 5.0F;
        }

        public virtual MovingObjectPosition collisionRayTrace(World var1, int var2, int var3, int var4, Vec3D var5, Vec3D var6)
        {
            updateBoundingBox(var1, var2, var3, var4);
            var5 = var5.addVector((double)(-var2), (double)(-var3), (double)(-var4));
            var6 = var6.addVector((double)(-var2), (double)(-var3), (double)(-var4));
            Vec3D var7 = var5.getIntermediateWithXValue(var6, minX);
            Vec3D var8 = var5.getIntermediateWithXValue(var6, maxX);
            Vec3D var9 = var5.getIntermediateWithYValue(var6, minY);
            Vec3D var10 = var5.getIntermediateWithYValue(var6, maxY);
            Vec3D var11 = var5.getIntermediateWithZValue(var6, minZ);
            Vec3D var12 = var5.getIntermediateWithZValue(var6, maxZ);
            if (!isVecInsideYZBounds(var7))
            {
                var7 = null;
            }

            if (!isVecInsideYZBounds(var8))
            {
                var8 = null;
            }

            if (!isVecInsideXZBounds(var9))
            {
                var9 = null;
            }

            if (!isVecInsideXZBounds(var10))
            {
                var10 = null;
            }

            if (!isVecInsideXYBounds(var11))
            {
                var11 = null;
            }

            if (!isVecInsideXYBounds(var12))
            {
                var12 = null;
            }

            Vec3D var13 = null;
            if (var7 != null && (var13 == null || var5.distanceTo(var7) < var5.distanceTo(var13)))
            {
                var13 = var7;
            }

            if (var8 != null && (var13 == null || var5.distanceTo(var8) < var5.distanceTo(var13)))
            {
                var13 = var8;
            }

            if (var9 != null && (var13 == null || var5.distanceTo(var9) < var5.distanceTo(var13)))
            {
                var13 = var9;
            }

            if (var10 != null && (var13 == null || var5.distanceTo(var10) < var5.distanceTo(var13)))
            {
                var13 = var10;
            }

            if (var11 != null && (var13 == null || var5.distanceTo(var11) < var5.distanceTo(var13)))
            {
                var13 = var11;
            }

            if (var12 != null && (var13 == null || var5.distanceTo(var12) < var5.distanceTo(var13)))
            {
                var13 = var12;
            }

            if (var13 == null)
            {
                return null;
            }
            else
            {
                int var14 = -1;
                if (var13 == var7)
                {
                    var14 = 4;
                }

                if (var13 == var8)
                {
                    var14 = 5;
                }

                if (var13 == var9)
                {
                    var14 = 0;
                }

                if (var13 == var10)
                {
                    var14 = 1;
                }

                if (var13 == var11)
                {
                    var14 = 2;
                }

                if (var13 == var12)
                {
                    var14 = 3;
                }

                return new MovingObjectPosition(var2, var3, var4, var14, var13.addVector((double)var2, (double)var3, (double)var4));
            }
        }

        private bool isVecInsideYZBounds(Vec3D var1)
        {
            return var1 == null ? false : var1.yCoord >= minY && var1.yCoord <= maxY && var1.zCoord >= minZ && var1.zCoord <= maxZ;
        }

        private bool isVecInsideXZBounds(Vec3D var1)
        {
            return var1 == null ? false : var1.xCoord >= minX && var1.xCoord <= maxX && var1.zCoord >= minZ && var1.zCoord <= maxZ;
        }

        private bool isVecInsideXYBounds(Vec3D var1)
        {
            return var1 == null ? false : var1.xCoord >= minX && var1.xCoord <= maxX && var1.yCoord >= minY && var1.yCoord <= maxY;
        }

        public virtual void onBlockDestroyedByExplosion(World var1, int var2, int var3, int var4)
        {
        }

        public virtual int getRenderBlockPass()
        {
            return 0;
        }

        public virtual bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            return canPlaceBlockAt(var1, var2, var3, var4);
        }

        public virtual bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockId(var2, var3, var4);
            return var5 == 0 || BLOCKS[var5].material.isReplaceable();
        }

        public virtual bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            return false;
        }

        public virtual void onEntityWalking(World var1, int var2, int var3, int var4, Entity var5)
        {
        }

        public virtual void onBlockPlaced(World var1, int var2, int var3, int var4, int var5)
        {
        }

        public virtual void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
        }

        public virtual void velocityToAddToEntity(World var1, int var2, int var3, int var4, Entity var5, Vec3D var6)
        {
        }

        public virtual void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
        }

        public virtual int getRenderColor(int var1)
        {
            return 16777215;
        }

        public virtual int colorMultiplier(BlockView var1, int var2, int var3, int var4)
        {
            return 16777215;
        }

        public virtual bool isPoweringTo(BlockView var1, int var2, int var3, int var4, int var5)
        {
            return false;
        }

        public virtual bool canProvidePower()
        {
            return false;
        }

        public virtual void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
        }

        public virtual bool isIndirectlyPoweringTo(World var1, int var2, int var3, int var4, int var5)
        {
            return false;
        }

        public virtual void setBlockBoundsForItemRender()
        {
        }

        public virtual void harvestBlock(World var1, EntityPlayer var2, int var3, int var4, int var5, int var6)
        {
            var2.addStat(StatList.mineBlockStatArray[id], 1);
            dropBlockAsItem(var1, var3, var4, var5, var6);
        }

        public virtual bool canBlockStay(World var1, int var2, int var3, int var4)
        {
            return true;
        }

        public virtual void onBlockPlacedBy(World var1, int var2, int var3, int var4, EntityLiving var5)
        {
        }

        public Block setBlockName(string var1)
        {
            blockName = "tile." + var1;
            return this;
        }

        public string translateBlockName()
        {
            return StatCollector.translateToLocal(getBlockName() + ".name");
        }

        public string getBlockName()
        {
            return blockName;
        }

        public virtual void playBlock(World var1, int var2, int var3, int var4, int var5, int var6)
        {
        }

        public bool getEnableStats()
        {
            return shouldTrackStatistics;
        }

        protected Block disableStats()
        {
            shouldTrackStatistics = false;
            return this;
        }

        public virtual int getPistonBehavior()
        {
            return material.getPistonBehavior();
        }

        static Block()
        {
            Item.itemsList[WOOL.id] = (new ItemCloth(WOOL.id - 256)).setItemName("cloth");
            Item.itemsList[LOG.id] = (new ItemLog(LOG.id - 256)).setItemName("log");
            Item.itemsList[SLAB.id] = (new ItemSlab(SLAB.id - 256)).setItemName("stoneSlab");
            Item.itemsList[SAPLING.id] = (new ItemSapling(SAPLING.id - 256)).setItemName("sapling");
            Item.itemsList[LEAVES.id] = (new ItemLeaves(LEAVES.id - 256)).setItemName("leaves");
            Item.itemsList[PISTON.id] = new ItemPiston(PISTON.id - 256);
            Item.itemsList[STICKY_PISTON.id] = new ItemPiston(STICKY_PISTON.id - 256);

            for (int var0 = 0; var0 < 256; ++var0)
            {
                if (BLOCKS[var0] != null && Item.itemsList[var0] == null)
                {
                    Item.itemsList[var0] = new ItemBlock(var0 - 256);
                    BLOCKS[var0].init();
                }
            }

            BLOCKS_ALLOW_VISION[0] = true;
            StatList.func_25154_a();
        }
    }

}