using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using System.Reflection;

namespace betareborn.CommandParser;

public class GameCommands
{
    public static Dictionary<string, int> allItems = new();
    public static Dictionary<int, Block> blocks = new();
    public static Dictionary<int, Item> items = new();

    public GameCommands()
    {
        { // Get blocks
            var fields = typeof(Block).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(Block))
                .ToList();
            fields.ForEach(x =>
            {
                var block = ((Block)x.GetValue(null));
                var id = block.blockID;
                allItems.TryAdd(x.Name.ToLower(), id);
                blocks.TryAdd(id, block);
            });
        }


        { // Get items
            var fields = typeof(Item).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(Item))
                .ToList();
            fields.ForEach(x =>
            {
                var item = ((Item)x.GetValue(null));
                var id = item.shiftedIndex;
                allItems.TryAdd(x.Name.ToLower(), id);
                items.TryAdd(id, item);
            });
        }
    }

    [MinecraftCommand("clear")]
    public void ClearInventory(CommandContext ctx)
    {
        var inventory = ctx.Game.thePlayer.inventory.mainInventory;
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = null;
        }

        ctx.Reply("Inventory cleared.");
    }

    [MinecraftCommand("give")]
    public void GiveItem(CommandContext ctx, string itemName, int count = -1)
    {
        if (allItems.TryGetValue(itemName, out var itemId))
        {
            int finalCount = count;

            if (count == -1) finalCount = items.TryGetValue(itemId, out var item) ? item.maxStackSize : 64;

            ctx.Game.thePlayer.inventory.addItemStackToInventory(new ItemStack(id: itemId, count: finalCount));
            ctx.Reply($"Gave {finalCount} of {itemName}");
        }
        else
        {
            ctx.Reply($"Item '{itemName}' not found.");
        }
    }


    [MinecraftCommand("heal")]
    public void Heal(CommandContext ctx, int amount = 20)
    {
        ctx.Game.thePlayer.heal(amount);
    }

    [MinecraftCommand("settime")]
    public void SetTime(CommandContext ctx, string timeValue)
    {
        long? timeToSet = timeValue.ToLower() switch
        {
            "sunrise" or "dawn" => 0,
            "morning" => 1000,
            "noon" or "day" => 6000,
            "sunset" or "dusk" => 12000,
            "night" => 13000,
            "midnight" => 18000,
            _ => long.TryParse(timeValue, out long t) ? t : null
        };

        if (timeToSet.HasValue)
        {
            ctx.Game.theWorld.setWorldTime(timeToSet.Value);
            ctx.Reply($"Time set to {timeValue} ({timeToSet.Value})");
        }
        else
        {
            ctx.Reply($"Invalid time value: {timeValue}");
        }
    }

    // Doesn't work for some reason
    [MinecraftCommand("teleport", "tp")]
    public void Teleport(CommandContext ctx, float x, float y, float z)
    {
        ctx.Game.thePlayer.setPosition(x, y, z);
    }

    [MinecraftCommand("summon", "spawn")]
    public void Summon(CommandContext ctx, string name)
    {
        var p = ctx.Game.thePlayer;
        var ent = EntityList.createEntityAt(name, ctx.Game.theWorld, (float)p.posX, (float)p.posY, (float)p.posZ);

        if (ent == null)
        {
            Console.Error.WriteLine($"Entity created by createEntityInWorld is null `{name}`");
        }
    }

    [MinecraftCommand("killall")]
    public void KillAll(CommandContext ctx, string filter = "all")
    {
        var world = ctx.Game.theWorld;
        var entities = new List<Entity>(world.loadedEntityList);
        int count = 0;
        filter = filter.ToLower();

        foreach (var ent in entities)
        {
            if (ent is EntityPlayer) continue;

            bool shouldKill = filter switch
            {
                "all" => true,
                "living" or "mob" => ent is EntityLiving,
                "monster" => ent is EntityMob,
                "animal" => ent is EntityAnimal,
                "item" => ent is EntityItem,
                "tnt" => ent is EntityTNTPrimed,
                _ => EntityList.getEntityString(ent)?.Equals(filter, StringComparison.OrdinalIgnoreCase) ?? false
            };

            if (shouldKill)
            {
                world.setEntityDead(ent);
                count++;
            }
        }

        ctx.Reply($"Killed {count} entities (filter: {filter}).");
    }
}