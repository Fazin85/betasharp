using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class OptionsSlider : Slider
{
    private readonly FloatOption _option;

    public OptionsSlider(int x, int y, FloatOption option, string text, float value, float min, float max, float step = 0)
        : base(x, y, text, option.Set, value, min, max, step)
    {
        _option = option;
    }
}
