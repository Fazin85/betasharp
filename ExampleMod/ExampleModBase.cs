using BetaSharp.Client.Rendering;
using BetaSharp.Modding;
using MonoMod.RuntimeDetour;

namespace ExampleMod;

[ModSide(Side.Client)]
public class ExampleModBase : ModBase
{
    public override string Name => "Example Mod";
    public override string Description => "Surely does something really cool";
    public override string Author => "ExampleAuthor123";
    private Hook _textRendererHook = null!;

    public override void Initialize(Side side)
    {
        Console.WriteLine("Initialize called for Example Mod");
        _textRendererHook = new(typeof(TextRenderer).GetMethod("RenderString")!, TextRenderer_renderString);
    }

    public override void PostInitialize(Side side)
    {
        Console.WriteLine($"PostInitialize called for Example Mod. " +
                          $"Loaded mods: [{string.Join(", ", Mods.ModRegistry.Select(m => m.Name))}]");
    }

    public override void Unload(Side side)
    {
        Console.WriteLine("Example Mod is unloading");
        _textRendererHook.Dispose();
    }

    private static void TextRenderer_renderString(
        Action<TextRenderer, string, int, int, uint, bool> orig,
        TextRenderer instance,
        string text,
        int x,
        int y,
        uint color,
        bool darken)
    {
        orig(instance, text, x, y, 0xFFFF0000, darken);
    }
}
