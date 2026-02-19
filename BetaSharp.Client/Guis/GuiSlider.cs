using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiSlider : GuiButton
{

    public float sliderValue = 1.0F;
    public bool dragging;
    private readonly EnumOptions _idFloat;

    public GuiSlider(int id, int x, int y, EnumOptions option, string displayString, float value) : base(id, x, y, 150, 20, displayString)
    {
        _idFloat = option;
        sliderValue = value;
    }

    protected override HoverState GetHoverState(bool var1)
    {
        return HoverState.Disabled;
    }

    protected override void MouseDragged(Minecraft mc, int mouseX, int mouseY)
    {
        if (Enabled)
        {
            if (dragging)
            {
                sliderValue = (mouseX - (XPosition + 4)) / (float)(_width - 8);
                if (sliderValue < 0.0F)
                {
                    sliderValue = 0.0F;
                }

                if (sliderValue > 1.0F)
                {
                    sliderValue = 1.0F;
                }

                mc.options.setOptionFloatValue(_idFloat, sliderValue);
                DisplayString = mc.options.getKeyBinding(_idFloat);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            DrawTexturedModalRect(XPosition + (int)(sliderValue * (_width - 8)), YPosition, 0, 66, 4, 20);
            DrawTexturedModalRect(XPosition + (int)(sliderValue * (_width - 8)) + 4, YPosition, 196, 66, 4, 20);
        }
    }

    public override bool MousePressed(Minecraft mc, int mouseX, int mouseY)
    {
        if (base.MousePressed(mc, mouseX, mouseY))
        {
            sliderValue = (mouseX - (XPosition + 4)) / (float)(_width - 8);
            if (sliderValue < 0.0F)
            {
                sliderValue = 0.0F;
            }

            if (sliderValue > 1.0F)
            {
                sliderValue = 1.0F;
            }

            mc.options.setOptionFloatValue(_idFloat, sliderValue);
            DisplayString = mc.options.getKeyBinding(_idFloat);
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
