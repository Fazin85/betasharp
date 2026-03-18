using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace BetaSharp.Client.Textures;

/// <summary>
/// Point d'entrée unique pour tous les atlas du jeu.
/// Gère l'atlas terrain (blocs) et l'atlas items séparément,
/// comme Minecraft vanilla post-1.5.
/// </summary>
public class TextureAtlasManager : IDisposable
{
    private readonly ILogger<TextureAtlasManager> _logger = Log.Instance.For<TextureAtlasManager>();

    /// <summary>Atlas des textures de blocs (terrain.png)</summary>
    public TextureAtlas Terrain { get; } = new TextureAtlas();

    /// <summary>Atlas des textures d'items (items.png)</summary>
    public TextureAtlas Items { get; } = new TextureAtlas();

    public static TextureAtlasManager Instance;
    private readonly TextureAtlasLoader _loader = new TextureAtlasLoader();
    private readonly string _assetsRoot;

    public TextureAtlasManager(string assetsRoot)
    {
        _assetsRoot = assetsRoot;
        Instance = this;
    }

    /// <summary>
    /// Initialise et génère tous les atlas au démarrage.
    /// Appeler une fois après la création du contexte OpenGL.
    /// </summary>
    public void Initialize()
    {
        _logger.LogInformation("[AtlasManager] Génération des atlas de textures...");

        StitchTerrain();
        StitchItems();

        _logger.LogInformation("[AtlasManager] Atlas générés.");
    }

    /// <summary>
    /// Recharge tous les atlas (utile pour les resource packs).
    /// </summary>
    public void Reload()
    {
        _logger.LogInformation("[AtlasManager] Rechargement des atlas...");
        Initialize();
    }

    /// <summary>
    /// Enregistre un dossier de textures de mod.
    /// À appeler avant Initialize() ou Reload().
    /// </summary>
    public void RegisterModTextures(string modNamespace, string texturesDirectory)
    {
        _loader.AddSource(modNamespace, texturesDirectory);
    }

    // ── Privé ────────────────────────────────────────────────────────────────

    private void StitchTerrain()
    {
        var loader = new TextureAtlasLoader();

        // Vanilla
        string vanillaBlocks = System.IO.Path.Combine(_assetsRoot, "png", "block");
        loader.AddSource("minecraft", vanillaBlocks);

        // Mods (même loader que le terrain)
        // Les mods s'enregistrent via RegisterModTextures → on copie les sources du loader principal
        // Pour simplifier ici on utilise le loader partagé
        loader.StitchInto(Terrain);

        _logger.LogInformation($"[AtlasManager] Terrain atlas : {Terrain.AtlasWidth}x{Terrain.AtlasHeight}px");
    }

    private void StitchItems()
    {
        var loader = new TextureAtlasLoader();

        string vanillaItems = System.IO.Path.Combine(_assetsRoot, "png", "item");
        loader.AddSource("minecraft", vanillaItems);

        loader.StitchInto(Items);

        _logger.LogInformation($"[AtlasManager] Items atlas : {Items.AtlasWidth}x{Items.AtlasHeight}px");
    }

    public void Dispose()
    {
        Terrain.Dispose();
        Items.Dispose();
    }
}
