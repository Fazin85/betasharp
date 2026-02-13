using BetaSharp.Items;
using java.util;

namespace BetaSharp.Screens;

public interface ScreenHandlerListener
{
    void onContentsUpdate(ScreenHandler handler, List stacks);

    void onSlotUpdate(ScreenHandler handler, int slot, ItemStack stack);

    void onPropertyUpdate(ScreenHandler handler, int syncId, int trackedValue);
}