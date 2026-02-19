using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Items;
using BetaSharp.Stats;
using java.util;

namespace BetaSharp.Client.Guis;

public abstract class GuiSlotStats : GuiSlot
{

    protected int field_27268_b;
    protected List field_27273_c;
    protected Comparator field_27272_d;
    public int field_27271_e;
    public int field_27270_f;
    private readonly GuiStats _gui;

    protected GuiSlotStats(GuiStats statsGui)
        : base(statsGui.mc, statsGui.Width, statsGui.Height, 32, statsGui.Height - 64, 20)
    {
        _gui = statsGui;
        field_27268_b = -1;
        field_27271_e = -1;
        field_27270_f = 0;
        func_27258_a(false);
        func_27259_a(true, 20);
    }

    protected override void ElementClicked(int var1, bool var2)
    {
    }

    protected override bool isSelected(int slotIndex)
    {
        return false;
    }

    protected override void drawBackground()
    {
        _gui.DrawDefaultBackground();
    }

    protected override void func_27260_a(int var1, int var2, Tessellator var3)
    {
        if (!Mouse.isButtonDown(0))
        {
            field_27268_b = -1;
        }

        if (field_27268_b == 0)
        {
            _gui.drawTranslucentRect(var1 + 115 - 18, var2 + 1, 0, 0);
        }
        else
        {
            _gui.drawTranslucentRect(var1 + 115 - 18, var2 + 1, 0, 18);
        }

        if (field_27268_b == 1)
        {
            _gui.drawTranslucentRect(var1 + 165 - 18, var2 + 1, 0, 0);
        }
        else
        {
            _gui.drawTranslucentRect(var1 + 165 - 18, var2 + 1, 0, 18);
        }

        if (field_27268_b == 2)
        {
            _gui.drawTranslucentRect(var1 + 215 - 18, var2 + 1, 0, 0);
        }
        else
        {
            _gui.drawTranslucentRect(var1 + 215 - 18, var2 + 1, 0, 18);
        }

        if (field_27271_e != -1)
        {
            short var4 = 79;
            byte var5 = 18;
            if (field_27271_e == 1)
            {
                var4 = 129;
            }
            else if (field_27271_e == 2)
            {
                var4 = 179;
            }

            if (field_27270_f == 1)
            {
                var5 = 36;
            }

            _gui.drawTranslucentRect(var1 + var4, var2 + 1, var5, 0);
        }

    }

    protected override void func_27255_a(int var1, int var2)
    {
        field_27268_b = -1;
        if (var1 >= 79 && var1 < 115)
        {
            field_27268_b = 0;
        }
        else if (var1 >= 129 && var1 < 165)
        {
            field_27268_b = 1;
        }
        else if (var1 >= 179 && var1 < 215)
        {
            field_27268_b = 2;
        }

        if (field_27268_b >= 0)
        {
            func_27266_c(field_27268_b);
            _gui.mc.sndManager.PlaySoundFX("random.click", 1.0F, 1.0F);
        }

    }

    public override int GetSize()
    {
        return field_27273_c.size();
    }

    protected StatCrafting func_27264_b(int var1)
    {
        return (StatCrafting)field_27273_c.get(var1);
    }

    protected abstract string getKeyForColumn(int column);

    protected void func_27265_a(StatCrafting var1, int x, int y, bool useBrightColor)
    {
        string text;
        if (var1 != null)
        {
            text = var1.format(_gui.statFileWriter.writeStat(var1));
        }
        else
        {
            text = "-";
        }

        _gui.FontRenderer.drawStringWithShadow(text,
            x - _gui.FontRenderer.getStringWidth(text), y + 5,
            useBrightColor ? 0xFFFFFFu : 0x909090u);
    }

    protected override void func_27257_b(int var1, int var2)
    {
        if (var2 >= top && var2 <= bottom)
        {
            int var3 = func_27256_c(var1, var2);
            int var4 = _gui.Width / 2 - 92 - 16;
            if (var3 >= 0)
            {
                if (var1 < var4 + 40 || var1 > var4 + 40 + 20)
                {
                    return;
                }

                StatCrafting var9 = func_27264_b(var3);
                func_27267_a(var9, var1, var2);
            }
            else
            {
                string var5;
                if (var1 >= var4 + 115 - 18 && var1 <= var4 + 115)
                {
                    var5 = getKeyForColumn(0);
                }
                else if (var1 >= var4 + 165 - 18 && var1 <= var4 + 165)
                {
                    var5 = getKeyForColumn(1);
                }
                else
                {
                    if (var1 < var4 + 215 - 18 || var1 > var4 + 215)
                    {
                        return;
                    }

                    var5 = getKeyForColumn(2);
                }

                var5 = ("" + TranslationStorage.getInstance().translateKey(var5)).Trim();
                if (var5.Length > 0)
                {
                    int var6 = var1 + 12;
                    int var7 = var2 - 12;
                    int var8 = _gui.FontRenderer.getStringWidth(var5);
                    _gui.drawTranslucentRect(var6 - 3, var7 - 3, var6 + var8 + 3, var7 + 8 + 3);
                    _gui.FontRenderer.drawStringWithShadow(var5, var6, var7, 0xFFFFFFFF);
                }
            }

        }
    }

    protected void func_27267_a(StatCrafting var1, int var2, int var3)
    {
        if (var1 != null)
        {
            Item var4 = Item.ITEMS[var1.getItemId()];
            string var5 = ("" + TranslationStorage.getInstance().translateNamedKey(var4.getItemName())).Trim();
            if (var5.Length > 0)
            {
                int var6 = var2 + 12;
                int var7 = var3 - 12;
                int var8 = _gui.FontRenderer.getStringWidth(var5);
                _gui.drawTranslucentRect(var6 - 3, var7 - 3, var6 + var8 + 3, var7 + 8 + 3);
                _gui.FontRenderer.drawStringWithShadow(var5, var6, var7, 0xFFFFFFFF);
            }

        }
    }

    protected void func_27266_c(int var1)
    {
        if (var1 != field_27271_e)
        {
            field_27271_e = var1;
            field_27270_f = -1;
        }
        else if (field_27270_f == -1)
        {
            field_27270_f = 1;
        }
        else
        {
            field_27271_e = -1;
            field_27270_f = 0;
        }

        Collections.sort(field_27273_c, field_27272_d);
    }
}
