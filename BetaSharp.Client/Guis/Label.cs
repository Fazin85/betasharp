using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class Label : Control
{
    public Alignment TextAlign { get; init; } = Alignment.TopLeft;
    public uint Color;
    private bool _autoSize = true;
    public Label(int x, int y, string text, uint color) : this(x, y, 0, 0, text, color)
    {
        _autoSize = true;
        Text = text; // Must be set after autoSize is enabled or else OnTextChanged won't set the size
        Size = new(
            Minecraft.INSTANCE.fontRenderer.GetStringWidth(text) + 1, // +1 for shadow
            8);
    }
    public Label(int x, int y, int width, int height, string text, uint color) : base(x, y, width, height)
    {
        _autoSize = false; // Must be set first or else OnTextChanged will overwrite the size
        Text = text;
        Color = color;
    }

    public override bool ContainsPoint(int x, int y) => false; // Click-through

    protected override void OnTextChanged(TextEventArgs e)
    {
        if (_autoSize)
        {
            var mc = Minecraft.INSTANCE;
            TextRenderer font = mc.fontRenderer;
            Size = new(font.GetStringWidth(Text), 8);
        }
    }

    protected override void OnRender(RenderEventArgs e)
    {
        var mc = Minecraft.INSTANCE;
        TextRenderer font = mc.fontRenderer;

        int textX = 0;
        int textY = 0;
        if (!_autoSize)
        {
            int textWidth = Math.Min(Width, font.GetStringWidth(Text));
            switch (TextAlign)
            {
                case Alignment.TopLeft:
                    textX = 0;
                    textY = 0;
                    break;
                case Alignment.Top:
                    textX = 0 + (Width - textWidth) / 2;
                    textY = 0;
                    break;
                case Alignment.TopRight:
                    textX = 0 + Width - textWidth;
                    textY = 0;
                    break;
                case Alignment.Left:
                    textX = 0;
                    textY = 0 + (Height - 8) / 2;
                    break;
                case Alignment.Center:
                    textX = 0 + (Width - textWidth) / 2;
                    textY = 0 + (Height - 8) / 2;
                    break;
                case Alignment.Right:
                    textX = 0 + Width - textWidth;
                    textY = 0 + (Height - 8) / 2;
                    break;
                case Alignment.BottomLeft:
                    textX = 0;
                    textY = 0 + Height - 8;
                    break;
                case Alignment.Bottom:
                    textX = 0 + (Width - textWidth) / 2;
                    textY = 0 + Height - 8;
                    break;
                case Alignment.BottomRight:
                    textX = 0 + Width - textWidth;
                    textY = 0 + Height - 8;
                    break;
            }
        }

        font.DrawStringWrappedWithShadow(Text, textX, textY, Width, Color);
    }
}
