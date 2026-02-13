using BetaSharp.Blocks;

namespace BetaSharp;

public class Session(string username, string sessionId)
{
    public static List<Block> RegisteredBlocksList { get; private set; } = [];
    public string username = username;
    public string sessionId = sessionId;

    static Session()
    {
        RegisteredBlocksList.Add(Block.STONE);
        RegisteredBlocksList.Add(Block.COBBLESTONE);
        RegisteredBlocksList.Add(Block.BRICKS);
        RegisteredBlocksList.Add(Block.DIRT);
        RegisteredBlocksList.Add(Block.PLANKS);
        RegisteredBlocksList.Add(Block.LOG);
        RegisteredBlocksList.Add(Block.LEAVES);
        RegisteredBlocksList.Add(Block.TORCH);
        RegisteredBlocksList.Add(Block.SLAB);
        RegisteredBlocksList.Add(Block.GLASS);
        RegisteredBlocksList.Add(Block.MOSSY_COBBLESTONE);
        RegisteredBlocksList.Add(Block.SAPLING);
        RegisteredBlocksList.Add(Block.DANDELION);
        RegisteredBlocksList.Add(Block.ROSE);
        RegisteredBlocksList.Add(Block.BROWN_MUSHROOM);
        RegisteredBlocksList.Add(Block.RED_MUSHROOM);
        RegisteredBlocksList.Add(Block.SAND);
        RegisteredBlocksList.Add(Block.GRAVEL);
        RegisteredBlocksList.Add(Block.SPONGE);
        RegisteredBlocksList.Add(Block.WOOL);
        RegisteredBlocksList.Add(Block.COAL_ORE);
        RegisteredBlocksList.Add(Block.IRON_ORE);
        RegisteredBlocksList.Add(Block.GOLD_ORE);
        RegisteredBlocksList.Add(Block.IRON_BLOCK);
        RegisteredBlocksList.Add(Block.GOLD_BLOCK);
        RegisteredBlocksList.Add(Block.BOOKSHELF);
        RegisteredBlocksList.Add(Block.TNT);
        RegisteredBlocksList.Add(Block.OBSIDIAN);
    }
}