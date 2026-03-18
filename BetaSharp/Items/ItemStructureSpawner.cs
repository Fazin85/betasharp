using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Gen.Structures;
using Microsoft.Extensions.Logging;
using static BetaSharp.Worlds.Gen.Structures.NbtStructure;

namespace BetaSharp.Items;

public class ItemStructureSaver : Item
{
    private bool cornerASet = false;
    private int cornerAX;
    private int cornerAY;
    private int cornerAZ;

    public ItemStructureSaver(int id) : base(id)
    {
        setItemName("structureSaver");
        setMaxCount(1);
    }

    // Clic droit sur un bloc = définir coin A ou B et sauvegarder
    public override bool useOnBlock(ItemStack itemStack, EntityPlayer player, World world, int x, int y, int z, int face)
    {
        if (world.isRemote) return true; // logique serveur uniquement

        if (!cornerASet)
        {
            cornerAX = x; cornerAY = y; cornerAZ = z;
            cornerASet = true;
            player.sendMessage($"[Structure] Coin A défini : {x} {y} {z}");
        }
        else
        {
            int minX = Math.Min(cornerAX, x), maxX = Math.Max(cornerAX, x);
            int minY = Math.Min(cornerAY, y), maxY = Math.Max(cornerAY, y);
            int minZ = Math.Min(cornerAZ, z), maxZ = Math.Max(cornerAZ, z);

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            int depth = maxZ - minZ + 1;

            player.sendMessage($"[Structure] Coin B défini : {x} {y} {z} — région {width}x{height}x{depth}");

            // Capture les blocs
            byte[] blocks = new byte[width * height * depth];
            byte[] data = new byte[width * height * depth];

            for (int iy = 0; iy < height; iy++)
                for (int iz = 0; iz < depth; iz++)
                    for (int ix = 0; ix < width; ix++)
                    {
                        int idx = iy * width * depth + iz * width + ix;
                        blocks[idx] = (byte)world.getBlockId(minX + ix, minY + iy, minZ + iz);
                        data[idx] = (byte)world.getBlockMeta(minX + ix, minY + iy, minZ + iz);
                    }

            // Capture les entités dans la région
            List<StructureEntity> structureEntities = new();
            var regionEntities = world.getClosestEntities(player, 16d);

            foreach (Entity ent in regionEntities)
            {
                string entId = EntityRegistry.GetId(ent);
                if (entId == null) continue;

                NBTTagCompound entData = new NBTTagCompound();
                ent.writeNbt(entData);

                structureEntities.Add(new StructureEntity
                {
                    EntityId = entId,
                    LocalX = (float)(ent.x - minX),
                    LocalY = (float)(ent.y - minY),
                    LocalZ = (float)(ent.z - minZ),
                    ExtraData = entData
                });
            }
            // Debug
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] != 0)
                    Console.WriteLine($"[DEBUG] idx={i} blockId={blocks[i]} meta={data[i]}, pos : x: {blocks[i]} y: z:");
            // Sauvegarde
            string name = $"structure_{DateTime.Now:yyyyMMdd_HHmmss}";
            NbtStructure structure = new NbtStructure(
                name, width, height, depth,
                blocks.Select(b => (int)b).ToArray(),
                data.Select(b => (int)b).ToArray(),
                structureEntities);

            Directory.CreateDirectory("structures");
            string path = $"structures/{name}.nbt";

            try
            {
                NbtStructureLoader.save(structure, System.IO.File.OpenWrite(path));
                player.sendMessage($"[Structure] Sauvegardée : {path}");
            }
            catch (Exception e)
            {
                player.sendMessage($"[Structure] Erreur : {e.Message}");
                _logger.LogError(e, "Erreur lors de la sauvegarde de la structure");
            }

            cornerASet = false; // reset pour la prochaine capture
        }

        return true;
    }

    // Clic droit dans le vide = reset la sélection
    public override ItemStack AltFire(ItemStack itemStack, World world, EntityPlayer player)
    {
        if (cornerASet)
        {
            cornerASet = false;
            player.sendMessage("[Structure] Sélection annulée.");
        }
        else
        {
            player.sendMessage("[Structure] Clic droit sur un bloc pour définir le coin A.");
        }
        return itemStack;
    }
}
