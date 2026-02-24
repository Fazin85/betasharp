using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class Slider : Control
{
    public float Value;
    private readonly float _min;
    private readonly float _max;
    private readonly float _step;
    private readonly string _textFormat;
    private readonly Action<float> _updateAction;

    public Slider(int x, int y, string textFormat, Action<float> updateAction, float value, float min, float max, float step = 0)
        : base(x, y, 150, 20)
    {
        Value = value;
        Text = string.Format(textFormat, value);
        _min = min;
        _max = max;
        _step = step;
        _textFormat = textFormat;
        _updateAction = updateAction;
    }

    protected override void OnMouseMoved(MouseEventArgs e)
    {
        if (!Enabled)
            return;

        float percentage = (e.X - (X + 4)) / (float)(Width - 8);
        percentage = Math.Clamp(percentage, 0, 1);
        Value = (percentage * (_max - _min) + _min);
        if (_step > 0.0F)
        {
            Value = MathF.Round(Value / _step) * _step;
        }

        Text = string.Format(_textFormat, Value);
        _updateAction(Value);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        if (!Enabled)
            return;

        GLManager.GL.Color4(1, 1, 1, 1);
        DrawTexturedRect(X + (int)(Value * (Width - 8)), Y, 0, 66, 4, 20);
        DrawTexturedRect(X + (int)(Value * (Width - 8)) + 4, Y, 196, 66, 4, 20);
    }
}
