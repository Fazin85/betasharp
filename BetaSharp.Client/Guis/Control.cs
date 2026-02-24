using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class Control
{
    public enum ButtonTexture
    {
        Disabled = 0,
        Normal = 1,
        Hovered = 2
    }

    protected float ZLevel = 0.0F;
    internal virtual bool TopLevel => false;

    private Control? _parent;
    public readonly List<Control> Children = [];
    public List<Control> Descendants => Children.SelectMany(c => c.Descendants).Prepend(this).ToList();

    // Track whether last press occurred inside this control
    private bool _pressedInside;

    public string Text
    {
        get;
        set
        {
            field = value;
            DoTextChanged(new TextEventArgs(value));
        }
    } = string.Empty;

    protected int TabIndex;

    public Size Size
    {
        get;
        set
        {
            field = value;
            UpdateAnchorInfo();
            LayoutChildren();
        }
    }
    public int Width => Size.Width;
    public int Height => Size.Height;

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
    public bool Focused
    {
        get;
        set
        {

        }
    }

    public virtual bool Focusable => false;
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
    }
    private AnchorInfo _anchorInfo;

    protected Control(int x, int y, int width, int height)
    {
        Position = new(x, y);
        Size = new(width, height);
        Visible = true;
    }
    protected Control() { }

    private void UpdateAnchorInfo()
    {
        _anchorInfo = new()
        {
            Left = X, Top = Y, Right = X + Width, Bottom = Y + Height,
        };
    }

    protected void LayoutChildren()
    {
        foreach (Control child in Children.ToArray())
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

    public void DrawTexturedRect(int x, int y, int u, int v, int width, int height)
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

    public event EventHandler<MouseEventArgs>? Clicked;
    public event EventHandler<MouseEventArgs>? MousePressed;
    public event EventHandler<MouseEventArgs>? MouseReleased;
    public event EventHandler<MouseEventArgs>? MouseMoved;
    public event EventHandler<KeyboardEventArgs>? KeyInput;
    public event EventHandler<RenderEventArgs>? Rendered;
    public event EventHandler<FocusEventArgs>? FocusChanged;
    public event EventHandler<TextEventArgs>? TextChanged;

    public virtual bool PointInBounds(int x, int y)
    {
        return Enabled
               && Visible
               && x >= X
               && y >= Y
               && x < X + Width
               && y < Y + Height
               && (_parent == null || _parent.PointInBounds(x, y));
    }

    public void DoMousePressed(MouseEventArgs e)
    {
        if (Focusable && !Focused)
        {
            Focused = true;
        }
        _pressedInside = true;
        OnMousePressed(e);
        MousePressed?.Invoke(this, e);
    }

    protected virtual void OnMousePressed(MouseEventArgs e) { }

    public void DoMouseReleased(MouseEventArgs e)
    {
        OnMouseReleased(e);
        MouseReleased?.Invoke(this, e);

        if (_pressedInside && PointInBounds(e.X, e.Y))
        {
            DoClicked(e);
        }

        _pressedInside = false;
    }

    protected virtual void OnMouseReleased(MouseEventArgs e) { }

    public void DoClicked(MouseEventArgs e)
    {
        OnClicked(e);
        Clicked?.Invoke(this, e);
    }

    protected virtual void OnClicked(MouseEventArgs e) { }

    public void DoMouseMoved(MouseEventArgs e)
    {
        OnMouseMoved(e);
        MouseMoved?.Invoke(this, e);
    }

    protected virtual void OnMouseMoved(MouseEventArgs e) { }

    public void DoKeyInput(KeyboardEventArgs e)
    {
        OnKeyInput(e);
        KeyInput?.Invoke(this, e);
    }

    protected virtual void OnKeyInput(KeyboardEventArgs e) { }

    public void DoRendered(RenderEventArgs e)
    {
        if (!Visible) return;

        foreach (Control child in Children.ToArray())
        {
            child.DoRendered(e);
        }

        OnRendered(e);
        Rendered?.Invoke(this, e);
    }

    protected virtual void OnRendered(RenderEventArgs e) { }

    public void DoFocusChanged(FocusEventArgs e)
    {
        OnFocusChanged(e);
        FocusChanged?.Invoke(this, e);
    }

    protected virtual void OnFocusChanged(FocusEventArgs e) { }

    public void DoTextChanged(TextEventArgs e)
    {
        OnTextChanged(e);
        TextChanged?.Invoke(this, e);
    }

    protected virtual void OnTextChanged(TextEventArgs e) { }

    public virtual void HandleMouseInput()
    {
        var mc = Minecraft.INSTANCE;
        int button = Mouse.getEventButton();
        bool isButtonDown = Mouse.getEventButtonState();
        int mouseX = Mouse.getEventX() * mc.displayWidth / mc.displayWidth;
        int mouseY = mc.displayHeight - Mouse.getEventY() * mc.displayHeight / mc.displayHeight - 1;

        if (isButtonDown && button is >= 0 and < Mouse.MouseButtons && PointInBounds(mouseX, mouseY))
        {
            DoMousePressed(new(mouseX, mouseY, button, isButtonDown));
        }
        else if (!isButtonDown && button is >= 0 and < Mouse.MouseButtons)
        {
            DoMouseReleased(new(mouseX, mouseY, button, isButtonDown));
        }

        if (PointInBounds(mouseX, mouseY))
        {
            DoMouseMoved(new(mouseX, mouseY, button, isButtonDown));
        }

        foreach (Control child in Children.ToArray())
        {
            child.HandleMouseInput();
        }
    }

    public virtual void HandleKeyboardInput()
    {
        int key = Keyboard.getEventKey();
        char keyChar = Keyboard.getEventCharacter();
        bool isKeyDown = Keyboard.getEventKeyState();
        bool isRepeat = Keyboard.isRepeatEvent();

        DoKeyInput(new(key, keyChar, isKeyDown, isRepeat));

        foreach (Control child in Children.ToArray())
        {
            child.HandleKeyboardInput();
        }
    }

    public void HandleInput()
    {
        while (Mouse.next())
        {
            HandleMouseInput();
        }

        while (Keyboard.Next())
        {
            HandleKeyboardInput();
        }
    }
}

// Event argument classes
public class MouseEventArgs : EventArgs
{
    public int X { get; }
    public int Y { get; }
    public int Button { get; }
    public bool Pressed { get; }
    public bool Handled { get; set; }

    public MouseEventArgs(int x, int y, int button, bool pressed)
    {
        X = x;
        Y = y;
        Button = button;
        Pressed = pressed;
        Handled = false;
    }
}

public class KeyboardEventArgs : EventArgs
{
    public int Key { get; }
    public char KeyChar { get; }
    public bool IsKeyDown { get; }
    public bool IsRepeat { get; }
    public bool Handled { get; set; }

    public KeyboardEventArgs(int key, char keyChar, bool isKeyDown, bool isRepeat)
    {
        Key = key;
        KeyChar = keyChar;
        IsKeyDown = isKeyDown;
        IsRepeat = isRepeat;
        Handled = false;
    }
}

public class RenderEventArgs : EventArgs
{
    public int MouseX { get; }
    public int MouseY { get; }
    public float TickDelta { get; }

    public RenderEventArgs(int mouseX, int mouseY, float tickDelta)
    {
        MouseX = mouseX;
        MouseY = mouseY;
        TickDelta = tickDelta;
    }
}

public class FocusEventArgs : EventArgs
{
    public bool Focused { get; }
    public Control? OldFocusedControl { get; }

    public FocusEventArgs(bool focused, Control? oldFocusedControl)
    {
        Focused = focused;
        OldFocusedControl = oldFocusedControl;
    }
}

public class TextEventArgs : EventArgs
{
    public string Text { get; }

    public TextEventArgs(string text)
    {
        Text = text;
    }
}
