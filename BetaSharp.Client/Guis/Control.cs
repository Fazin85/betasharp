using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class Control
{
    public enum HoverState
    {
        Disabled = 0,
        Normal = 1,
        Hovered = 2
    }

    protected float ZLevel = 0.0F;
    internal virtual bool TopLevel => false;
    private bool[] _mouseDown = new bool[Mouse.MouseButtons];

    private Control? _parent;
    public readonly List<Control> Children = [];

    private int _lastMouseX;
    private int _lastMouseY;

    public string Text;

    protected Size Size
    {
        get;
        set
        {
            field = value;
            UpdateAnchorInfo();
            LayoutChildren();
        }
    }
    protected int Width => Size.Width;
    protected int Height => Size.Height;

    public Point Position
    {
        get;
        set
        {
            field = value;
            UpdateAnchorInfo();
            LayoutChildren();
        }
    }
    public int X => Position.X;
    public int Y => Position.Y;
    public bool Enabled;
    public bool IsFocused = false;
    public bool Visible;

    public Anchors Anchor
    {
        get;
        set
        {
            field = value;
            UpdateAnchorInfo();
            LayoutChildren();
        }
    } = Anchors.Top | Anchors.Left;
    private AnchorInfo _anchorInfo;

    private void UpdateAnchorInfo()
    {
        _anchorInfo = new()
        {
            Left = X, Top = Y, Right = X + Width, Bottom = Y + Height,
        };
    }

    protected void LayoutChildren()
    {
        foreach (Control child in Children)
        {
            child.UpdatePosition();
        }
    }

    protected void UpdatePosition()
    {
        if (_parent == null) return;

        int parentWidth = _parent.Width;
        int parentHeight = _parent.Height;

        int newX = X;
        int newY = Y;
        int newWidth = Width;
        int newHeight = Height;

        if (Anchor.HasFlag(Anchors.Left) && Anchor.HasFlag(Anchors.Right))
        {
            newWidth = parentWidth - _anchorInfo.Left - (parentWidth - _anchorInfo.Right);
        }
        else if (Anchor.HasFlag(Anchors.Right))
        {
            newX = parentWidth - _anchorInfo.Right;
        }

        if (Anchor.HasFlag(Anchors.Top) && Anchor.HasFlag(Anchors.Bottom))
        {
            newHeight = parentHeight - _anchorInfo.Top - (parentHeight - _anchorInfo.Bottom);
        }
        else if (Anchor.HasFlag(Anchors.Bottom))
        {
            newY = parentHeight - _anchorInfo.Bottom;
        }

        Position = new(newX, newY);
        Size = new(newWidth, newHeight);
    }

    protected virtual HoverState GetHoverState(bool isMouseOver)
    {
        if (!Enabled) return HoverState.Disabled;
        if (isMouseOver) return HoverState.Hovered;

        return HoverState.Normal;
    }

    public void DrawTexturedModalRect(int x, int y, int u, int v, int width, int height)
    {
        float f = 0.00390625F;
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();
        tess.addVertexWithUV(x + 0, y + height, ZLevel, (double)((u + 0) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + height, ZLevel, (double)((u + width) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + 0, ZLevel, (double)((u + width) * f), (double)((v + 0) * f));
        tess.addVertexWithUV(x + 0, y + 0, ZLevel, (double)((u + 0) * f), (double)((v + 0) * f));
        tess.draw();
    }

    public virtual void Render(int mouseX, int mouseY, float tickDelta)
    {
        if (!Visible) return;

        foreach (Control child in Children)
        {
            child.Render(mouseX, mouseY, tickDelta);
        }
    }

    public virtual bool MouseInBounds(int mouseX, int mouseY)
    {
        return Enabled
               && Visible
               && mouseX >= X
               && mouseY >= Y
               && mouseX < X + Width
               && mouseY < Y + Height
               && (_parent == null || _parent.MouseInBounds(mouseX, mouseY));
    }

    public virtual void MouseClicked(int mouseX, int mouseY, int button)
    {
    }

    public bool JustClicked(int mouseX, int mouseY, int button)
    {
        bool buttonWasDown = _mouseDown[button];
        bool buttonIsDown = Mouse.isButtonDown(button);
        _mouseDown[button] = buttonIsDown;
        return MouseInBounds(mouseX, mouseY)
               && !Children.Any(c => c.MouseInBounds(mouseX, mouseY))
               && (buttonIsDown && !buttonWasDown);
    }
}
