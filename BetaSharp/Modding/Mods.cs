using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Modding;

public class Mods
{
    public static ImmutableList<ModBase> ModRegistry { get; private set; } = ImmutableList<ModBase>.Empty;
    public static string ModsFolder { get; private set; } = null!;
    public static string ConfigFolder { get; private set; } = null!;

    private static Dictionary<ModBase, ModLoadContext> _modLoadContexts { get; set; } = [];
    private static bool _loaded = false;
    private static readonly ILogger s_logger = Log.Instance.For<Mods>();

    /// <summary>
    /// Loads mods from the specified folder.
    /// If any mods throw unhandled exceptions during initialization, this will not handle them.
    /// This is by design. Mods that throw exceptions during initialization should be fixed by their authors, not silently ignored.
    /// If a mod is broken to the point where it cannot be loaded at all, it will be skipped and an error will be logged.
    /// </summary>
    public static void LoadMods(string baseDirectory, Side side)
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        ModsFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, "mods"));
        ConfigFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, "config"));

        if (!Directory.Exists(ModsFolder))
        {
            Directory.CreateDirectory(ModsFolder);
            return; // It's empty.
        }
        if (!Directory.Exists(ConfigFolder))
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        List<string> modsFiles = Directory.GetFiles(ModsFolder)
            .Where(f => System.IO.Path.GetExtension(f).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            .ToList();

        s_logger.LogInformation($"Found {modsFiles.Count} mods in mods folder.");
        if (modsFiles.Count == 0)
        {
            return;
        }

        List<ModBase> loadedMods = [];

        s_logger.LogInformation($"Loading {modsFiles.Count} mods...");
        foreach (string file in modsFiles)
        {
            s_logger.LogInformation("Loading mod " + file + "...");

            ModLoadContext loadContext = new(file);
            Assembly assembly;
            try
            {
                assembly = loadContext.LoadFromAssemblyPath(file);
            }
            catch (Exception ex)
            {
                s_logger.LogError("Failed to load mod {File}: {Exception}", file, ex);
                continue;
            }

            ModInfoAttribute? modInfo = assembly.GetCustomAttribute<ModInfoAttribute>();
            if (modInfo is null)
            {
                s_logger.LogWarning(
                    $"Mod {{File}} has no {nameof(ModInfoAttribute)}. It will not have a name, author, description. " +
                    $"This will not prevent it from being loaded, but it is recommended to add a {nameof(ModInfoAttribute)} to your mod's assembly. " +
                    "See the example mod for an example.", file);
            }
            string modName = modInfo?.Name ?? System.IO.Path.GetFileNameWithoutExtension(file);
            string modAuthor = modInfo?.Author ?? "Unknown author";
            string modDescription = modInfo?.Description ?? "No description";
            string modVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString() ?? "Unknown version";


            Type? modType = null;
            try
            {
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t is { IsInterface: false, IsAbstract: false }
                        && typeof(ModBase).IsAssignableFrom(t))
                    {
                        modType = t;
                        break; // Mods should only contain one mod.
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                s_logger.LogError("Aborting mod load: Failed to get types from mod {ModName}: {ReflectionTypeLoadException}", modName, ex);
                continue;
            }

            if (modType == null)
            {
                s_logger.LogWarning($"Mod {{ModName}} has no class that implements {nameof(ModBase)}, and as such will not be initialized.", modName);
                continue;
            }
            if (Attribute.GetCustomAttribute(modType, typeof(ModSideAttribute)) is ModSideAttribute modSideAttr)
            {
                if (modSideAttr.Side != side && modSideAttr.Side != Side.Both && side != Side.Both)
                {
                    s_logger.LogInformation("Skipping mod {ModName} because it is marked for side {ModSide} and not {CurrentSide}.", modName, modSideAttr.Side, side);
                    continue;
                }
            }

            s_logger.LogInformation("Instantiating mod {File}...", file);
            ModBase mod;
            try
            {
                mod = (ModBase)Activator.CreateInstance(modType,
                    modName, modAuthor, modDescription, modVersion, Log.Instance.For($"Mod:{modName}"))!;
            }
            catch (Exception ex)
            {
                s_logger.LogError("Failed to create mod instance for mod {ModName}: {Exception}", modName, ex);
                continue;
            }

            s_logger.LogInformation("Successfully instantiated the mod {ModName}. Initializing...", modName);

            loadedMods.Add(mod);
            _modLoadContexts[mod] = loadContext;

            if (side is Side.Client or Side.Both)
                mod.Initialize(Side.Client);
            if (side is Side.Server or Side.Both)
                mod.Initialize(Side.Server);

            s_logger.LogInformation("Successfully initialized mod {ModName}", modName);
        }

        ModRegistry = loadedMods.ToImmutableList();

        s_logger.LogInformation("Running PostInitialize on mods...");
        foreach (ModBase mod in ModRegistry)
        {
            s_logger.LogInformation("Running PostInitialize of mod {ModName}...", mod.Name);

            if (side is Side.Client or Side.Both)
                mod.PostInitialize(Side.Client);
            if (side is Side.Server or Side.Both)
                mod.PostInitialize(Side.Server);

            s_logger.LogInformation("PostInitialize of mod {ModName} complete.", mod.Name);
        }
        s_logger.LogInformation("Mods initialized.");
    }

    /// <summary>
    /// Unloads the specified mod and its assembly.
    /// </summary>
    public static void UnloadMod(ModBase mod, Side side)
    {
        if (!_modLoadContexts.TryGetValue(mod, out var context))
        {
            s_logger.LogWarning("Attempted to unload mod {ModName} which is not loaded.", mod.Name);
            return;
        }

        s_logger.LogInformation("Unloading mod {ModName}...", mod.Name);
        try
        {
            if (side is Side.Client or Side.Both)
                mod.Unload(Side.Client);
            if (side is Side.Server or Side.Both)
                mod.Unload(Side.Server);

            _modLoadContexts.Remove(mod);
            ModRegistry = ModRegistry.Remove(mod);

            WeakReference contextWeakRef = new(context, trackResurrection: true);
            context.Unload();
            for (int i = 0; contextWeakRef.IsAlive && i < 50; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            if (contextWeakRef.IsAlive)
            {
                s_logger.LogError("Attempted to unload mod {ModName}, but its assembly could not be unloaded. " +
                          "This is likely due to lingering references to the mod's assembly that are still alive. " +
                          "Make sure to clean up any hooks, content, or other references to the mod in the Unload method of your mod. " +
                          "If you are having trouble, consider asking for help in the BetaSharp Discord server.", mod.Name);
                return;
            }

            s_logger.LogInformation("Successfully unloaded mod {ModName}.", mod.Name);
        }
        catch (Exception ex)
        {
            s_logger.LogError("Failed to unload mod {ModName}: {Exception}", mod.Name, ex);
        }
    }
}
