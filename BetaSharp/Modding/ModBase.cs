using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BetaSharp.Modding;

public abstract class ModBase
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Author { get; init; }
    public required string Version { get; init; }
    public required ILogger Logger { get; init; }
    public abstract bool HasOptionsMenu { get; }

    protected ModBase(string name, string author, string description, string version, ILogger logger)
    {
        Name = name;
        Author = author;
        Description = description;
        Version = version;
        Logger = logger;
    }

    protected ModBase() { }

    /// <summary>
    /// Method called when mods are being initialized.
    /// Use this to register your mod's content, hooks, and perform other initialization logic.
    /// This method is called before the game fully initializes, so be cautious about accessing game content that may not be loaded yet.
    /// <see cref="Mods.ModRegistry"/> is not fully initialized at this point, so you cannot access other mods or their content here.
    /// </summary>
    /// <param name="side">The side the mod is being initialized on. Won't be <see cref="Side.Both"/>.</param>
    public abstract void Initialize(Side side);
    /// <summary>
    /// Method called immediately after all mods have been initialized.
    /// <see cref="Mods.ModRegistry"/> is available at this point.
    /// </summary>
    /// <param name="side">The side the mod is being initialized on. Won't be <see cref="Side.Both"/>.</param>
    public virtual void PostInitialize(Side side) { }
    /// <summary>
    /// Called when the mod is being unloaded. Could be called by a mod menu to disable the mod without restarting the game.
    /// It is not strictly necessary to implement this method, it is heavily encouraged to implement it if your mod registers hooks or content that needs to be cleaned up when the mod is unloaded.
    /// It is not expected for the mod to be reloaded after being unloaded, so you do not need to worry about that.
    /// </summary>
    /// <param name="side">The side the mod is being initialized on. Won't be <see cref="Side.Both"/>.</param>
    public virtual void Unload(Side side) { }
    /// <summary>
    /// Called when the user clicks the "Mod Options" button in the mod menu. Only called if <see cref="HasOptionsMenu"/> is true.
    /// Should open a GUI for configuring the mod's settings.
    /// </summary>
    public virtual void OpenOptionsMenu() { }
}
