using System.Diagnostics;
using BetaSharp.Client.Input;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Items;
using BetaSharp.Modding;
using BetaSharp.Stats;
using com.sun.org.apache.xpath.@internal.operations;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiMods : GuiScreen
{
    private const int ButtonDone = 0;
    private const int ButtonModSettings = 1;
    private const int ButtonUnloadMod = 2;
    private const int ButtonOpenModsFolder = 3;
    private readonly GuiScreen _parent;
    private GuiButton _optionsButton;
    private ModList _modList;
    public GuiMods(GuiScreen parent)
    {
        _parent = parent;
    }

    public override void InitGui()
    {
        TranslationStorage t9n = TranslationStorage.Instance;
        _controlList.Add(new(ButtonDone, Width - 50 - 10, Height - 30, 50, 20, t9n.TranslateKey("gui.done")));
        _controlList.Add(new(ButtonOpenModsFolder, Width - 150 - 10 - 10, Height - 30, 100, 20, "Open Mods Folder"));
        _controlList.Add(new(ButtonUnloadMod, 10, Height - 30, 50, 20, "Unload"));
        _controlList.Add(_optionsButton = new(ButtonModSettings, 50 + 10 + 10, Height - 30, 100, 20, "Mod Options"));
        _modList = new(this);
        _optionsButton.Enabled = false;
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case ButtonDone:
                mc.displayGuiScreen(_parent);
                break;
            case ButtonOpenModsFolder:
                string path = Mods.ModsFolder;

                if (OperatingSystem.IsWindows()) Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                else if (OperatingSystem.IsMacOS()) Process.Start("open", path);
                else if (OperatingSystem.IsLinux()) Process.Start("xdg-open", path);

                break;
            case ButtonUnloadMod:
                if (_modList.SelectedIndex != -1)
                {
                    ModBase mod = Mods.ModRegistry[_modList.SelectedIndex];
                    Mods.UnloadMod(mod, Side.Both);
                    _modList.SelectedIndex = -1;
                    UpdateOptionsButtonState(false);
                }
                break;
            case ButtonModSettings:
                if (_modList.SelectedIndex != -1)
                {
                    ModBase mod = Mods.ModRegistry[_modList.SelectedIndex];
                    if (mod.HasOptionsMenu)
                    {
                        mod.OpenOptionsMenu();
                    }
                }
                break;
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        // Left shaded background (for mod list)
        DrawRect(10, 36, Width / 2 - 5, Height - 40, 0xC0000000);
        // Right shaded background (for mod details)
        DrawRect(Width / 2 + 5, 36, Width - 10, Height - 40, 0xC0000000);
        _modList.Draw(mouseX, mouseY, 10, 36, Width / 2 - 5, Height - 40);
        base.Render(mouseX, mouseY, partialTicks);
    }

    private void UpdateOptionsButtonState(bool enabled)
    {
        _optionsButton.Enabled = enabled;
    }

    private class ModList
    {
        public int SelectedIndex = -1;
        private readonly GuiMods _parent;
        private int _scrollOffset;
        private int _initialClickY = -1;
        private int _initialClickX = -1;
        private int _initialScrollOffset;
        private bool _mouseHasMovedMuchSinceClick;
        private bool _mouseWasDown;
        private const int ListDragThreshold = 5;

        public ModList(GuiMods parent)
        {
            _parent = parent;
        }

        public void Draw(int mouseX, int mouseY, int left, int top, int right, int bottom)
        {
            GLManager.GL.Enable(EnableCap.ScissorTest);
            var mc = Minecraft.INSTANCE;
            ScaledResolution res = new(mc.options, mc.displayWidth, mc.displayHeight);
            int scale = (int)Math.Round(mc.displayWidth / res.ScaledWidthDouble);

            GLManager.GL.Scissor(
                left * scale,
                (mc.displayHeight - bottom * scale),
                (uint)((right - left) * scale),
                (uint)((bottom - top) * scale));
            var mods = Mods.ModRegistry;
            /*
            // Fake mods for testing
            var mods = new (string Name, string Description)[]
            {
                ("Example Mod", "Surely does something really cool"),
                ("Another Mod", "This mod adds even more cool features"),
                ("Yet Another Mod", "Because one mod isn't enough"),
                ("Super Cool Mod", "The coolest mod of them all"),
                ("Mega Mod", "Adds a ton of content to the game"),
                ("Fun Mod", "Makes the game more fun to play"),
                ("Utility Mod", "Adds useful tools and features"),
                ("Graphics Mod", "Improves the game's visuals"),
                ("Performance Mod", "Optimizes the game for better performance"),
                ("Adventure Mod", "Adds new adventures and quests to explore"),
                ("Mod with a really long name that will have to be truncated with ellipses",
                "This mod has a really long description that will also be truncated when displayed in the mod list"),
            }.ToList();
            */
            int contentHeight = mods.Count * 36;
            int height = Math.Min(contentHeight, bottom - top);

            int hoveredIndex = -1;
            if (mouseY >= top && mouseY <= bottom && mouseX >= left && mouseX <= right - 6)
            {
                for (int i = 0; i < mods.Count; i++)
                {
                    int y = top + i * 36 - _scrollOffset;
                    if (y + 36 < top || y > bottom) continue; // Skip off-screen mods
                    if (mouseX >= left && mouseX <= right - 6 && mouseY >= y && mouseY <= y + 36)
                    {
                        hoveredIndex = i;
                        break;
                    }
                }
            }

            bool clicked = false;
            if (Mouse.isButtonDown(0))
            {
                _mouseWasDown = true;
                if (_initialClickY == -1)
                {
                    _initialClickY = mouseY;
                    _initialClickX = mouseX;
                    _initialScrollOffset = _scrollOffset;
                    _mouseHasMovedMuchSinceClick = false;
                }
                else
                {
                    int deltaY = 0;
                    if (Math.Abs(mouseY - _initialClickY) > ListDragThreshold
                        || Math.Abs(mouseX - _initialClickX) > ListDragThreshold)
                    {
                        _mouseHasMovedMuchSinceClick = true;
                    }
                    if (_initialClickX < right - 6) // dragging the list
                    {
                        if (_mouseHasMovedMuchSinceClick)
                        {
                            deltaY = mouseY - _initialClickY;
                        }
                    }
                    else // dragging the scrollbar
                    {
                        deltaY = mouseY - _initialClickY;
                        int contentScrollRange = contentHeight - height;
                        if (contentScrollRange > 0)
                        {
                            deltaY = (deltaY * contentScrollRange) / (height - Math.Clamp((height * height) / contentHeight, 32, height - 8));
                            deltaY *= -1;
                        }
                    }
                    _scrollOffset = Math.Clamp(_initialScrollOffset - deltaY, 0, Math.Max(0, contentHeight - height));
                }
            }
            else
            {
                _initialClickY = -1;
                _initialClickX = -1;
                if (_mouseWasDown)
                {
                    clicked = !_mouseHasMovedMuchSinceClick && hoveredIndex != -1;
                    _mouseWasDown = false;
                }
            }

            SelectedIndex = clicked ? hoveredIndex : SelectedIndex;

            for (int i = 0; i < mods.Count; i++)
            {
                int y = top + i * 36 - _scrollOffset;
                if (y + 36 < top || y > bottom) continue; // Skip off-screen mods

                var mod = mods[i];

                // Draw mod entry background
                if (i == SelectedIndex)
                {
                    // Border
                    DrawRect(left, y, right - 7, y + 36, 0xFF808080);
                    // Background
                    DrawRect(left + 1, y + 1, right - 7 - 1, y + 36 - 1, 0xFF000000);
                }
                else if (i == hoveredIndex)
                {
                    // Border
                    // top
                    DrawRect(left, y, right - 8, y + 1, 0x80808080);
                    // bottom
                    DrawRect(left, y + 35, right - 8, y + 36, 0x80808080);
                    // left
                    DrawRect(left, y + 1, left + 1, y + 35, 0x80808080);
                    // right
                    DrawRect(right - 8, y, right - 7, y + 36, 0x80808080);
                    // Background
                    DrawRect(left + 1, y + 1, right - 7 - 1, y + 36 - 1, 0x80000000);
                }

                string name = mod.Name;
                int nameWidth = _parent.FontRenderer.GetStringWidth(name);
                while (nameWidth > right - left - 6)
                {
                    name = name[..^1];
                    nameWidth = _parent.FontRenderer.GetStringWidth(name + "...");
                }
                if (name != mod.Name) name += "...";
                string description = mod.Description.Split('\n')[0];
                int descriptionWidth = _parent.FontRenderer.GetStringWidth(description);
                while (descriptionWidth > right - left - 6)
                {
                    description = description[..^1];
                    descriptionWidth = _parent.FontRenderer.GetStringWidth(description + "...");
                }
                if (description != mod.Description.Split('\n')[0]) description += "...";

                // Draw mod name and description
                DrawString(_parent.FontRenderer, name, left + 3, y + 3, 0xFFFFFF);
                DrawString(_parent.FontRenderer, description, left + 3, y + 13, 0x808080);
            }

            if (height < contentHeight)
            {
                // Draw scrollbar
                int barHeight = Math.Clamp((height * height) / contentHeight, 32, height - 8);
                int barY = top + (_scrollOffset * (height - barHeight)) / (contentHeight - height);
                int barX = right - 6;

                // Bar background
                DrawRect(barX, top, barX + 6, bottom, 0xC0000000);
                // Bar body
                DrawRect(barX, barY, barX + 6, barY + barHeight, 0xFF808080);
                // Bar highlight
                DrawRect(barX, barY, barX + 5, barY + barHeight - 1, 0xFFC0C0C0);
            }

            GLManager.GL.Disable(EnableCap.ScissorTest);
        }
    }
}
