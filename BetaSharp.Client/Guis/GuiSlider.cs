using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiSlider : GuiButton
{
    public float Value;
    private readonly float _min;
    private readonly float _max;
    private readonly float _step;
    private bool IsBeingDragged;
    private readonly string _displayStringFormat;
    private readonly Action<float> _updateAction;

    public GuiSlider(int id, int x, int y, string displayStringFormat, Action<float> updateAction, float value, float min, float max, float step = 0)
        : base(id, x, y, 150, 20, string.Format(displayStringFormat, value))
    {
        Value = value;
        _min = min;
        _max = max;
        _step = step;
        _displayStringFormat = displayStringFormat;
        _updateAction = updateAction;
    }

    public GuiSlider Size(int width, int height)
    {
        Width = width;
        Height = height;
        return this;
    }

    protected override HoverState GetHoverState(bool var1)
    {
        return HoverState.Disabled;
    }

    protected override void MouseMoved(Minecraft mc, int mouseX, int mouseY)
    {
        if (!Enabled)
            return;

        if (IsBeingDragged)
        {
            float percentage = (mouseX - (X + 4)) / (float)(Width - 8);
            percentage = Math.Clamp(percentage, 0, 1);
            Value = (percentage * (_max - _min) + _min);
            if (_step > 0.0F)
            {
                Value = MathF.Round(Value / _step) * _step;
            }

            DisplayString = string.Format(_displayStringFormat, Value);
            _updateAction(Value);
        }
    }

    protected override void RenderBackground(Minecraft mc, int mouseX, int mouseY)
    {
        if (!Enabled)
            return;

        GLManager.GL.Color4(1, 1, 1, 1);
        DrawTexturedModalRect(X + (int)(Value * (Width - 8)), Y, 0, 66, 4, 20);
        DrawTexturedModalRect(X + (int)(Value * (Width - 8)) + 4, Y, 196, 66, 4, 20);
    }

    public override bool MouseInBounds(int mouseX, int mouseY)
    {
        if (!base.MouseInBounds(mc, mouseX, mouseY))
            return false;

        IsBeingDragged = true;
        MouseMoved(mc, mouseX, mouseY);
        return true;

    }

    public override void MouseReleased(int x, int y)
    {
        IsBeingDragged = false;
    }
}
