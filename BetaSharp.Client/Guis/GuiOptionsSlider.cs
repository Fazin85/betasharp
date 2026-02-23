using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiOptionsSlider : GuiSlider
{
    public float sliderValue = 1.0F;
    public bool dragging;
    private readonly FloatOption _option;

    public GuiOptionsSlider(int id, int x, int y, FloatOption option, string displayString, float value)
        : base(id, x, y, $"{displayString}: {{0}}", value, 0.0F, 1.0F)
    {
        _option = option;
        sliderValue = value;
    }

    public GuiOptionsSlider Size(int width, int height)
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
        if (Enabled)
        {
            if (dragging)
            {
                sliderValue = (mouseX - (X + 4)) / (float)(Width - 8);
                if (sliderValue < 0.0F)
                {
                    sliderValue = 0.0F;
                }

                if (sliderValue > 1.0F)
                {
                    sliderValue = 1.0F;
                }

                _option.Set(sliderValue);
                DisplayString = _option.GetDisplayString(TranslationStorage.Instance);
            }
        }
    }

    protected override void RenderBackground(Minecraft mc, int mouseX, int mouseY)
    {
        if (!Enabled)
            return;

        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        DrawTexturedModalRect(X + (int)(sliderValue * (Width - 8)), Y, 0, 66, 4, 20);
        DrawTexturedModalRect(X + (int)(sliderValue * (Width - 8)) + 4, Y, 196, 66, 4, 20);
    }

    public override bool MouseInBounds(Minecraft mc, int mouseX, int mouseY)
    {
        if (base.MouseInBounds(mc, mouseX, mouseY))
        {
            sliderValue = (mouseX - (X + 4)) / (float)(Width - 8);
            if (sliderValue < 0.0F)
            {
                sliderValue = 0.0F;
            }

            if (sliderValue > 1.0F)
            {
                sliderValue = 1.0F;
            }

            _option.Set(sliderValue);
            DisplayString = _option.GetDisplayString(TranslationStorage.Instance);
            dragging = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void MouseReleased(int x, int y)
    {
        dragging = false;
    }
}
