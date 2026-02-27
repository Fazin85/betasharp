using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiParticle
{
    private readonly List<Particle> particles = [];
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
            p.UpdatePrevious();
            p.Update(this);
            if (p.PendingRemoval)
            {
                particles.RemoveAt(i--);
            }
        }
    }

    public void render(float tickDelta)
    {
        mc.textureManager.BindTexture(mc.textureManager.GetTextureId("/gui/particles.png"));

        for (int i = 0; i < particles.Count; ++i)
        {
            Particle p = particles[i];
            int x = (int)(p.PrevX + (p.X - p.PrevX) * tickDelta - 4);
            int y = (int)(p.PrevY + (p.Y - p.PrevY) * tickDelta - 4);
            float alpha = (float)(p.PrevAlpha + (p.Alpha - p.PrevAlpha) * tickDelta);
            float r = (float)(p.PrevR + (p.R - p.PrevR) * tickDelta);
            float g = (float)(p.PrevG + (p.G - p.PrevG) * tickDelta);
            float b = (float)(p.PrevB + (p.B - p.PrevB) * tickDelta);
            GLManager.GL.Color4(r, g, b, alpha);
            Minecraft.DrawTextureRegion(x, y, 40, 0, 8, 8);
        }
    }
}
