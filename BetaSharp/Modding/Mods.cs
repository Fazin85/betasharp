using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;

namespace BetaSharp.Modding;

public class Mods
{
    public static ImmutableList<ModBase> ModRegistry { get; private set; } = ImmutableList<ModBase>.Empty;
    public static string ModsFolder { get; private set; } = null!;

    private static Dictionary<ModBase, ModLoadContext> _modLoadContexts { get; set; } = [];
    private static bool _loaded = false;

    /// <summary>
    /// Loads mods from the specified folder.
    /// If any mods throw unhandled exceptions during initialization, this will not handle them.
    /// This is by design. Mods that throw exceptions during initialization should be fixed by their authors, not silently ignored.
    /// If a mod is broken to the point where it cannot be loaded at all, it will be skipped and an error will be logged.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this method has already been called.</exception>
    public static void LoadMods(string modsFolder, Side side)
    {
        if (_loaded)
        {
            throw new InvalidOperationException("Mods have already been loaded.");
        }

        _loaded = true;
        ModsFolder = System.IO.Path.GetFullPath(modsFolder);

        if (!Directory.Exists(ModsFolder))
        {
            Directory.CreateDirectory(ModsFolder);
            return; // It's empty.
        }

        List<string> modsFiles = Directory.GetFiles(ModsFolder)
            .Where(f => System.IO.Path.GetExtension(f).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Log.Info($"Found {modsFiles.Count} mods in mods folder.");
        if (modsFiles.Count == 0)
        {
            return;
        }

        List<ModBase> loadedMods = [];

        Log.Info($"Loading {modsFiles.Count} mods...");
        foreach (string file in modsFiles)
        {
            Log.Info("Loading mod " + file + "...");

            ModLoadContext loadContext = new(file);
            Assembly assembly;
            try
            {
                assembly = loadContext.LoadFromAssemblyPath(file);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load mod {file}: {ex}");
                continue;
            }

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
                Log.Error($"Aborting mod load: Failed to get types from mod {file}: {ex}");
                continue;
            }

            if (modType == null)
            {
                Log.Warn($"Mod {file} has no class that implements {nameof(ModBase)}, and as such will not be initialized.");
                continue;
            }
            if (Attribute.GetCustomAttribute(modType, typeof(ModSideAttribute)) is ModSideAttribute modSideAttr)
            {
                if (modSideAttr.Side != side && modSideAttr.Side != Side.Both)
                {
                    Log.Info($"Skipping mod {file} because it is marked for side {modSideAttr.Side} and not {side}.");
                    continue;
                }
            }

            Log.Info($"Instantiating mod {file}...");
            ModBase modBaseInstance;
            try
            {
                modBaseInstance = (ModBase)Activator.CreateInstance(modType)!;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create mod instance for mod {file}: {ex}");
                continue;
            }
            modBaseInstance.Logger = Log.GetLogger($"Mod:{modBaseInstance.Name}");

            Log.Info($"Successfully instantiated mod {modBaseInstance.Name} ({file}). Initializing...");
            loadedMods.Add(modBaseInstance);
            _modLoadContexts[modBaseInstance] = loadContext;
            modBaseInstance.Initialize(side);
            Log.Info($"Successfully initialized mod {modBaseInstance.Name}");
        }

        ModRegistry = loadedMods.ToImmutableList();

        Log.Info("Running PostInitialize on mods...");
        foreach (ModBase mod in ModRegistry)
        {
            Log.Info($"Running PostInitialize of mod {mod.Name}...");
            mod.PostInitialize(side);
            Log.Info($"PostInitialize of mod {mod.Name} complete.");
        }
        Log.Info("Mods initialized.");
    }

    /// <summary>
    /// Unloads the specified mod and its assembly.
    /// </summary>
    public static void UnloadMod(ModBase mod, Side side)
    {
        if (!_modLoadContexts.TryGetValue(mod, out var context))
        {
            Log.Warn($"Attempted to unload mod {mod.Name} which is not loaded.");
            return;
        }

        Log.Info($"Unloading mod {mod.Name}...");
        try
        {
            mod.Unload(side);
            _modLoadContexts.Remove(mod);
            ModRegistry = ModRegistry.Remove(mod);
            context.Unload();
            Log.Info($"Successfully unloaded mod {mod.Name}.");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to unload mod {mod.Name}: {ex}");
        }
    }
}
