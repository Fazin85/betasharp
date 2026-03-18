using BetaSharp.Worlds.Gen.Structures;

public static class StructureManager
{
    private static Dictionary<string, NbtStructure> cache = new();
    private static string structuresPath = "./assets/structures/"; // dossier relatif au jeu

    public static NbtStructure get(string name)
    {
        if (!cache.ContainsKey(name))
        {
            string path = structuresPath + name + ".nbt";
            if (!File.Exists(path))
            {
                Console.WriteLine($"[WARNING] Structure introuvable : {path}");
                return null;
            }
            cache[name] = NbtStructureLoader.load(path);
        }
        return cache[name];
    }

    public static void clearCache() => cache.Clear();
}
