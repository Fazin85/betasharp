using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

// NOTE: CHESTS DON'T ROTATE BASED ON PLAYER ORIENTATION, THIS IS VANILLA BEHAVIOR, NOT A BUG
public class BlockChest : BlockWithEntity
{
    private JavaRandom random = new();

    // Textures chest simple
    // base=26 → chest_side, -1=25 → chest_top, +1=27 → chest_front
    private const string TexTop = "chest_top";
    private const string TexSide = "chest_side";
    private const string TexFront = "chest_front";

    // Textures double chest (côté droit = bloc de gauche)
    // base+16=42 → double_chest_right_side
    // base+17=43 → double_chest_right_front
    // base+15=41 → double_chest_right_back
    // base+32=58 → double_chest_left_side
    // base+33=59 → double_chest_left_front
    // base+31=57 → double_chest_left_back
    private const string TexDoubleRightSide = "chest_side_right";
    private const string TexDoubleRightFront = "chest_front_right";
    private const string TexDoubleRightBack = "chest_back_right";
    private const string TexDoubleLeftSide = "chest_side_left";
    private const string TexDoubleLeftFront = "chest_front_left";
    private const string TexDoubleLeftBack = "chest_back_left";

    public BlockChest(int id) : base(id, Material.Wood)
    {
        textureId = "chest";
    }

    public override string getTextureId(BlockView blockView, int x, int y, int z, string side)
    {
        int sideInt = SideNameToInt(side);

        if (sideInt == 1 || sideInt == 0)
            return TexTop;

        int blockNorth = blockView.getBlockId(x, y, z - 1);
        int blockSouth = blockView.getBlockId(x, y, z + 1);
        int blockWest = blockView.getBlockId(x - 1, y, z);
        int blockEast = blockView.getBlockId(x + 1, y, z);

        // Chest simple (pas de voisin chest sur X ou Z)
        if (blockNorth != id && blockSouth != id)
        {
            if (blockWest != id && blockEast != id)
            {
                // Chest seul — détermine la face avant selon les blocs opaques voisins
                int facingSide = 3;
                if (Block.BlocksOpaque[blockNorth] && !Block.BlocksOpaque[blockSouth]) facingSide = 3;
                if (Block.BlocksOpaque[blockSouth] && !Block.BlocksOpaque[blockNorth]) facingSide = 2;
                if (Block.BlocksOpaque[blockWest] && !Block.BlocksOpaque[blockEast]) facingSide = 5;
                if (Block.BlocksOpaque[blockEast] && !Block.BlocksOpaque[blockWest]) facingSide = 4;

                return sideInt == facingSide ? TexFront : TexSide;
            }
            else
            {
                // Double chest orienté Est-Ouest
                if (sideInt == 4 || sideInt == 5) return TexSide;

                bool isLeft = blockWest == id;
                int neighbor = blockView.getBlockId(isLeft ? x - 1 : x + 1, y, z - 1);
                int neighbor2 = blockView.getBlockId(isLeft ? x - 1 : x + 1, y, z + 1);

                int facingSide = 3;
                if ((Block.BlocksOpaque[blockNorth] || Block.BlocksOpaque[neighbor]) && !Block.BlocksOpaque[blockSouth] && !Block.BlocksOpaque[neighbor2]) facingSide = 3;
                if ((Block.BlocksOpaque[blockSouth] || Block.BlocksOpaque[neighbor2]) && !Block.BlocksOpaque[blockNorth] && !Block.BlocksOpaque[neighbor]) facingSide = 2;

                bool isFront = sideInt == facingSide;
                // Correction de l'offset gauche/droite selon l'orientation
                bool flipSide = isLeft ^ (sideInt == 3);

                if (isFront) return flipSide ? TexDoubleRightFront : TexDoubleLeftFront;
                return flipSide ? TexDoubleRightBack : TexDoubleLeftBack;
            }
        }
        else
        {
            // Double chest orienté Nord-Sud
            if (sideInt == 2 || sideInt == 3) return TexSide;

            bool isLeft = blockNorth == id;
            int neighbor = blockView.getBlockId(x - 1, y, isLeft ? z - 1 : z + 1);
            int neighbor2 = blockView.getBlockId(x + 1, y, isLeft ? z - 1 : z + 1);

            int facingSide = 5;
            if ((Block.BlocksOpaque[blockWest] || Block.BlocksOpaque[neighbor]) && !Block.BlocksOpaque[blockEast] && !Block.BlocksOpaque[neighbor2]) facingSide = 5;
            if ((Block.BlocksOpaque[blockEast] || Block.BlocksOpaque[neighbor2]) && !Block.BlocksOpaque[blockWest] && !Block.BlocksOpaque[neighbor]) facingSide = 4;

            bool isFront = sideInt == facingSide;
            bool flipSide = isLeft ^ (sideInt == 4);

            if (isFront) return flipSide ? TexDoubleRightFront : TexDoubleLeftFront;
            return flipSide ? TexDoubleRightBack : TexDoubleLeftBack;
        }
    }

    public override string getTexture(string side)
    {
        int sideInt = SideNameToInt(side);
        if (sideInt == 1 || sideInt == 0) return TexTop;
        if (sideInt == 3) return TexFront;
        return TexSide;
    }

    // Convertit les noms de face string en int pour la logique existante
    private static int SideNameToInt(string side) => side switch
    {
        "bottom" => 0,
        "top" => 1,
        "north" => 2,
        "south" => 3,
        "west" => 4,
        "east" => 5,
        _ => int.TryParse(side, out int n) ? n : 0
    };

    public override bool canPlaceAt(World world, int x, int y, int z)
    {
        int count = 0;
        if (world.getBlockId(x - 1, y, z) == id) count++;
        if (world.getBlockId(x + 1, y, z) == id) count++;
        if (world.getBlockId(x, y, z - 1) == id) count++;
        if (world.getBlockId(x, y, z + 1) == id) count++;
        return count <= 1
            && !hasNeighbor(world, x - 1, y, z)
            && !hasNeighbor(world, x + 1, y, z)
            && !hasNeighbor(world, x, y, z - 1)
            && !hasNeighbor(world, x, y, z + 1);
    }

    private bool hasNeighbor(World world, int x, int y, int z)
    {
        if (world.getBlockId(x, y, z) != id) return false;
        return world.getBlockId(x - 1, y, z) == id
            || world.getBlockId(x + 1, y, z) == id
            || world.getBlockId(x, y, z - 1) == id
            || world.getBlockId(x, y, z + 1) == id;
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        BlockEntityChest chest = (BlockEntityChest)world.getBlockEntity(x, y, z);
        if (chest is null)
        {
            world.server.Warn($"BlockChest.onBreak: BlockEntity introuvable à {x} {y} {z}");
            base.onBreak(world, x, y, z);
            return;
        }
        DropContents(world, chest);
        base.onBreak(world, x, y, z);
    }

    public bool DropContents(World world, BlockEntityChest chest)
    {
        int x = chest.X, y = chest.Y, z = chest.Z;
        for (int slot = 0; slot < chest.size(); ++slot)
        {
            ItemStack stack = chest.getStack(slot);
            if (stack != null)
            {
                float ox = random.NextFloat() * 0.8F + 0.1F;
                float oy = random.NextFloat() * 0.8F + 0.1F;
                float oz = random.NextFloat() * 0.8F + 0.1F;

                while (stack.count > 0)
                {
                    int amount = stack.count;
                    EntityItem item = new EntityItem(world, x + ox, y + oy, z + oz,
                        new ItemStack(stack.itemId, amount, stack.getDamage()));
                    float spread = 0.05F;
                    item.velocityX = random.NextGaussian() * spread;
                    item.velocityY = random.NextGaussian() * spread + 0.2F;
                    item.velocityZ = random.NextGaussian() * spread;
                    world.SpawnEntity(item);
                    return item != null;
                }
            }
        }
        return false;
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        IInventory inv = (BlockEntityChest)world.getBlockEntity(x, y, z);

        if (world.shouldSuffocate(x, y + 1, z)) return true;
        if (world.getBlockId(x - 1, y, z) == id && world.shouldSuffocate(x - 1, y + 1, z)) return true;
        if (world.getBlockId(x + 1, y, z) == id && world.shouldSuffocate(x + 1, y + 1, z)) return true;
        if (world.getBlockId(x, y, z - 1) == id && world.shouldSuffocate(x, y + 1, z - 1)) return true;
        if (world.getBlockId(x, y, z + 1) == id && world.shouldSuffocate(x, y + 1, z + 1)) return true;

        if (world.getBlockId(x - 1, y, z) == id) inv = new InventoryLargeChest("Large chest", (BlockEntityChest)world.getBlockEntity(x - 1, y, z), inv);
        if (world.getBlockId(x + 1, y, z) == id) inv = new InventoryLargeChest("Large chest", inv, (BlockEntityChest)world.getBlockEntity(x + 1, y, z));
        if (world.getBlockId(x, y, z - 1) == id) inv = new InventoryLargeChest("Large chest", (BlockEntityChest)world.getBlockEntity(x, y, z - 1), inv);
        if (world.getBlockId(x, y, z + 1) == id) inv = new InventoryLargeChest("Large chest", inv, (BlockEntityChest)world.getBlockEntity(x, y, z + 1));

        world.playSound(player, "random.door_open", 0.4F, 1.0F);
        if (world.isRemote) return true;

        player.openChestScreen(inv);
        return true;
    }

    protected override BlockEntity getBlockEntity() => new BlockEntityChest();
}
