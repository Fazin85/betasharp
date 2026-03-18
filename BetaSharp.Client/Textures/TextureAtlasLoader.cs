using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Textures;

/// <summary>
/// Scanne les dossiers de textures et alimente un TextureAtlas.
/// Supporte les textures vanilla et les textures de mods.
/// </summary>
public class TextureAtlasLoader
{
    private readonly ILogger<TextureAtlasLoader> _logger = Log.Instance.For<TextureAtlasLoader>();

    // Dossiers scannés, dans l'ordre de priorité (le dernier écrase le précédent)
    private readonly List<(string Namespace, string Directory)> _sources = new();

    /// <summary>
    /// Enregistre un dossier source de textures.
    /// Le namespace sert de préfixe : "minecraft:stone", "mymod:custom_block"
    /// </summary>
    public void AddSource(string @namespace, string directory)
    {
        if (!Directory.Exists(directory))
        {
            _logger.LogWarning($"[AtlasLoader] Dossier introuvable : '{directory}'");
            return;
        }

        _sources.Add((@namespace, directory));
        _logger.LogInformation($"[AtlasLoader] Source ajoutée : '{@namespace}' → '{directory}'");
    }

    /// <summary>
    /// Scanne toutes les sources et retourne le dictionnaire nom → chemin
    /// prêt à être passé à TextureAtlas.Stitch().
    /// 
    /// Le nom d'une texture est : "namespace:nom_fichier_sans_extension"
    /// Ex: "minecraft:stone", "minecraft:grass_top", "mymod:ruby_ore"
    /// </summary>
    public Dictionary<string, string> BuildTexturePaths()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (ns, dir) in _sources)
        {
            try
            {
                var files = Directory.EnumerateFiles(dir, "*.png", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    // Construit la clé relative au dossier source
                    string relative = System.IO.Path.GetRelativePath(dir, file);
                    string key = BuildKey(ns, relative);

                    if (result.ContainsKey(key))
                        _logger.LogDebug($"[AtlasLoader] Override : '{key}' remplacé par '{file}'");

                    result[key] = file;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"[AtlasLoader] Erreur lors du scan de '{dir}' : {e.Message}");
            }
        }

        _logger.LogInformation($"[AtlasLoader] {result.Count} textures trouvées au total.");
        return result;
    }

    /// <summary>
    /// Construit et uploade directement un atlas depuis les sources enregistrées.
    /// </summary>
    public void StitchInto(TextureAtlas atlas)
    {
        var paths = BuildTexturePaths();
        atlas.Stitch(paths);
    }
    private static UVRegion ResolveUV(string textureId)
    {
        var atlas = TextureAtlasManager.Instance?.Terrain;
        if (atlas == null)
        {
            Console.WriteLine("ATLAS NULL");
            return default;
        }
        var uv = atlas.GetUV(textureId);
        Console.WriteLine($"ResolveUV({textureId}) → {uv.U0},{uv.V0},{uv.U1},{uv.V1}");
        return uv;
    }

    // ── Privé ────────────────────────────────────────────────────────────────

    private static string BuildKey(string ns, string relativePath)
    {
        string normalized = relativePath
            .Replace('\\', '/')
            .Replace(".png", "", StringComparison.OrdinalIgnoreCase);
        int slash = normalized.LastIndexOf('/');
        if (slash >= 0)
            normalized = normalized[(slash + 1)..];

        // Pas de namespace — les textureId sont des noms courts partout dans le code
        return normalized;
    }
}
