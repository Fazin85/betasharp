using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Resource;
using BetaSharp.Client.Resource.Language;
using BetaSharp.Util.Maths;
using java.io;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiMainMenu : GuiScreen
{
    private const int BUTTON_OPTIONS = 0;
    private const int BUTTON_SINGLEPLAYER = 1;
    private const int BUTTON_MULTIPLAYER = 2;
    private const int BUTTON_MODS = 3;
    private const int BUTTON_QUIT = 4;

    private static readonly java.util.Random rand = new();
    private string splashText = "missingno";
    private GuiButton multiplayerButton;

    public GuiMainMenu()
    {
        try
        {
            List<string> splashLines = [];
            BufferedReader reader =
                new(new java.io.StringReader(AssetManager.Instance.getAsset("title/splashes.txt")
                    .getTextContent()));
            string line = "";

            while (true)
            {
                line = reader.readLine();
                if (line == null)
                {
                    splashText = splashLines[rand.nextInt(splashLines.Count)];
                    break;
                }

                line = line.Trim();
                if (line.Length > 0)
                {
                    splashLines.Add(line);
                }
            }
        }
        catch (Exception exception)
        {
        }
    }

    public override void updateScreen()
    {
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    public override void initGui()
    {
        Calendar calendar = Calendar.getInstance();

        // Special days
        calendar.setTime(new Date());
        if (calendar.get(2) + 1 == 11 && calendar.get(5) == 9)
        {
            splashText = "Happy birthday, ez!";
        }
        else if (calendar.get(2) + 1 == 6 && calendar.get(5) == 1)
        {
            splashText = "Happy birthday, Notch!";
        }
        else if (calendar.get(2) + 1 == 12 && calendar.get(5) == 24)
        {
            splashText = "Merry X-mas!";
        }
        else if (calendar.get(2) + 1 == 1 && calendar.get(5) == 1)
        {
            splashText = "Happy new year!";
        }

        TranslationStorage translator = TranslationStorage.getInstance();
        int buttonTopY = height / 4 + 48;

        controlList.add(new GuiButton(BUTTON_SINGLEPLAYER, width / 2 - 100, buttonTopY, translator.translateKey("menu.singleplayer")));
        controlList.add(multiplayerButton =
            new GuiButton(BUTTON_MULTIPLAYER, width / 2 - 100, buttonTopY + 24, translator.translateKey("menu.multiplayer")));
        controlList.add(new GuiButton(BUTTON_MODS, width / 2 - 100, buttonTopY + 48, translator.translateKey("menu.mods")));

        if (mc.hideQuitButton)
        {
            controlList.add(new GuiButton(BUTTON_OPTIONS, width / 2 - 100, buttonTopY + 72, translator.translateKey("menu.options")));
        }
        else
        {
            controlList.add(new GuiButton(BUTTON_OPTIONS, width / 2 - 100, buttonTopY + 72 + 12, 98, 20,
                translator.translateKey("menu.options")));

            controlList.add(new GuiButton(BUTTON_QUIT, width / 2 + 2, buttonTopY + 72 + 12, 98, 20,
                translator.translateKey("menu.quit")));
        }

        if (mc.session == null)
        {
            multiplayerButton.enabled = false;
        }
    }

    protected override void actionPerformed(GuiButton button)
    {
        switch (button.id)
        {
            case BUTTON_OPTIONS:
                mc.displayGuiScreen(new GuiOptions(this, mc.options));
                break;
            case BUTTON_SINGLEPLAYER:
                mc.displayGuiScreen(new GuiSelectWorld(this));
                break;
            case BUTTON_MULTIPLAYER:
                mc.displayGuiScreen(new GuiMultiplayer(this));
                break;
            case BUTTON_MODS:
                mc.displayGuiScreen(new GuiTexturePacks(this));
                break;
            case BUTTON_QUIT:
                mc.shutdown();
                break;
        }
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        Tessellator tessellator = Tessellator.instance;
        short logoWidth = 274;
        int logoX = width / 2 - logoWidth / 2;
        byte logoY = 30;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/title/mclogo.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        drawTexturedModalRect(logoX + 0, logoY + 0, 0, 0, 155, 44);
        drawTexturedModalRect(logoX + 155, logoY + 0, 0, 45, 155, 44);
        tessellator.setColorOpaque_I(16777215);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(width / 2 + 90, 70.0F, 0.0F);
        GLManager.GL.Rotate(-20.0F, 0.0F, 0.0F, 1.0F);
        float splashScale = 1.8F - MathHelper.abs(MathHelper.sin(java.lang.System.currentTimeMillis() % 1000L /
            1000.0F * (float)Math.PI * 2.0F) * 0.1F);
        splashScale = splashScale * 100.0F / (fontRenderer.getStringWidth(splashText) + 32);
        GLManager.GL.Scale(splashScale, splashScale, splashScale);
        drawCenteredString(fontRenderer, splashText, 0, -8, 16776960);
        GLManager.GL.PopMatrix();
        drawString(fontRenderer, "Minecraft Beta 1.7.3", 2, 2, 5263440);
        string copyrightText = "Copyright Mojang Studios. Not an official Minecraft product.";
        drawString(fontRenderer, copyrightText, width - fontRenderer.getStringWidth(copyrightText) - 2, height - 20, 16777215);
        string disclaimerText = "Not approved by or associated with Mojang Studios or Microsoft.";
        drawString(fontRenderer, disclaimerText, width - fontRenderer.getStringWidth(disclaimerText) - 2, height - 10, 16777215);
        base.render(mouseX, mouseY, partialTicks);
    }
}