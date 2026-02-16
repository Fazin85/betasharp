using BetaSharp.Client.Guis;

namespace BetaSharp.Client.Rendering;

public class Particle : java.lang.Object
{
    private static readonly java.util.Random field_25128_s = new();
    public double field_25146_a;
    public double field_25145_b;
    public double field_25144_c;
    public double field_25143_d;
    public double field_25142_e;
    public double field_25141_f;
    public double field_25140_g;
    public bool field_25139_h;
    public int field_25138_i;
    public int field_25137_j;
    public double field_25136_k;
    public double field_25135_l;
    public double field_25134_m;
    public double field_25133_n;
    public double field_25132_o;
    public double field_25131_p;
    public double field_25130_q;
    public double field_25129_r;

    public void func_25125_a(GuiParticle var1)
    {
        field_25146_a += field_25142_e;
        field_25145_b += field_25141_f;
        field_25142_e *= field_25140_g;
        field_25141_f *= field_25140_g;
        field_25141_f += 0.1D;
        if (++field_25138_i > field_25137_j)
        {
            func_25126_b();
        }

        field_25133_n = 2.0D - (double)field_25138_i / (double)field_25137_j * 2.0D;
        if (field_25133_n > 1.0D)
        {
            field_25133_n = 1.0D;
        }

        field_25133_n *= field_25133_n;
        field_25133_n *= 0.5D;
    }

    public void func_25127_a()
    {
        field_25132_o = field_25136_k;
        field_25131_p = field_25135_l;
        field_25130_q = field_25134_m;
        field_25129_r = field_25133_n;
        field_25144_c = field_25146_a;
        field_25143_d = field_25145_b;
    }

    public void func_25126_b()
    {
        field_25139_h = true;
    }
}
