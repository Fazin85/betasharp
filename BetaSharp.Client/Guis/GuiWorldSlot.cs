using System;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Client.Guis;

public class GuiWorldSlot : GuiSlot
{
    private readonly GuiSelectWorld _parentWorldGui;

    public GuiWorldSlot(GuiSelectWorld parent)
        : base(parent.mc, parent.Width, parent.Height, 32, parent.Height - 64, 36)
    {
        _parentWorldGui = parent;
    }

    public override int GetSize()
    {
        return _parentWorldGui.SaveList.Count;
    }

    protected override void ElementClicked(int slotIndex, bool doubleClick)
    {
        _parentWorldGui.SelectedWorldIndex = slotIndex;

        WorldSaveInfo worldInfo = _parentWorldGui.SaveList[slotIndex];

        bool isValidSelection = _parentWorldGui.SelectedWorldIndex >= 0
                                && _parentWorldGui.SelectedWorldIndex < GetSize();

        bool canSelect = isValidSelection && !worldInfo.getIsUnsupported();

        _parentWorldGui.ButtonSelect.Enabled = canSelect;
        _parentWorldGui.ButtonRename.Enabled = canSelect;
        _parentWorldGui.ButtonDelete.Enabled = canSelect;

        if (doubleClick && canSelect)
        {
            _parentWorldGui.SelectWorld(slotIndex);
        }
    }

    protected override bool isSelected(int slotIndex)
    {
        return slotIndex == _parentWorldGui.SelectedWorldIndex;
    }

    protected override int getContentHeight()
    {
        return GetSize() * 36;
    }

    protected override void drawBackground()
    {
        _parentWorldGui.DrawDefaultBackground();
    }

    protected override void drawSlot(int slotIndex, int x, int y, int slotHeight, Tessellator tessellator)
    {
        WorldSaveInfo worldInfo = _parentWorldGui.SaveList[slotIndex];
        string displayName = worldInfo.getDisplayName();
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = $"{_parentWorldGui.WorldNameHeader} {slotIndex + 1}";
        }
        DateTimeOffset lastPlayed = DateTimeOffset.FromUnixTimeMilliseconds(worldInfo.getLastPlayed());
        string dateStr = lastPlayed.LocalDateTime.ToString(_parentWorldGui.DateFormat);
        float sizeInMb = worldInfo.getSize() / 1024f / 1024f;
        string fileInfo = $"{worldInfo.getFileName()} ({dateStr}, {sizeInMb:F1} MB)";

        string extraStatus = worldInfo.getIsUnsupported()
            ? _parentWorldGui.UnsupportedFormatMessage
            : "";
        Gui.DrawString(_parentWorldGui.FontRenderer, displayName, x + 2, y + 1, 0xFFFFFF);
        Gui.DrawString(_parentWorldGui.FontRenderer, fileInfo, x + 2, y + 12, 0x808080);

        if (!string.IsNullOrEmpty(extraStatus))
        {
            Gui.DrawString(_parentWorldGui.FontRenderer, extraStatus, x + 2, y + 22, 0x808080);
        }
    }
}
