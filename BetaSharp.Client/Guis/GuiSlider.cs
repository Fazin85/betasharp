using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiSlider : GuiButton
{

    public float sliderValue = 1.0F;
    public bool dragging = false;
    private EnumOptions idFloat = null;

    public GuiSlider(int var1, int var2, int var3, EnumOptions var4, string var5, float var6) : base(var1, var2, var3, 150, 20, var5)
    {
        idFloat = var4;
        sliderValue = var6;
    }

    protected override int getHoverState(bool var1)
    {
        return 0;
    }

    protected override void mouseDragged(Minecraft var1, int var2, int var3)
    {
        if (enabled)
        {
            if (dragging)
            {
                sliderValue = (var2 - (xPosition + 4)) / (float)(width - 8);
                if (sliderValue < 0.0F)
                {
                    sliderValue = 0.0F;
                }

                if (sliderValue > 1.0F)
                {
                    sliderValue = 1.0F;
                }

                var1.options.setOptionFloatValue(idFloat, sliderValue);
                displayString = var1.options.getKeyBinding(idFloat);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            drawTexturedModalRect(xPosition + (int)(sliderValue * (width - 8)), yPosition, 0, 66, 4, 20);
            drawTexturedModalRect(xPosition + (int)(sliderValue * (width - 8)) + 4, yPosition, 196, 66, 4, 20);
        }
    }

    public override bool mousePressed(Minecraft var1, int var2, int var3)
    {
        if (base.mousePressed(var1, var2, var3))
        {
            sliderValue = (var2 - (xPosition + 4)) / (float)(width - 8);
            if (sliderValue < 0.0F)
            {
                sliderValue = 0.0F;
            }

            if (sliderValue > 1.0F)
            {
                sliderValue = 1.0F;
            }

            var1.options.setOptionFloatValue(idFloat, sliderValue);
            displayString = var1.options.getKeyBinding(idFloat);
            dragging = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void mouseReleased(int var1, int var2)
    {
        dragging = false;
    }
}