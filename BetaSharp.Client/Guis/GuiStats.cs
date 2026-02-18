using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Items;
using BetaSharp.Stats;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiStats : GuiScreen
{
    private static readonly ItemRenderer itemRenderer = new();
    protected GuiScreen parentScreen;
    protected string screenTitle = "Select world";
    private GuiSlotStatsGeneral slotGeneral;
    private GuiSlotStatsItem slotItem;
    private GuiSlotStatsBlock slotBlock;
    private readonly StatFileWriter statFileWriter;
    private GuiSlot currentSlot = null;

    public GuiStats(GuiScreen parent, StatFileWriter stats)
    {
        parentScreen = parent;
        statFileWriter = stats;
    }

    public override void InitGui()
    {
        screenTitle = StatCollector.translateToLocal("gui.stats");
        slotGeneral = new GuiSlotStatsGeneral(this);
        slotGeneral.RegisterScrollButtons(_controlList, 1, 1);
        slotItem = new GuiSlotStatsItem(this);
        slotItem.RegisterScrollButtons(_controlList, 1, 1);
        slotBlock = new GuiSlotStatsBlock(this);
        slotBlock.RegisterScrollButtons(_controlList, 1, 1);
        currentSlot = slotGeneral;
        initButtons();
    }
    public void initButtons()
    {
        const int BUTTON_DONE = 0;
        const int BUTTON_GENERAL = 1;
        const int BUTTON_BLOCKS = 2;
        const int BUTTON_ITEMS = 3;

        TranslationStorage translations = TranslationStorage.getInstance();
        _controlList.Add(new GuiButton(BUTTON_DONE, Width / 2 + 4, Height - 28, 150, 20, translations.translateKey("gui.done")));
        _controlList.Add(new GuiButton(BUTTON_GENERAL, Width / 2 - 154, Height - 52, 100, 20, translations.translateKey("stat.generalButton")));
        GuiButton blocksButton = new(BUTTON_BLOCKS, Width / 2 - 46, Height - 52, 100, 20, translations.translateKey("stat.blocksButton"));
        _controlList.Add(blocksButton);
        GuiButton itemsButton = new(BUTTON_ITEMS, Width / 2 + 62, Height - 52, 100, 20, translations.translateKey("stat.itemsButton"));
        _controlList.Add(itemsButton);
        if (slotBlock.GetSize() == 0)
        {
            blocksButton.Enabled = false;
        }

        if (slotItem.GetSize() == 0)
        {
            itemsButton.Enabled = false;
        }
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case 0: // DONE
                    mc.displayGuiScreen(parentScreen);
                    break;
                case 1: // GENERAL
                    currentSlot = slotGeneral;
                    break;
                case 3: // ITEMS
                    currentSlot = slotItem;
                    break;
                case 2: // BLOCKS
                    currentSlot = slotBlock;
                    break;
                default:
                    currentSlot.actionPerformed(button);
                    break;
            }

        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        currentSlot.drawScreen(mouseX, mouseY, partialTicks);
        DrawCenteredString(FontRenderer, screenTitle, Width / 2, 20, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }

    private void drawItemSlot(int x, int y, int itemId)
    {
        drawSlotBackground(x + 1, y + 1);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.PushMatrix();
        GLManager.GL.Rotate(180.0F, 1.0F, 0.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.PopMatrix();
        itemRenderer.drawItemIntoGui(FontRenderer, mc.textureManager, itemId, 0, Item.ITEMS[itemId].getTextureId(0), x + 2, y + 2);
        Lighting.turnOff();
        GLManager.GL.Disable(GLEnum.RescaleNormal);
    }

    private void drawSlotBackground(int x, int y)
    {
        drawSlotTexture(x, y, 0, 0);
    }

    private void drawSlotTexture(int x, int y, int u, int v)
    {
        int textureId = mc.textureManager.getTextureId("/gui/slot.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawingQuads();
        tessellator.addVertexWithUV(x + 0, y + 18, _zLevel, (double)((u + 0) * 0.0078125F), (double)((v + 18) * 0.0078125F));
        tessellator.addVertexWithUV(x + 18, y + 18, _zLevel, (double)((u + 18) * 0.0078125F), (double)((v + 18) * 0.0078125F));
        tessellator.addVertexWithUV(x + 18, y + 0, _zLevel, (double)((u + 18) * 0.0078125F), (double)((v + 0) * 0.0078125F));
        tessellator.addVertexWithUV(x + 0, y + 0, _zLevel, (double)((u + 0) * 0.0078125F), (double)((v + 0) * 0.0078125F));
        tessellator.draw();
    }

    public static Minecraft func_27141_a(GuiStats var0)
    {
        return var0.mc;
    }

    public static TextRenderer func_27145_b(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static StatFileWriter func_27142_c(GuiStats var0)
    {
        return var0.statFileWriter;
    }

    public static TextRenderer func_27140_d(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27146_e(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static Minecraft func_27143_f(GuiStats var0)
    {
        return var0.mc;
    }

    public static void func_27128_a(GuiStats var0, int right, int bottom, int left, int top)
    {
        var0.DrawGradientRect(right, bottom, left, top, 0xC0000000, 0xC0000000);
    }

    public static Minecraft func_27149_g(GuiStats var0)
    {
        return var0.mc;
    }

    public static TextRenderer func_27133_h(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27137_i(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27132_j(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27134_k(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27139_l(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static void func_27129_a(GuiStats var0, int var1, int var2, int var3, int var4, uint topColor, uint bottomColor)
    {
        var0.DrawGradientRect(var1, var2, var3, var4, topColor, bottomColor);
    }

    public static TextRenderer func_27144_m(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static TextRenderer func_27127_n(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static void func_27135_b(GuiStats var0, int var1, int var2, int var3, int var4, uint topColor, uint bottomColor)
    {
        var0.DrawGradientRect(var1, var2, var3, var4, topColor, bottomColor);
    }

    public static TextRenderer func_27131_o(GuiStats var0)
    {
        return var0.FontRenderer;
    }

    public static void func_27148_a(GuiStats var0, int var1, int var2, int itemId)
    {
        var0.drawItemSlot(var1, var2, itemId);
    }
}
