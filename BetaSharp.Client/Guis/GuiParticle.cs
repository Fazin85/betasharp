using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiParticle : Gui
{
    private readonly List<Particle> particles = new ();
    private readonly Minecraft mc;

    public GuiParticle(Minecraft mc)
    {
        this.mc = mc;
    }

    public void updateParticles()
    {
        for (int i = 0; i < particles.Count; ++i)
        {
            Particle p = particles[i];
            p.func_25127_a();
            p.func_25125_a(this);
            if (p.field_25139_h)
            {
                particles.RemoveAt(i--);
            }
        }

    }

    public void render(float partialTicks)
    {
        mc.textureManager.bindTexture(mc.textureManager.getTextureId("/gui/particles.png"));

        for (int i = 0; i < particles.Count; ++i)
        {
            Particle p = particles[i];
            int x = (int)(p.field_25144_c + (p.field_25146_a - p.field_25144_c) * (double)partialTicks - 4.0D);
            int y = (int)(p.field_25143_d + (p.field_25145_b - p.field_25143_d) * (double)partialTicks - 4.0D);
            float alpha = (float)(p.field_25129_r + (p.field_25133_n - p.field_25129_r) * (double)partialTicks);
            float r = (float)(p.field_25132_o + (p.field_25136_k - p.field_25132_o) * (double)partialTicks);
            float g = (float)(p.field_25131_p + (p.field_25135_l - p.field_25131_p) * (double)partialTicks);
            float b = (float)(p.field_25130_q + (p.field_25134_m - p.field_25130_q) * (double)partialTicks);
            GLManager.GL.Color4(r, g, b, alpha);
            DrawTexturedModalRect(x, y, 40, 0, 8, 8);
        }

    }
}