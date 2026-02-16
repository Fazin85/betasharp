using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiSlider : GuiButton
{

    public float sliderValue = 1.0F;
    public bool dragging = false;
    private readonly EnumOptions _idFloat = null;

    public GuiSlider(int var1, int var2, int var3, EnumOptions var4, string var5, float var6) : base(var1, var2, var3, 150, 20, var5)
    {
        _idFloat = var4;
        sliderValue = var6;
    }

    protected override int GetHoverState(bool var1)
    {
        return 0;
    }

    protected override void MouseDragged(Minecraft var1, int var2, int var3)
    {
        if (Enabled)
        {
            if (dragging)
            {
                sliderValue = (var2 - (XPosition + 4)) / (float)(_width - 8);
                if (sliderValue < 0.0F)
                {
                    sliderValue = 0.0F;
                }

                if (sliderValue > 1.0F)
                {
                    sliderValue = 1.0F;
                }

                var1.options.setOptionFloatValue(_idFloat, sliderValue);
                DisplayString = var1.options.getKeyBinding(_idFloat);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            DrawTexturedModalRect(XPosition + (int)(sliderValue * (_width - 8)), YPosition, 0, 66, 4, 20);
            DrawTexturedModalRect(XPosition + (int)(sliderValue * (_width - 8)) + 4, YPosition, 196, 66, 4, 20);
        }
    }

    public override bool MousePressed(Minecraft var1, int var2, int var3)
    {
        if (base.MousePressed(var1, var2, var3))
        {
            sliderValue = (var2 - (XPosition + 4)) / (float)(_width - 8);
            if (sliderValue < 0.0F)
            {
                sliderValue = 0.0F;
            }

            if (sliderValue > 1.0F)
            {
                sliderValue = 1.0F;
            }

            var1.options.setOptionFloatValue(_idFloat, sliderValue);
            DisplayString = var1.options.getKeyBinding(_idFloat);
            dragging = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void MouseReleased(int var1, int var2)
    {
        dragging = false;
    }
}
