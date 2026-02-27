using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public partial class Control
{
    public event EventHandler<MouseEventArgs>? Clicked;
    public event EventHandler<MouseEventArgs>? MousePressed;
    public event EventHandler<MouseEventArgs>? MouseReleased;
    public event EventHandler<MouseEventArgs>? MouseMoved;
    public event EventHandler<KeyboardEventArgs>? KeyInput;
    public event EventHandler<RenderEventArgs>? Rendered;
    public event EventHandler<FocusEventArgs>? FocusChanged;
    public event EventHandler<TextEventArgs>? TextChanged;

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

    public void DoMouseDragged(MouseEventArgs e)
    {
        OnMouseDragged(e);
        MouseMoved?.Invoke(this, e);
    }

    protected virtual void OnMouseDragged(MouseEventArgs e) { }

    public void DoKeyInput(KeyboardEventArgs e)
    {
        OnKeyInput(e);
        KeyInput?.Invoke(this, e);
    }

    protected virtual void OnKeyInput(KeyboardEventArgs e) { }

    public void DoRendered(RenderEventArgs e)
    {
        if (!Visible) return;

        OnRendered(e);
        Rendered?.Invoke(this, e);

        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(X, Y, ZLevel);
        foreach (Control child in Children.ToArray())
        {
            child.DoRendered(e);
        }
        GLManager.GL.PopMatrix();
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
}
