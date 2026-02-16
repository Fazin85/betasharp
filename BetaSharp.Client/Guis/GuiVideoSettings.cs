using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiVideoSettings : GuiScreen
{

    private readonly GuiScreen field_22110_h;
    protected string field_22107_a = "Video Settings";
    private readonly GameOptions guiGameSettings;
    private static readonly EnumOptions[] field_22108_k = new EnumOptions[] { EnumOptions.RENDER_DISTANCE, EnumOptions.FOV, EnumOptions.FRAMERATE_LIMIT, EnumOptions.VIEW_BOBBING, EnumOptions.GUI_SCALE, EnumOptions.ANISOTROPIC, EnumOptions.MIPMAPS, EnumOptions.MSAA, EnumOptions.ENVIRONMENT_ANIMATION, EnumOptions.DEBUG_MODE };

    public GuiVideoSettings(GuiScreen var1, GameOptions var2)
    {
        field_22110_h = var1;
        guiGameSettings = var2;
    }

    public override void InitGui()
    {
        TranslationStorage var1 = TranslationStorage.getInstance();
        field_22107_a = var1.translateKey("options.videoTitle");
        int var2 = 0;
        EnumOptions[] var3 = field_22108_k;
        int var4 = var3.Length;

        for (int var5 = 0; var5 < var4; ++var5)
        {
            EnumOptions var6 = var3[var5];
            if (!var6.getEnumFloat())
            {
                controlList.Add(new GuiSmallButton(var6.returnEnumOrdinal(), Width / 2 - 155 + var2 % 2 * 160, Height / 6 + 24 * (var2 >> 1), var6, guiGameSettings.getKeyBinding(var6)));
            }
            else
            {
                controlList.Add(new GuiSlider(var6.returnEnumOrdinal(), Width / 2 - 155 + var2 % 2 * 160, Height / 6 + 24 * (var2 >> 1), var6, guiGameSettings.getKeyBinding(var6), guiGameSettings.getOptionFloatValue(var6)));
            }

            ++var2;
        }

        controlList.Add(new GuiButton(200, Width / 2 - 100, Height / 6 + 168, var1.translateKey("gui.done")));
    }

    protected override void ActionPerformed(GuiButton var1)
    {
        if (var1.Enabled)
        {
            if (var1.Id < 100 && var1 is GuiSmallButton)
            {
                guiGameSettings.setOptionValue(((GuiSmallButton)var1).returnEnumOptions(), 1);
                var1.DisplayString = guiGameSettings.getKeyBinding(EnumOptions.getEnumOptions(var1.Id));
            }

            if (var1.Id == 200)
            {
                mc.options.saveOptions();
                mc.displayGuiScreen(field_22110_h);
            }

            if (var1.Id == (int)EnumOptions.GUI_SCALE.ordinal())
            {
                ScaledResolution var2 = new(mc.options, mc.displayWidth, mc.displayHeight);
                int var3 = var2.ScaledWidth;
                int var4 = var2.ScaledHeight;
                SetWorldAndResolution(mc, var3, var4);
            }
        }
    }

    public override void Render(int var1, int var2, float var3)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, field_22107_a, Width / 2, 20, 0x00FFFFFF);
        base.Render(var1, var2, var3);
    }
}
