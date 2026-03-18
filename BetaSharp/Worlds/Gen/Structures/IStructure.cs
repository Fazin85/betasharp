using System;
using System.Collections.Generic;
using System.Text;
using static BetaSharp.Worlds.Gen.Structures.NbtStructure;

namespace BetaSharp.Worlds.Gen.Structures;
public interface IStructure
{

}
public static class TestStructures
{
    public static NbtStructure createTestHut()
    {
        int width = 7, height = 5, depth = 7;
        byte[] blocks = new byte[width * height * depth];
        byte[] data = new byte[width * height * depth];

        int log = 17, plank = 5, glass = 20;
        int door = 64, stair = 53, chest = 54;
        int stone = 1, torch = 50;

        void set(int x, int y, int z, int blockId, int meta = 0)
        {
            int i = y * width * depth + z * width + x;
            blocks[i] = (byte)blockId;
            data[i] = (byte)meta;
        }

        // Sol en planches (y=0)
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                set(x, 0, z, plank);

        // Poteaux aux coins (y=1 à y=3)
        for (int y = 1; y <= 3; y++)
        {
            set(0, y, 0, log); set(6, y, 0, log);
            set(0, y, 6, log); set(6, y, 6, log);
        }

        // Murs avant (z=0) et arrière (z=6)
        for (int y = 1; y <= 3; y++)
            for (int x = 1; x <= 5; x++)
            {
                set(x, y, 0, plank);
                set(x, y, 6, plank);
            }

        // Murs gauche (x=0) et droit (x=6)
        for (int y = 1; y <= 3; y++)
            for (int z = 1; z <= 5; z++)
            {
                set(0, y, z, plank);
                set(6, y, z, plank);
            }

        // Fenêtres (y=2)
        set(2, 2, 0, glass); set(4, 2, 0, glass);
        set(2, 2, 6, glass); set(4, 2, 6, glass);
        set(0, 2, 2, glass); set(0, 2, 4, glass);
        set(6, 2, 2, glass); set(6, 2, 4, glass);

        // Porte avant centre
        set(3, 1, 0, door, 0);
        set(3, 2, 0, door, 8);

        // Escaliers
        set(1, 1, 1, stair, 2);
        set(1, 2, 2, stair, 2);

        // Coffres
        set(2, 1, 5, chest, 2);
        set(4, 1, 5, chest, 2);

        // Torches
        set(1, 2, 1, torch, 0);
        set(5, 2, 1, torch, 0);
        set(1, 2, 5, torch, 0);
        set(5, 2, 5, torch, 0);

        // Toit
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                set(x, 4, z, plank);

        // Plafond intérieur
        for (int z = 1; z <= 5; z++)
            for (int x = 1; x <= 5; x++)
                set(x, 4, z, stone);

        // Entités — centre de la cabane
        List<StructureEntity> entities = new List<StructureEntity>
        {
            // Zombie au centre
            new StructureEntity { EntityId = "Zombie", LocalX = 3.5f, LocalY = 1f, LocalZ = 3.5f },
            // Cochon dans un coin
            new StructureEntity { EntityId = "Pig",    LocalX = 1.5f, LocalY = 1f, LocalZ = 2.5f },
        };

        NbtStructure hut = new NbtStructure("test_hut", width, height, depth,
            blocks.Select(b => (int)b).ToArray(),
            data.Select(b => (int)b).ToArray(),
            entities);

        return hut;
    }
}
