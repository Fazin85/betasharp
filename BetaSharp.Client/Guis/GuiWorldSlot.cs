using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiWorldSlot : GuiSlot
{
    readonly GuiSelectWorld _parentWorldGui;


    public GuiWorldSlot(GuiSelectWorld parent) : base(parent.mc, parent.Width, parent.Height, 32, parent.Height - 64, 36)
    {
        _parentWorldGui = parent;
    }

    public override int GetSize()
    {
        return GuiSelectWorld.GetSize(_parentWorldGui).size();
    }

    protected override void ElementClicked(int slotIndex, bool doubleClick)
    {
        GuiSelectWorld.onElementSelected(_parentWorldGui, slotIndex);
        WorldSaveInfo worldInfo = (WorldSaveInfo)GuiSelectWorld.GetSize(_parentWorldGui).get(slotIndex);
        bool canSelect = GuiSelectWorld.getSelectedWorld(_parentWorldGui) >= 0 && GuiSelectWorld.getSelectedWorld(_parentWorldGui) < GetSize() && !worldInfo.getIsUnsupported();
        GuiSelectWorld.getSelectButton(_parentWorldGui).Enabled = canSelect;
        GuiSelectWorld.getRenameButton(_parentWorldGui).Enabled = canSelect;
        GuiSelectWorld.getDeleteButton(_parentWorldGui).Enabled = canSelect;
        if (doubleClick && canSelect)
        {
            _parentWorldGui.selectWorld(slotIndex);
        }

    }

    protected override bool isSelected(int slotIndex)
    {
        return slotIndex == GuiSelectWorld.getSelectedWorld(_parentWorldGui);
    }

    protected override int getContentHeight()
    {
        return GuiSelectWorld.GetSize(_parentWorldGui).size() * 36;
    }

    protected override void drawBackground()
    {
        _parentWorldGui.DrawDefaultBackground();
    }

    protected override void drawSlot(int slotIndex, int x, int y, int slotHeight, Tessellator tessellator)
    {
        WorldSaveInfo worldInfo = (WorldSaveInfo)GuiSelectWorld.GetSize(_parentWorldGui).get(slotIndex);
        string displayName = worldInfo.getDisplayName();
        if (displayName == null || MathHelper.stringNullOrLengthZero(displayName))
        {
            displayName = GuiSelectWorld.getWorldNameHeader(_parentWorldGui) + " " + (slotIndex + 1);
        }

        string fileInfo = worldInfo.getFileName();
        fileInfo = fileInfo + " (" + GuiSelectWorld.getDateFormatter(_parentWorldGui).format(new Date(worldInfo.getLastPlayed()));
        long size = worldInfo.getSize();
        fileInfo = fileInfo + ", " + size / 1024L * 100L / 1024L / 100.0F + " MB)";
        string extraStatus = "";
        if (worldInfo.getIsUnsupported())
        {
            extraStatus = GuiSelectWorld.getUnsupportedFormatMessage(_parentWorldGui) + " " + extraStatus;
        }

        Gui.DrawString(_parentWorldGui.FontRenderer, displayName, x + 2, y + 1, 0xFFFFFF);
        Gui.DrawString(_parentWorldGui.FontRenderer, fileInfo, x + 2, y + 12, 0x808080);
        Gui.DrawString(_parentWorldGui.FontRenderer, extraStatus, x + 2, y + 12 + 10, 0x808080);
    }
}
