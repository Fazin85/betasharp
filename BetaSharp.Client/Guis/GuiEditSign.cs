using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Util;
using java.awt;
using java.awt.datatransfer;

namespace BetaSharp.Client.Guis;

public class GuiEditSign : GuiScreen
{

    protected string screenTitle = "Edit sign message:";
    private readonly BlockEntitySign entitySign;
    private int updateCounter;
    private int editLine = 0;
    private static readonly string allowedCharacters = ChatAllowedCharacters.allowedCharacters;
    
    // Selection tracking per line
    private int[] cursorPosition = new int[4];
    private int[] selectionStart = new int[4];
    private int[] selectionEnd = new int[4];

    public GuiEditSign(BlockEntitySign sign)
    {
        entitySign = sign;
    }

    private const int BUTTON_DONE = 0;

    public override void initGui()
    {
        controlList.clear();
        Keyboard.enableRepeatEvents(true);
        controlList.add(new GuiButton(BUTTON_DONE, width / 2 - 100, height / 4 + 120, "Done"));
        
        // Initialize cursor and selection arrays
        for (int i = 0; i < 4; i++)
        {
            cursorPosition[i] = 0;
            selectionStart[i] = -1;
            selectionEnd[i] = -1;
        }
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
        if (mc.world.isRemote)
        {
            mc.getSendQueue().addToSendQueue(new UpdateSignPacket(entitySign.x, entitySign.y, entitySign.z, entitySign.Texts));
        }

    }

    public override void updateScreen()
    {
        ++updateCounter;
    }

    protected override void actionPerformed(GuiButton button)
    {
        if (button.enabled)
        {
            switch (button.id)
            {
                case BUTTON_DONE:
                    entitySign.markDirty();
                    mc.displayGuiScreen(null);
                    break;
            }
        }
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
        // Check for Ctrl combos first
        bool ctrlDown = Keyboard.isKeyDown(Keyboard.KEY_LCONTROL) || Keyboard.isKeyDown(Keyboard.KEY_RCONTROL);
        bool shiftDown = Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) || Keyboard.isKeyDown(Keyboard.KEY_RSHIFT);

        if (ctrlDown)
        {
            switch (eventKey)
            {
                case Keyboard.KEY_A:
                    // Select all on current line
                    selectionStart[editLine] = 0;
                    selectionEnd[editLine] = entitySign.Texts[editLine]?.Length ?? 0;
                    cursorPosition[editLine] = selectionEnd[editLine];
                    return;
                case Keyboard.KEY_C:
                    // Copy current line
                    CopyLineToClipboard();
                    return;
                case Keyboard.KEY_X:
                    // Cut current line
                    CutLineToClipboard();
                    return;
                case Keyboard.KEY_V:
                    // Paste into current line
                    PasteClipboardIntoLine();
                    return;
            }
        }

        // Handle Shift+Left/Right for selection
        if (shiftDown)
        {
            switch (eventKey)
            {
                case Keyboard.KEY_LEFT:
                    if (selectionStart[editLine] == -1)
                    {
                        selectionStart[editLine] = cursorPosition[editLine];
                    }
                    if (cursorPosition[editLine] > 0) cursorPosition[editLine]--;
                    selectionEnd[editLine] = cursorPosition[editLine];
                    return;
                case Keyboard.KEY_RIGHT:
                    if (selectionStart[editLine] == -1)
                    {
                        selectionStart[editLine] = cursorPosition[editLine];
                    }
                    if (cursorPosition[editLine] < entitySign.Texts[editLine].Length) cursorPosition[editLine]++;
                    selectionEnd[editLine] = cursorPosition[editLine];
                    return;
            }
        }

        // Arrow keys for navigation
        if (eventKey == 200)  // Up
        {
            editLine = editLine - 1 & 3;
            return;
        }

        if (eventKey == 208)  // Down
        {
            editLine = editLine + 1 & 3;
            return;
        }

        if (eventKey == 203)  // Left
        {
            if (cursorPosition[editLine] > 0) cursorPosition[editLine]--;
            ClearLineSelection();
            return;
        }

        if (eventKey == 205)  // Right
        {
            if (cursorPosition[editLine] < entitySign.Texts[editLine].Length) cursorPosition[editLine]++;
            ClearLineSelection();
            return;
        }

        // Backspace
        if (eventKey == 14)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }
            else if (entitySign.Texts[editLine].Length > 0 && cursorPosition[editLine] > 0)
            {
                cursorPosition[editLine]--;
                entitySign.Texts[editLine] = entitySign.Texts[editLine].Substring(0, cursorPosition[editLine]) + entitySign.Texts[editLine].Substring(cursorPosition[editLine] + 1);
            }
            ClearLineSelection();
            return;
        }

        // Delete key
        if (eventKey == 211)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }
            else if (cursorPosition[editLine] < entitySign.Texts[editLine].Length)
            {
                entitySign.Texts[editLine] = entitySign.Texts[editLine].Substring(0, cursorPosition[editLine]) + entitySign.Texts[editLine].Substring(cursorPosition[editLine] + 1);
            }
            ClearLineSelection();
            return;
        }

        // Enter key switches to next line
        if (eventKey == 28)
        {
            editLine = editLine + 1 & 3;
            return;
        }

        // Regular character input
        if (allowedCharacters.IndexOf(eventChar) >= 0 && entitySign.Texts[editLine].Length < 15)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }

            entitySign.Texts[editLine] = entitySign.Texts[editLine].Substring(0, cursorPosition[editLine]) + eventChar + entitySign.Texts[editLine].Substring(cursorPosition[editLine]);
            cursorPosition[editLine]++;
            ClearLineSelection();
        }
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, screenTitle, width / 2, 40, 0x00FFFFFF);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(width / 2, 0.0F, 50.0F);
        float scale = 93.75F;
        GLManager.GL.Scale(-scale, -scale, -scale);
        GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
        Block signBlock = entitySign.getBlock();
        if (signBlock == Block.Sign)
        {
            float rotation = entitySign.getPushedBlockData() * 360 / 16.0F;
            GLManager.GL.Rotate(rotation, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
        }
        else
        {
            int rotationIndex = entitySign.getPushedBlockData();
            float angle = 0.0F;
            if (rotationIndex == 2)
            {
                angle = 180.0F;
            }

            if (rotationIndex == 4)
            {
                angle = 90.0F;
            }

            if (rotationIndex == 5)
            {
                angle = -90.0F;
            }

            GLManager.GL.Rotate(angle, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
        }

        if (updateCounter / 6 % 2 == 0)
        {
            entitySign.CurrentRow = editLine;
        }

        BlockEntityRenderer.Instance.RenderTileEntityAt(entitySign, -0.5D, -0.75D, -0.5D, 0.0F);
        entitySign.CurrentRow = -1;
        GLManager.GL.PopMatrix();
        base.render(mouseX, mouseY, partialTicks);
    }

    private bool HasLineSelection()
    {
        return selectionStart[editLine] != -1 && selectionEnd[editLine] != -1 && selectionStart[editLine] != selectionEnd[editLine];
    }

    private (int start, int end) GetLineSelectionRange()
    {
        if (!HasLineSelection()) return (0, 0);
        int s = Math.Min(selectionStart[editLine], selectionEnd[editLine]);
        int e = Math.Max(selectionStart[editLine], selectionEnd[editLine]);
        return (s, e);
    }

    private string GetSelectedLineText()
    {
        if (!HasLineSelection()) return "";
        var (s, e) = GetLineSelectionRange();
        return entitySign.Texts[editLine].Substring(s, e - s);
    }

    private void DeleteLineSelection()
    {
        if (!HasLineSelection()) return;
        var (s, e) = GetLineSelectionRange();
        entitySign.Texts[editLine] = entitySign.Texts[editLine].Substring(0, s) + entitySign.Texts[editLine].Substring(e);
        cursorPosition[editLine] = s;
        ClearLineSelection();
    }

    private void ClearLineSelection()
    {
        selectionStart[editLine] = -1;
        selectionEnd[editLine] = -1;
    }

    private void CopyLineToClipboard()
    {
        if (!HasLineSelection()) return;
        try
        {
            string sel = GetSelectedLineText();
            StringSelection ss = new(sel);
            Toolkit.getDefaultToolkit().getSystemClipboard().setContents(ss, null);
        }
        catch (Exception)
        {
        }
    }

    private void CutLineToClipboard()
    {
        if (!HasLineSelection()) return;
        CopyLineToClipboard();
        DeleteLineSelection();
    }

    private void PasteClipboardIntoLine()
    {
        try
        {
            Transferable t = Toolkit.getDefaultToolkit().getSystemClipboard().getContents(null);
            if (t != null && t.isDataFlavorSupported(DataFlavor.stringFlavor))
            {
                string clip = (string)t.getTransferData(DataFlavor.stringFlavor);
                clip ??= "";
                if (HasLineSelection()) DeleteLineSelection();
                int maxInsert = Math.Max(0, 15 - entitySign.Texts[editLine].Length);
                if (clip.Length > maxInsert) clip = clip.Substring(0, maxInsert);
                entitySign.Texts[editLine] = entitySign.Texts[editLine].Substring(0, cursorPosition[editLine]) + clip + entitySign.Texts[editLine].Substring(cursorPosition[editLine]);
                cursorPosition[editLine] += clip.Length;
                ClearLineSelection();
            }
        }
        catch (Exception)
        {
        }
    }
}
