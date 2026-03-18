using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Worlds.Gen.Structures;
public class NbtStructure : IStructure
{
    public string Name { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Depth { get; private set; }
    private int[] blocks;
    private int[] data;
    private List<StructureEntity> savedEntities = new();


    public NbtStructure(string name, int width, int height, int depth, int[] blocks, int[] data, List<StructureEntity> entities)
    {
        Name = name;
        Width = width;
        Height = height;
        Depth = depth;
        this.blocks = blocks;
        this.data = data;
        this.savedEntities = entities;
    }

    // Accès par coordonnées locales
    private int getIndex(int x, int y, int z) => y * Width * Depth + z * Width + x;

    public int getBlockId(int x, int y, int z) => blocks[getIndex(x, y, z)];
    public StructureEntity getEntityId(int i) => savedEntities[i];
    public int getBlockData(int x, int y, int z) => data[getIndex(x, y, z)];

    public List<StructureEntity> ents => savedEntities; // l'usage du intId fait crasher pour une certaine raison impliquant le serveur, ou il renvoie : IllegalArgumentExeption : Entity is already tracked !
    public byte[] blocksIds => blocks.Select(b => (byte)(b & 0xFF)).ToArray();
    public byte[] blockMetas => data.Select(b => (byte)(b & 0xFF)).ToArray();




    /*
    public byte[] encodeToByte(int[] var1) //
    {
        List<byte> byteList = new List<byte>();

        foreach (int number in var1)
        {
            byteList.AddRange(BitConverter.GetBytes(number));
        }

        byte[] byteArray = byteList.ToArray();
        return byteArray;
    }*/
    public StructureEntity getEnt(int x, int y, int z) => savedEntities[getIndex(x, y, z)];

    public void place(World world, int originX, int originY, int originZ, Random random)
    {
        // Blocs qui nécessitent un voisin pour exister
        HashSet<int> deferredBlocks = new HashSet<int> {
        64,  // wooden door
        71,  // iron door
        50,  // torch
        75,  // redstone torch off
        76,  // redstone torch on
        // ajoute d'autres si nécessaire (boutons, leviers, etc.)
    };

        // Passe 1 : tous les blocs normaux
        for (int y = 0; y < Height; y++)
            for (int z = 0; z < Depth; z++)
                for (int x = 0; x < Width; x++)
                {
                    int blockId = getBlockId(x, y, z);
                    int blockData = getBlockData(x, y, z);
                    if (blockId != 0 && !deferredBlocks.Contains(blockId))
                        world.setBlockWithMeta(originX + x, originY + y, originZ + z, blockId, blockData);
                }

        // Passe 2 : blocs dépendants, y de bas en haut pour les portes
        for (int y = 0; y < Height; y++)
            for (int z = 0; z < Depth; z++)
                for (int x = 0; x < Width; x++)
                {
                    int blockId = getBlockId(x, y, z);
                    int blockData = getBlockData(x, y, z);
                    if (blockId != 0 && deferredBlocks.Contains(blockId))
                        world.setBlockWithMeta(originX + x, originY + y, originZ + z, blockId, blockData);
                }
        //
        foreach (StructureEntity se in savedEntities)
        {
            float ax, ay, az;
            ax = originX + se.LocalX;
            ay = originY + se.LocalY;
            az = originZ + se.LocalZ;
            Entity ent = EntityRegistry.createEntityAt(se.EntityId, world, ax, ay, az);
            if (ent == null)
            {
                Console.WriteLine($"[WARNING] Entité inconnue : {se.EntityId}");
                continue;
            }

            ent.setPosition(
                ax,
                ay,
                az
            );

            if (se.ExtraData != null)
                ent.readNbt(se.ExtraData);

            world.SpawnEntity(ent);
        }

    }
    public class StructureEntity
    {
        public string EntityId { get; set; }  // ex: "Zombie", "Chest" 
        public float LocalX { get; set; }     // position relative à l'origine
        public float LocalY { get; set; }
        public float LocalZ { get; set; }
        public NBTTagCompound ExtraData { get; set; } // données optionnelles (équipement, etc.)
    }
}
