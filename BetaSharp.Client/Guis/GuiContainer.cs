using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.Screens;
using BetaSharp.Screens.Slots;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public abstract class GuiContainer : GuiScreen
{

    private static readonly ItemRenderer itemRenderer = new();
    protected int xSize = 176;
    protected int ySize = 166;
    public ScreenHandler inventorySlots;

    public GuiContainer(ScreenHandler var1)
    {
        inventorySlots = var1;
    }

    public override void initGui()
    {
        base.initGui();
        mc.player.currentScreenHandler = inventorySlots;
    }

    public override void render(int var1, int var2, float var3)
    {
        drawDefaultBackground();
        int var4 = (width - xSize) / 2;
        int var5 = (height - ySize) / 2;
        drawGuiContainerBackgroundLayer(var3);
        GLManager.GL.PushMatrix();
        GLManager.GL.Rotate(120.0F, 1.0F, 0.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.PopMatrix();
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(var4, var5, 0.0F);
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        Slot var6 = null;

        int var9;
        int var10;
        for (int var7 = 0; var7 < inventorySlots.slots.size(); ++var7)
        {
            Slot var8 = (Slot)inventorySlots.slots.get(var7);
            drawSlotInventory(var8);
            if (getIsMouseOverSlot(var8, var1, var2))
            {
                var6 = var8;
                GLManager.GL.Disable(GLEnum.Lighting);
                GLManager.GL.Disable(GLEnum.DepthTest);
                var9 = var8.xDisplayPosition;
                var10 = var8.yDisplayPosition;
                drawGradientRect(var9, var10, var9 + 16, var10 + 16, 0x80FFFFFF, 0x80FFFFFF);
                GLManager.GL.Enable(GLEnum.Lighting);
                GLManager.GL.Enable(GLEnum.DepthTest);
            }
        }

        InventoryPlayer var12 = mc.player.inventory;

        GLManager.GL.Disable(GLEnum.RescaleNormal);
        Lighting.turnOff();
        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.Disable(GLEnum.DepthTest);
        drawGuiContainerForegroundLayer();
        if (var12.getCursorStack() == null && var6 != null && var6.hasStack())
        {
            string var13 = ("" + TranslationStorage.getInstance().translateNamedKey(var6.getStack().getItemName())).Trim();
            if (var13.Length > 0)
            {
                var9 = var1 - var4 + 12;
                var10 = var2 - var5 - 12;
                int var11 = fontRenderer.getStringWidth(var13);
                drawGradientRect(var9 - 3, var10 - 3, var9 + var11 + 3, var10 + 8 + 3, 0xC0000000, 0xC0000000);
                fontRenderer.drawStringWithShadow(var13, var9, var10, 0xFFFFFFFF);
            }
        }

        // Render the dragged cursor item last so it appears on top of foreground text
        if (var12.getCursorStack() != null)
        {
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            Lighting.turnOn();
            GLManager.GL.Enable(GLEnum.Lighting);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Translate(0.0F, 0.0F, 32.0F);
            itemRenderer.renderItemIntoGUI(fontRenderer, mc.textureManager, var12.getCursorStack(), var1 - var4 - 8, var2 - var5 - 8);
            itemRenderer.renderItemOverlayIntoGUI(fontRenderer, mc.textureManager, var12.getCursorStack(), var1 - var4 - 8, var2 - var5 - 8);
            Lighting.turnOff();
            GLManager.GL.Disable(GLEnum.Lighting);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.Disable(GLEnum.RescaleNormal);
        }

        GLManager.GL.PopMatrix();
        base.render(var1, var2, var3);
        GLManager.GL.Enable(GLEnum.Lighting);
        GLManager.GL.Enable(GLEnum.DepthTest);
    }

    protected virtual void drawGuiContainerForegroundLayer()
    {
    }

    protected abstract void drawGuiContainerBackgroundLayer(float var1);

    private void drawSlotInventory(Slot var1)
    {
        int var2 = var1.xDisplayPosition;
        int var3 = var1.yDisplayPosition;
        ItemStack var4 = var1.getStack();
        if (var4 == null)
        {
            int var5 = var1.getBackgroundTextureId();
            if (var5 >= 0)
            {
                GLManager.GL.Disable(GLEnum.Lighting);
                mc.textureManager.bindTexture(mc.textureManager.getTextureId("/gui/items.png"));
                drawTexturedModalRect(var2, var3, var5 % 16 * 16, var5 / 16 * 16, 16, 16);
                GLManager.GL.Enable(GLEnum.Lighting);
                return;
            }
        }

        itemRenderer.renderItemIntoGUI(fontRenderer, mc.textureManager, var4, var2, var3);
        itemRenderer.renderItemOverlayIntoGUI(fontRenderer, mc.textureManager, var4, var2, var3);
    }

    private Slot getSlotAtPosition(int var1, int var2)
    {
        for (int var3 = 0; var3 < inventorySlots.slots.size(); ++var3)
        {
            Slot var4 = (Slot)inventorySlots.slots.get(var3);
            if (getIsMouseOverSlot(var4, var1, var2))
            {
                return var4;
            }
        }

        return null;
    }

    private bool getIsMouseOverSlot(Slot var1, int var2, int var3)
    {
        int var4 = (width - xSize) / 2;
        int var5 = (height - ySize) / 2;
        var2 -= var4;
        var3 -= var5;
        return var2 >= var1.xDisplayPosition - 1 && var2 < var1.xDisplayPosition + 16 + 1 && var3 >= var1.yDisplayPosition - 1 && var3 < var1.yDisplayPosition + 16 + 1;
    }

    protected override void mouseClicked(int var1, int var2, int var3)
    {
        base.mouseClicked(var1, var2, var3);
        if (var3 == 0 || var3 == 1)
        {
            Slot var4 = getSlotAtPosition(var1, var2);
            int var5 = (width - xSize) / 2;
            int var6 = (height - ySize) / 2;
            bool var7 = var1 < var5 || var2 < var6 || var1 >= var5 + xSize || var2 >= var6 + ySize;
            int var8 = -1;
            if (var4 != null)
            {
                var8 = var4.id;
            }

            if (var7)
            {
                var8 = -999;
            }

            if (var8 != -1)
            {
                bool var9 = var8 != -999 && (Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) || Keyboard.isKeyDown(Keyboard.KEY_RSHIFT));
                mc.playerController.func_27174_a(inventorySlots.syncId, var8, var3, var9, mc.player);
            }
        }

    }

    protected override void mouseMovedOrUp(int var1, int var2, int var3)
    {
        if (var3 == 0)
        {
        }

    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
        if (eventKey == 1 || eventKey == mc.options.keyBindInventory.keyCode)
        {
            mc.player.closeHandledScreen();
        }

    }

    public override void onGuiClosed()
    {
        if (mc.player != null)
        {
            mc.playerController.func_20086_a(inventorySlots.syncId, mc.player);
        }
    }

    public override bool doesGuiPauseGame()
    {
        return false;
    }

    public override void updateScreen()
    {
        base.updateScreen();
        if (!mc.player.isAlive() || mc.player.dead)
        {
            mc.player.closeHandledScreen();
        }

    }
}
