using BetaSharp;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiLanguageSlot : GuiSlot
{
    private readonly GuiLanguage _parent;

    public GuiLanguageSlot(GuiLanguage parent)
        : base(parent.mc, parent.width, parent.height, 32, parent.height - 64, 24)
    {
        _parent = parent;
    }

    public override int getSize() => TranslationStorage.GetAvailableLocales().Count;

    protected override void elementClicked(int slotIndex, bool doubleClick)
    {
        var locales = TranslationStorage.GetAvailableLocales();
        if (slotIndex >= 0 && slotIndex < locales.Count)
            _parent.SelectLanguageAndBack(slotIndex);
    }

    protected override bool isSelected(int slotIndex)
        => slotIndex >= 0 && slotIndex < TranslationStorage.GetAvailableLocales().Count
           && TranslationStorage.Instance.CurrentLocale == TranslationStorage.GetAvailableLocales()[slotIndex].Code;

    protected override int getContentHeight() => getSize() * 24;

    protected override void drawBackground() => _parent.drawDefaultBackground();

    protected override void drawSlot(int slotIndex, int x, int y, int slotHeight, Tessellator tessellator)
    {
        var locales = TranslationStorage.GetAvailableLocales();
        if (slotIndex < 0 || slotIndex >= locales.Count) return;
        string displayName = locales[slotIndex].DisplayName;
        _parent.drawString(_parent.fontRenderer, displayName, x + 2, y + 4, 0x00FFFFFF);
    }
}
