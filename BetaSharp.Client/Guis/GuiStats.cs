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
    private GuiListStatsGeneral _listGeneral;
    private GuiListStatsItem _listItem;
    private GuiListStatsBlock _listBlock;
    public StatFileWriter statFileWriter { get; }
    private GuiList _currentList;

    public GuiStats(GuiScreen parent, StatFileWriter stats)
    {
        parentScreen = parent;
        statFileWriter = stats;
    }

    public override void InitGui()
    {
        screenTitle = StatCollector.TranslateToLocal("gui.stats");
        _listGeneral = new GuiListStatsGeneral(this);
        _listGeneral.RegisterScrollButtons(Children, 1, 1);
        _listItem = new GuiListStatsItem(this);
        _listItem.RegisterScrollButtons(Children, 1, 1);
        _listBlock = new GuiListStatsBlock(this);
        _listBlock.RegisterScrollButtons(Children, 1, 1);
        _currentList = _listGeneral;
        initButtons();
    }
    public void initButtons()
    {
        const int BUTTON_DONE = 0;
        const int BUTTON_GENERAL = 1;
        const int BUTTON_BLOCKS = 2;
        const int BUTTON_ITEMS = 3;

        TranslationStorage translations = TranslationStorage.Instance;
        Children.Add(new Button(BUTTON_DONE, Width / 2 + 4, Height - 28, 150, 20, translations.TranslateKey("gui.done")));
        Children.Add(new Button(BUTTON_GENERAL, Width / 2 - 154, Height - 52, 100, 20, translations.TranslateKey("stat.generalButton")));
        Button blocksButton = new(BUTTON_BLOCKS, Width / 2 - 46, Height - 52, 100, 20, translations.TranslateKey("stat.blocksButton"));
        Children.Add(blocksButton);
        Button itemsButton = new(BUTTON_ITEMS, Width / 2 + 62, Height - 52, 100, 20, translations.TranslateKey("stat.itemsButton"));
        Children.Add(itemsButton);
        if (_listBlock.GetSize() == 0)
        {
            blocksButton.Enabled = false;
        }

        if (_listItem.GetSize() == 0)
        {
            itemsButton.Enabled = false;
        }
    }

    protected override void ActionPerformed(Button button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case 0: // DONE
                    mc.OpenScreen(parentScreen);
                    break;
                case 1: // GENERAL
                    _currentList = _listGeneral;
                    break;
                case 3: // ITEMS
                    _currentList = _listItem;
                    break;
                case 2: // BLOCKS
                    _currentList = _listBlock;
                    break;
                default:
                    _currentList.ActionPerformed(button);
                    break;
            }

        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        _currentList.DrawScreen(mouseX, mouseY, tickDelta);
        Gui.DrawCenteredString(FontRenderer, screenTitle, Width / 2, 20, 0xFFFFFF);
    }

    public void drawItemSlot(int x, int y, int itemId)
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
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.BindTexture(mc.textureManager.GetTextureId("/gui/slot.png"));
        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawingQuads();
        tessellator.addVertexWithUV(x + 0, y + 18, ZLevel, (double)((u + 0) * 0.0078125F), (double)((v + 18) * 0.0078125F));
        tessellator.addVertexWithUV(x + 18, y + 18, ZLevel, (double)((u + 18) * 0.0078125F), (double)((v + 18) * 0.0078125F));
        tessellator.addVertexWithUV(x + 18, y + 0, ZLevel, (double)((u + 18) * 0.0078125F), (double)((v + 0) * 0.0078125F));
        tessellator.addVertexWithUV(x + 0, y + 0, ZLevel, (double)((u + 0) * 0.0078125F), (double)((v + 0) * 0.0078125F));
        tessellator.draw();
    }

    public void drawTranslucentRect(int right, int bottom, int left, int top)
    {
        Gui.DrawGradientRect(right, bottom, left, top, 0xC0000000, 0xC0000000);
    }
}
