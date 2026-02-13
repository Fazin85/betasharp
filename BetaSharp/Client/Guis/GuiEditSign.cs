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

    protected string screenTitle = "Edit sign message:";
    private BlockEntitySign entitySign;
    private int updateCounter;
    private int editLine = 0;
    private static readonly string allowedCharacters = ChatAllowedCharacters.allowedCharacters;

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
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
        if (mc.world.isRemote)
        {
            mc.getSendQueue().addToSendQueue(new UpdateSignPacket(entitySign.x, entitySign.y, entitySign.z, entitySign.texts));
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
        if (eventKey == 200)
        {
            editLine = editLine - 1 & 3;
        }

        if (eventKey == 208 || eventKey == 28)
        {
            editLine = editLine + 1 & 3;
        }

        if (eventKey == 14 && entitySign.texts[editLine].Length > 0)
        {
            entitySign.texts[editLine] = entitySign.texts[editLine].Substring(0, entitySign.texts[editLine].Length - 1);
        }

        if (allowedCharacters.IndexOf(eventChar) >= 0 && entitySign.texts[editLine].Length < 15)
        {
            entitySign.texts[editLine] = entitySign.texts[editLine] + eventChar;
        }

    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, screenTitle, width / 2, 40, 16777215);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(width / 2, 0.0F, 50.0F);
        float scale = 93.75F;
        GLManager.GL.Scale(-scale, -scale, -scale);
        GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
        Block signBlock = entitySign.getBlock();
        if (signBlock == Block.SIGN)
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
            entitySign.currentRow = editLine;
        }

        BlockEntityRenderer.instance.renderTileEntityAt(entitySign, -0.5D, -0.75D, -0.5D, 0.0F);
        entitySign.currentRow = -1;
        GLManager.GL.PopMatrix();
        base.render(mouseX, mouseY, partialTicks);
    }
}