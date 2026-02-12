using betareborn.Blocks;
using betareborn.Blocks.Entities;
using betareborn.Client.Input;
using betareborn.Client.Rendering.Blocks.Entities;
using betareborn.Client.Rendering.Core;
using betareborn.Network.Packets.Play;
using betareborn.Util;

namespace betareborn.Client.Guis
{
    public class GuiEditSign : GuiScreen
    {

        protected string screenTitle = "Edit sign message:";
        private BlockEntitySign entitySign;
        private int updateCounter;
        private int editLine = 0;
        private static readonly string allowedCharacters = ChatAllowedCharacters.allowedCharacters;

        public GuiEditSign(BlockEntitySign var1)
        {
            entitySign = var1;
        }

        public override void initGui()
        {
            controlList.clear();
            Keyboard.enableRepeatEvents(true);
            controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 120, "Done"));
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

        protected override void actionPerformed(GuiButton var1)
        {
            if (var1.enabled)
            {
                if (var1.id == 0)
                {
                    entitySign.markDirty();
                    mc.displayGuiScreen(null);
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

        public override void render(int var1, int var2, float var3)
        {
            drawDefaultBackground();
            drawCenteredString(fontRenderer, screenTitle, width / 2, 40, 16777215);
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate(width / 2, 0.0F, 50.0F);
            float var4 = 93.75F;
            GLManager.GL.Scale(-var4, -var4, -var4);
            GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
            Block var5 = entitySign.getBlock();
            if (var5 == Block.SIGN)
            {
                float var6 = entitySign.getPushedBlockData() * 360 / 16.0F;
                GLManager.GL.Rotate(var6, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
            }
            else
            {
                int var8 = entitySign.getPushedBlockData();
                float var7 = 0.0F;
                if (var8 == 2)
                {
                    var7 = 180.0F;
                }

                if (var8 == 4)
                {
                    var7 = 90.0F;
                }

                if (var8 == 5)
                {
                    var7 = -90.0F;
                }

                GLManager.GL.Rotate(var7, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
            }

            if (updateCounter / 6 % 2 == 0)
            {
                entitySign.currentRow = editLine;
            }

            BlockEntityRenderer.instance.renderTileEntityAt(entitySign, -0.5D, -0.75D, -0.5D, 0.0F);
            entitySign.currentRow = -1;
            GLManager.GL.PopMatrix();
            base.render(var1, var2, var3);
        }
    }

}