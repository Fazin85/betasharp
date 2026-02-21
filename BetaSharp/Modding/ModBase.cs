using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BetaSharp.Modding;

public abstract class ModBase
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Author { get; }
    public abstract bool HasOptionsMenu { get; }
    public ILogger Logger { get; internal set; } = NullLogger.Instance;
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
