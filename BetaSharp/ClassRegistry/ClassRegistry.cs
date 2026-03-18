using System.Reflection;
using System.Text;
using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Registry;

// ═══════════════════════════════════════════════════════════════════════════
// ATTRIBUTS DE REGISTRATION
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Marque une classe Block pour auto-registration au démarrage.
/// Le constructeur doit accepter (int id, string textureId) ou (int id).
///
/// Exemple :
///   [RegisterBlock(id: 200, textureId: 10, hardness: 1.5f, name: "ruby_ore")]
///   public class BlockRuby : BlockOre { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RegisterBlockAttribute : Attribute
{
    public int Id { get; }
    public string TextureId { get; }
    public float Hardness { get; }
    public float Resistance { get; }
    public float Luminance { get; }
    public string Name { get; }
    public bool TickRandomly { get; }

    public RegisterBlockAttribute(
        int id,
        string textureId = "",
        float hardness = 1.0f,
        float resistance = 0.0f,
        float luminance = 0.0f,
        string name = "",
        bool tickRandomly = false)
    {
        Id = id;
        TextureId = textureId;
        Hardness = hardness;
        Resistance = resistance;
        Luminance = luminance;
        Name = name;
        TickRandomly = tickRandomly;
    }
}

/// <summary>
/// Marque une classe Item pour auto-registration au démarrage.
/// Le constructeur doit accepter (int id).
///
/// Exemple :
///   [RegisterItem(id: 300, maxCount: 1, name: "ruby_sword", textureX: 5, textureY: 3)]
///   public class ItemRubySword : ItemSword { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RegisterItemAttribute : Attribute
{
    public int Id { get; }
    public int MaxCount { get; }
    public string Name { get; }
    public int TextureX { get; }
    public int TextureY { get; }

    public string TextureID { get; }
    public RegisterItemAttribute(
        int id,
        int maxCount = 99,
        string name = "",
        string textureId = "stick") // default texture
    {
        Id = id;
        MaxCount = maxCount;
        Name = name;
        
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// REGISTRE CENTRAL
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Registre central du jeu — deux modes coexistent pendant la migration :
///
///   MODE STATIQUE  (legacy) : blocs/items déclarés manuellement dans Block.cs / Item.cs
///                             → collectés depuis Block.Blocks[] et Item.ITEMS[]
///
///   MODE DYNAMIQUE (nouveau): classes annotées [RegisterBlock] / [RegisterItem]
///                             → instanciées automatiquement par réflexion
///
/// Les deux modes sont actifs simultanément.
/// Migrer un bloc = ajouter l'attribut sur sa classe et retirer la ligne statique dans Block.cs.
/// </summary>
public static class ClassRegistry
{
    private static readonly ILogger _logger = Log.Instance.For<GameRegistryMarker>();

    // ── Données publiques ─────────────────────────────────────────────────

    public static IReadOnlyList<Type> BlockTypes => _blockTypes;
    public static IReadOnlyList<Type> ItemTypes => _itemTypes;
    public static IReadOnlyList<Block> RegisteredBlocks => _registeredBlocks;
    public static IReadOnlyList<Item> RegisteredItems => _registeredItems;
    public static IReadOnlyList<Type> DynamicBlockTypes => _dynamicBlockTypes;
    public static IReadOnlyList<Type> DynamicItemTypes => _dynamicItemTypes;

    private static readonly List<Type> _blockTypes = new();
    private static readonly List<Type> _itemTypes = new();
    private static readonly List<Block> _registeredBlocks = new();
    private static readonly List<Item> _registeredItems = new();
    private static readonly List<Type> _dynamicBlockTypes = new();
    private static readonly List<Type> _dynamicItemTypes = new();

    // ── API principale ────────────────────────────────────────────────────

    /// <summary>
    /// Initialise le registre en trois étapes :
    ///   1. Instancie les classes annotées [RegisterBlock] / [RegisterItem]
    ///   2. Collecte Block.Blocks[] / Item.ITEMS[] (statique legacy)
    ///   3. Indexe tous les types par réflexion pour DumpClasses()
    ///
    /// À appeler UNE FOIS au démarrage, après le chargement des assemblies.
    /// </summary>
    public static void AutoRegister(params Assembly[]? extraAssemblies)
    {
        _blockTypes.Clear();
        _itemTypes.Clear();
        _registeredBlocks.Clear();
        _registeredItems.Clear();
        _dynamicBlockTypes.Clear();
        _dynamicItemTypes.Clear();

        var assemblies = new HashSet<Assembly> { Assembly.GetExecutingAssembly() };
        if (extraAssemblies != null)
            foreach (Assembly a in extraAssemblies)
                assemblies.Add(a);

        // ── Étape 1 : mode dynamique ──────────────────────────────────────
        int dynBlocks = 0, dynItems = 0;

        foreach (Assembly asm in assemblies)
        {
            foreach (Type t in asm.GetTypes())
            {
                if (t.IsAbstract) continue;

                var ba = t.GetCustomAttribute<RegisterBlockAttribute>();
                if (ba != null && t.IsSubclassOf(typeof(Block)))
                {
                    if (TryInstantiateBlock(t, ba))
                    {
                        _dynamicBlockTypes.Add(t);
                        dynBlocks++;
                    }
                }

                var ia = t.GetCustomAttribute<RegisterItemAttribute>();
                if (ia != null && t.IsSubclassOf(typeof(Item)))
                {
                    if (TryInstantiateItem(t, ia))
                    {
                        _dynamicItemTypes.Add(t);
                        dynItems++;
                    }
                }
            }
        }

        // ── Étape 2 : mode statique (legacy) ─────────────────────────────
        foreach (Block? b in Block.Blocks)
            if (b != null && !_registeredBlocks.Contains(b))
                _registeredBlocks.Add(b);

        foreach (Item? i in Item.ITEMS)
            if (i != null && !_registeredItems.Contains(i))
                _registeredItems.Add(i);

        // ── Étape 3 : indexer tous les types ─────────────────────────────
        foreach (Assembly asm in assemblies)
        {
            foreach (Type t in asm.GetTypes())
            {
                if (t.IsAbstract) continue;
                if (t.IsSubclassOf(typeof(Block))) _blockTypes.Add(t);
                if (t.IsSubclassOf(typeof(Item))) _itemTypes.Add(t);
            }
        }

        _logger.LogInformation(
            $"[ClassRegistry] Initialisé — " +
            $"Blocs : {_registeredBlocks.Count} ({dynBlocks} dynamiques), " +
            $"Items : {_registeredItems.Count} ({dynItems} dynamiques), " +
            $"Types : {_blockTypes.Count} Block / {_itemTypes.Count} Item.");
    }

    /// <summary>
    /// Rapport complet de l'état du registre.
    /// Distingue statique/dynamique, signale les trous et les types orphelins.
    /// </summary>
    public static void DumpClasses()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("╔══════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║                   GAME REGISTRY DUMP                        ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════════╝");

        // ── Blocs ──────────────────────────────────────────────────────────
        sb.AppendLine();
        sb.AppendLine($"▶ BLOCKS — {_registeredBlocks.Count} instances | {_blockTypes.Count} types");
        sb.AppendLine("   ID  │ Mode      │ Type                          │ Nom");
        sb.AppendLine("───────┼───────────┼───────────────────────────────┼──────────────────");

        for (int id = 0; id < Block.Blocks.Length; id++)
        {
            Block? b = Block.Blocks[id];
            if (b == null) continue;
            bool dyn = _dynamicBlockTypes.Contains(b.GetType());
            string mode = dyn ? "[dynamic]" : "[static] ";
            sb.AppendLine($"  {id,4} │ {mode} │ {b.GetType().Name,-30} │ {b.getBlockName()}");
        }

        var blockGaps = FindGaps(Block.Blocks.Select(b => b != null).ToArray(), 1, 254);
        if (blockGaps.Count > 0)
            sb.AppendLine($"\n  ⚠  IDs libres (blocs) : {string.Join(", ", blockGaps)}");

        var typesWithBlockInstance = new HashSet<Type>(_registeredBlocks.Select(b => b.GetType()));
        var orphanBlockTypes = _blockTypes.Where(t => !typesWithBlockInstance.Contains(t)).ToList();
        if (orphanBlockTypes.Count > 0)
        {
            sb.AppendLine("\n  ⚠  Types Block sans instance :");
            foreach (Type t in orphanBlockTypes)
                sb.AppendLine($"     - {t.FullName}");
        }

        // ── Items ──────────────────────────────────────────────────────────
        sb.AppendLine();
        sb.AppendLine($"▶ ITEMS — {_registeredItems.Count} instances | {_itemTypes.Count} types");
        sb.AppendLine("   ID   │ Mode      │ Type                          │ Nom");
        sb.AppendLine("────────┼───────────┼───────────────────────────────┼──────────────────");

        for (int id = 0; id < Item.ITEMS.Length; id++)
        {
            Item? item = Item.ITEMS[id];
            if (item == null) continue;
            bool dyn = _dynamicItemTypes.Contains(item.GetType());
            string mode = dyn ? "[dynamic]" : "[static] ";
            sb.AppendLine($"  {id,5} │ {mode} │ {item.GetType().Name,-30} │ {item.getItemName()}");
        }

        var itemGaps = FindGaps(Item.ITEMS.Select(i => i != null).ToArray(), 256, 511);
        if (itemGaps.Count > 0)
            sb.AppendLine($"\n  ⚠  IDs libres (items) : {string.Join(", ", itemGaps.Take(20))}" +
                          (itemGaps.Count > 20 ? $" (+{itemGaps.Count - 20} autres)" : ""));

        var typesWithItemInstance = new HashSet<Type>(_registeredItems.Select(i => i.GetType()));
        var orphanItemTypes = _itemTypes.Where(t => !typesWithItemInstance.Contains(t)).ToList();
        if (orphanItemTypes.Count > 0)
        {
            sb.AppendLine("\n  ⚠  Types Item sans instance :");
            foreach (Type t in orphanItemTypes)
                sb.AppendLine($"     - {t.FullName}");
        }

        // ── État de migration ──────────────────────────────────────────────
        sb.AppendLine();
        sb.AppendLine("▶ MIGRATION");
        sb.AppendLine($"  Blocs  statiques  : {_registeredBlocks.Count - _dynamicBlockTypes.Count}  → à migrer vers [RegisterBlock]");
        sb.AppendLine($"  Blocs  dynamiques : {_dynamicBlockTypes.Count}");
        sb.AppendLine($"  Items  statiques  : {_registeredItems.Count - _dynamicItemTypes.Count}  → à migrer vers [RegisterItem]");
        sb.AppendLine($"  Items  dynamiques : {_dynamicItemTypes.Count}");

        // ── Résumé ─────────────────────────────────────────────────────────
        int warnings = blockGaps.Count + itemGaps.Count +
                       orphanBlockTypes.Count + orphanItemTypes.Count;
        sb.AppendLine();
        sb.AppendLine(warnings == 0
            ? "  V  Registre propre — aucun problème détecté."
            : $"  X  {warnings} problème(s) — voir les avertissements ci-dessus.");
        sb.AppendLine("════════════════════════════════════════════════════════════════");

        _logger.LogInformation(sb.ToString());
    }

    public static T? GetBlock<T>() where T : Block
        => _registeredBlocks.OfType<T>().FirstOrDefault();

    public static T? GetItem<T>() where T : Item
        => _registeredItems.OfType<T>().FirstOrDefault();

    // ── Privé ─────────────────────────────────────────────────────────────

    private static bool TryInstantiateBlock(Type type, RegisterBlockAttribute attr)
    {
        if (Block.Blocks[attr.Id] != null)
        {
            _logger.LogWarning(
                $"[ClassRegistry] Conflit ID bloc {attr.Id} : " +
                $"{Block.Blocks[attr.Id]!.GetType().Name} vs {type.Name} — {type.Name} ignoré.");
            return false;
        }

        Block? instance = null;

        // Essai (int id, string textureId)
        try { instance = (Block?)Activator.CreateInstance(type, attr.Id, attr.TextureId); }
        catch { /* essai suivant */ }

        // Essai (int id)
        if (instance == null)
        {
            try { instance = (Block?)Activator.CreateInstance(type, attr.Id); }
            catch (Exception e)
            {
                _logger.LogError($"[ClassRegistry] Impossible d'instancier {type.Name} : {e.Message}");
                return false;
            }
        }

        if (instance == null) return false;

        if (attr.Hardness > 0) instance.SetHardness(attr.Hardness);
        if (attr.Resistance > 0) instance.SetResistance(attr.Resistance);
        if (attr.Luminance > 0) instance.SetLuminance(attr.Luminance);
        if (!string.IsNullOrEmpty(attr.Name)) instance.setBlockName(attr.Name);
        if (attr.TickRandomly) instance.SetTickRandom(true);

        _logger.LogDebug($"[ClassRegistry] [dynamic] Bloc {type.Name} enregistré (ID {attr.Id})");
        return true;
    }

    private static bool TryInstantiateItem(Type type, RegisterItemAttribute attr)
    {
        int arrayId = attr.Id + 256;
        if (arrayId < Item.ITEMS.Length && Item.ITEMS[arrayId] != null)
        {
            _logger.LogWarning(
                $"[ClassRegistry] Conflit ID item {attr.Id} : " +
                $"{Item.ITEMS[arrayId]!.GetType().Name} vs {type.Name} — {type.Name} ignoré.");
            return false;
        }

        Item? instance = null;
        try { instance = (Item?)Activator.CreateInstance(type, attr.Id); }
        catch (Exception e)
        {
            _logger.LogError($"[ClassRegistry] Impossible d'instancier {type.Name} : {e.Message}");
            return false;
        }

        if (instance == null) return false;

        instance.setMaxCount(attr.MaxCount);
        if (!string.IsNullOrEmpty(attr.Name))
            instance.setItemName(attr.Name);
        if (attr.TextureX != 0 || attr.TextureY != 0)
            instance.setTextureId(attr.TextureX, attr.TextureY);

        _logger.LogDebug($"[ClassRegistry] [dynamic] Item {type.Name} enregistré (ID {attr.Id})");
        return true;
    }

    private static List<int> FindGaps(bool[] occupied, int minId, int maxId)
    {
        var gaps = new List<int>();
        for (int i = minId; i <= Math.Min(maxId, occupied.Length - 1); i++)
            if (!occupied[i]) gaps.Add(i);
        return gaps;
    }

    private class GameRegistryMarker { }
}
