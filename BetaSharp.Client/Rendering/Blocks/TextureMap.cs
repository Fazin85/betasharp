namespace BetaSharp.Client.Textures;

/// <summary>
/// Table de correspondance entre les anciens IDs de texture entiers (système terrain.png 16×16)
/// et les noms de textures dans le nouvel atlas.
///
/// FORMAT DE NOM : "namespace:blocks/nom" ou "namespace:items/nom"
///
/// MIGRATION : ajouter une entrée ici suffit pour qu'un textureId utilise l'atlas.
/// Les IDs non mappés tombent automatiquement sur le fallback legacy (terrain.png).
///
/// RÉFÉRENCE GRILLE terrain.png 16×16 :
///   id = col + row * 16  (col et row commencent à 0)
///   col = id & 15
///   row = (id >> 4) & 15
///
///   0   = grass_top          16  = cobblestone         32  = gold_ore
///   1   = stone              17  = bedrock              33  = iron_ore
///   2   = dirt               18  = sand                 34  = coal_ore
///   3   = grass_side_overlay 19  = gravel              35  = bookshelf_top (wood)
///   4   = planks_oak         20  = log_oak_top         36  = mossy_cobblestone
///   5   = (stone_slab_side)  21  = iron_block          37  = obsidian
///   6   = (stone_slab_top)   22  = gold_block          38  = grass_side_tinted
///   7   = brick              23  = diamond_block        39  = tall_grass
///   8   = tnt_side           24  = chest_top           49  = glass
///   9   = (web)              28  = mushroom_red_top     50  = diamond_ore
///   10  = rose               29  = mushroom_brown_top   51  = redstone_ore
///   11  = dandelion          31  = fire_0               52  = mob_spawner
///   12  = (portal)           32  = gold_ore             55  = dead_bush
///   13  = (cobweb)           ...                        65  = torch_on
///   14  = (portal frame)
///   15  = sapling_oak
/// </summary>
public static class TextureIdMap
{
    // ── Table principale  id → nom atlas ─────────────────────────────────
    private static readonly Dictionary<int, string> _map = new()
    {
        // ── Ligne 0 (id 0–15) ──────────────────────────────────────────
        [0] = "grass_top",
        [1] = "stone",
        [2] = "dirt",
        [3] = "grass_side_overlay",
        [4] = "planks_oak",
        [7] = "brick",
        [8] = "tnt_side",
        [10] = "flower_rose",
        [11] = "flower_dandelion",
        [13] = "web",
        [15] = "sapling_oak",

        // ── Ligne 1 (id 16–31) ─────────────────────────────────────────
        [16] = "cobblestone",
        [17] = "bedrock",
        [18] = "sand",
        [19] = "gravel",
        [20] = "log_oak_top",
        [21] = "iron_block",
        [22] = "gold_block",
        [23] = "diamond_block",
        [24] = "chest_top",
        [25] = "chest_front",
        [26] = "chest_side",
        [28] = "mushroom_red_top",
        [29] = "mushroom_brown_top",
        [31] = "fire_0",

        // ── Ligne 2 (id 32–47) ─────────────────────────────────────────
        [32] = "gold_ore",
        [33] = "iron_ore",
        [34] = "coal_ore",
        [35] = "planks_oak",           // bookshelf côtés
        [36] = "cobblestone_mossy",
        [37] = "obsidian",
        [38] = "grass_side_tinted",
        [39] = "tallgrass",

        // ── Ligne 3 (id 48–63) ─────────────────────────────────────────
        [49] = "glass",
        [50] = "diamond_ore",
        [51] = "redstone_ore",
        [52] = "mob_spawner",
        [55] = "deadbush",

        // ── Ligne 4 (id 64–79) ─────────────────────────────────────────
        [65] = "torch_on",
        [66] = "snow",
        [67] = "ice",
        [70] = "cactus_side",
        [72] = "clay",
        [73] = "reeds",
        [74] = "jukebox_side",

        // ── Ligne 5 (id 80–95) ─────────────────────────────────────────
        [80] = "torch_on",             // torche inventaire
        [83] = "ladder",
        [88] = "wheat_stage_7",

        // ── Ligne 6 (id 96–111) ────────────────────────────────────────
        [96] = "lever",
        [99] = "redstone_torch_on",
        [102] = "pumpkin_top",
        [103] = "netherrack",
        [104] = "soul_sand",
        [105] = "glowstone",
        [106] = "piston_top_sticky",
        [107] = "piston_top",

        // ── Ligne 7 (id 112–127) ───────────────────────────────────────
        [115] = "redstone_torch_off",
        [121] = "cake_top",

        // ── Ligne 8 (id 128–143) ───────────────────────────────────────
        [128] = "rail_normal",

        // ── Ligne 9 (id 144–159) ───────────────────────────────────────
        [144] = "lapis_block",

        // ── Ligne 10 (id 160–175) ──────────────────────────────────────
        [160] = "lapis_ore",
        [164] = "redstone_dust_cross",

        // ── Ligne 11 (id 176–191) ──────────────────────────────────────
        [179] = "rail_golden",

        // ── Ligne 12 (id 192–207) ──────────────────────────────────────
        [195] = "rail_detector",
    };

    // ── Noms alternatifs pour un même ID (faces multiples) ────────────────
    // Certains blocs ont plusieurs faces avec des noms différents mais le même ID de base.
    // Ces overrides sont utilisés via GetName(id, face).
    private static readonly Dictionary<(int id, int face), string> _faceOverrides = new()
    {
        // Log : dessus/dessous vs côtés
        [(17, 0)] = "log_oak_top",     // dessus
        [(17, 1)] = "log_oak_top",     // dessous
        [(17, 2)] = "log_oak",         // nord
        [(17, 3)] = "log_oak",         // sud
        [(17, 4)] = "log_oak",         // est
        [(17, 5)] = "log_oak",         // ouest

        
        [(2, 1)] = "grass_top",
        [(2, 2)] = "grass_side",
        [(2, 3)] = "grass_side",
        [(2, 4)] = "grass_side",
        [(2, 5)] = "grass_side",

        // TNT
        [(8, 1)] = "tnt_top",
        [(8, 0)] = "tnt_bottom",

        // Citrouille
        [(102, 2)] = "pumpkin_face",
        [(102, 3)] = "pumpkin_side",
        [(102, 4)] = "pumpkin_side",
        [(102, 5)] = "pumpkin_side",
    };

    // ── API publique ──────────────────────────────────────────────────────

    /// <summary>
    /// Retourne le nom de texture atlas pour un textureId.
    /// Retourne null si non mappé (→ fallback legacy).
    /// </summary>
    public static string? GetName(int textureId)
        => _map.TryGetValue(textureId, out string? name) ? name : null;

    public static int GetIndex(string textureId)
    {
        return 0;
    }
    
    /// <summary>
    /// Retourne le nom de texture atlas pour un textureId avec une face spécifique.
    /// Vérifie d'abord les overrides par face, puis la table principale.
    /// </summary>
    public static string? GetName(int blockId, int face)
    {
        if (_faceOverrides.TryGetValue((blockId, face), out string? faceName))
            return faceName;
        return GetName(blockId);
    }

    /// <summary>
    /// Retourne true si ce textureId est mappé vers l'atlas.
    /// </summary>
    public static bool IsMapped(int textureId) => _map.ContainsKey(textureId);

    /// <summary>
    /// Enregistre un mapping supplémentaire (pour les mods ou la migration progressive).
    /// </summary>
    public static void Register(int textureId, string atlasName)
        => _map[textureId] = atlasName;

    /// <summary>
    /// Retourne tous les IDs mappés — utile pour vérifier la couverture.
    /// </summary>
    public static IEnumerable<(int Id, string Name)> GetAll()
        => _map.Select(kv => (kv.Key, kv.Value));
}
