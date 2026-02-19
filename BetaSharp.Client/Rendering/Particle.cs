using BetaSharp.Client.Guis;

namespace BetaSharp.Client.Rendering;

public class Particle : java.lang.Object
{
    public double X;
    public double Y;
    public double PrevX;
    public double PrevY;
    public double VelocityX;
    public double VelocityY;
    public double Friction;
    public bool PendingRemoval;
    public int Age;
    public int Lifetime;
    public double R;
    public double G;
    public double B;
    public double Alpha;
    public double PrevR;
    public double PrevG;
    public double PrevB;
    public double PrevAlpha;

    public void Update(GuiParticle particleGui)
    {
        X += VelocityX;
        Y += VelocityY;
        VelocityX *= Friction;
        VelocityY *= Friction;
        VelocityY += 0.1;
        if (++Age > Lifetime)
        {
            PendingRemoval = true;
        }

        Alpha = 2 - (double)Age / Lifetime * 2;
        if (Alpha > 1)
        {
            Alpha = 1;
        }

        Alpha *= Alpha;
        Alpha *= 0.5;
    }

    public void UpdatePrevious()
    {
        PrevR = R;
        PrevG = G;
        PrevB = B;
        PrevAlpha = Alpha;
        PrevX = X;
        PrevY = Y;
    }
}
