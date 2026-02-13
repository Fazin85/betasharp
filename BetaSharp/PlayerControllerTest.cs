using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp;

public class PlayerControllerTest : PlayerController
{
    public PlayerControllerTest(Minecraft var1) : base(var1)
    {
        field_1064_b = true;
    }

    public override void func_6473_b(EntityPlayer entityPlayer)
    {
        for (int i = 0; i < 9; ++i)
        {
            if (entityPlayer.inventory.main[i] == null)
            {
                mc.player.inventory.main[i] = new ItemStack(Session.RegisteredBlocksList[i]);
            }
            else
            {
                mc.player.inventory.main[i].count = 1;
            }
        }
    }

    public override bool shouldDrawHUD()
    {
        return false;
    }

    public override void func_717_a(World var1)
    {
        base.func_717_a(var1);
    }

    public override void updateController()
    {
    }
}