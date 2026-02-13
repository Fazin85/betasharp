using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiWorldSlot : GuiSlot
{
    readonly GuiSelectWorld parentWorldGui;


    public GuiWorldSlot(GuiSelectWorld parent) : base(parent.mc, parent.width, parent.height, 32, parent.height - 64, 36)
    {
        parentWorldGui = parent;
    }

    public override int getSize()
    {
        return GuiSelectWorld.getSize(parentWorldGui).size();
    }

    protected override void elementClicked(int slotIndex, bool doubleClick)
    {
        GuiSelectWorld.onElementSelected(parentWorldGui, slotIndex);
        WorldSaveInfo worldInfo = (WorldSaveInfo)GuiSelectWorld.getSize(parentWorldGui).get(slotIndex);
        bool canSelect = GuiSelectWorld.getSelectedWorld(parentWorldGui) >= 0 && GuiSelectWorld.getSelectedWorld(parentWorldGui) < getSize() && !worldInfo.getIsUnsupported();
        GuiSelectWorld.getSelectButton(parentWorldGui).enabled = canSelect;
        GuiSelectWorld.getRenameButton(parentWorldGui).enabled = canSelect;
        GuiSelectWorld.getDeleteButton(parentWorldGui).enabled = canSelect;
        if (doubleClick && canSelect)
        {
            parentWorldGui.selectWorld(slotIndex);
        }

    }

    protected override bool isSelected(int slotIndex)
    {
        return slotIndex == GuiSelectWorld.getSelectedWorld(parentWorldGui);
    }

    protected override int getContentHeight()
    {
        return GuiSelectWorld.getSize(parentWorldGui).size() * 36;
    }

    protected override void drawBackground()
    {
        parentWorldGui.drawDefaultBackground();
    }

    protected override void drawSlot(int slotIndex, int x, int y, int slotHeight, Tessellator tessellator)
    {
        WorldSaveInfo worldInfo = (WorldSaveInfo)GuiSelectWorld.getSize(parentWorldGui).get(slotIndex);
        string displayName = worldInfo.getDisplayName();
        if (displayName == null || MathHelper.stringNullOrLengthZero(displayName))
        {
            displayName = GuiSelectWorld.getWorldNameHeader(parentWorldGui) + " " + (slotIndex + 1);
        }

        string fileInfo = worldInfo.getFileName();
        fileInfo = fileInfo + " (" + GuiSelectWorld.getDateFormatter(parentWorldGui).format(new Date(worldInfo.getLastPlayed()));
        long size = worldInfo.getSize();
        fileInfo = fileInfo + ", " + size / 1024L * 100L / 1024L / 100.0F + " MB)";
        string extraStatus = "";
        if (worldInfo.getIsUnsupported())
        {
            extraStatus = GuiSelectWorld.getUnsupportedFormatMessage(parentWorldGui) + " " + extraStatus;
        }

        parentWorldGui.drawString(parentWorldGui.fontRenderer, displayName, x + 2, y + 1, 16777215);
        parentWorldGui.drawString(parentWorldGui.fontRenderer, fileInfo, x + 2, y + 12, 8421504);
        parentWorldGui.drawString(parentWorldGui.fontRenderer, extraStatus, x + 2, y + 12 + 10, 8421504);
    }
}