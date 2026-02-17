using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Util;

namespace BetaSharp.Client.Guis;

public class GuiEditSign : GuiScreen
{

    protected string _screenTitle = "Edit sign message:";
    private readonly BlockEntitySign _entitySign;
    private int _updateCounter;
    private int _editLine = 0;
    private static readonly string _allowedCharacters = ChatAllowedCharacters.allowedCharacters;

    public GuiEditSign(BlockEntitySign sign)
    {
        _entitySign = sign;
    }

    private const int ButtonDone = 0;

    public override void InitGui()
    {
        controlList.Clear();
        Keyboard.enableRepeatEvents(true);
        controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 4 + 120, "Done"));
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
        if (mc.world.isRemote)
        {
            mc.getSendQueue().addToSendQueue(new UpdateSignPacket(_entitySign.x, _entitySign.y, _entitySign.z, _entitySign.texts));
        }

    }

    public override void UpdateScreen()
    {
        ++_updateCounter;
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case ButtonDone:
                    _entitySign.markDirty();
                    mc.displayGuiScreen(null);
                    break;
            }
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        if (eventKey == 200)
        {
            _editLine = _editLine - 1 & 3;
        }

        if (eventKey == 208 || eventKey == 28)
        {
            _editLine = _editLine + 1 & 3;
        }

        if (eventKey == 14 && _entitySign.texts[_editLine].Length > 0)
        {
            _entitySign.texts[_editLine] = _entitySign.texts[_editLine].Substring(0, _entitySign.texts[_editLine].Length - 1);
        }

        if (_allowedCharacters.IndexOf(eventChar) >= 0 && _entitySign.texts[_editLine].Length < 15)
        {
            _entitySign.texts[_editLine] = _entitySign.texts[_editLine] + eventChar;
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, _screenTitle, Width / 2, 40, 0x00FFFFFF);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(Width / 2, 0.0F, 50.0F);
        float scale = 93.75F;
        GLManager.GL.Scale(-scale, -scale, -scale);
        GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
        Block signBlock = _entitySign.getBlock();
        if (signBlock == Block.Sign)
        {
            float rotation = _entitySign.getPushedBlockData() * 360 / 16.0F;
            GLManager.GL.Rotate(rotation, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
        }
        else
        {
            int rotationIndex = _entitySign.getPushedBlockData();
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

        if (_updateCounter / 6 % 2 == 0)
        {
            _entitySign.currentRow = _editLine;
        }

        BlockEntityRenderer.instance.renderTileEntityAt(_entitySign, -0.5D, -0.75D, -0.5D, 0.0F);
        _entitySign.currentRow = -1;
        GLManager.GL.PopMatrix();
        base.Render(mouseX, mouseY, partialTicks);
    }
}