using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BetaSharp.Util.Maths.Noise;

internal class PerlinNoiseSampler : NoiseSampler
{
    private readonly int[] _permutations;
    private readonly double _x;
    private readonly double _y;
    private readonly double _z;

    public PerlinNoiseSampler() : this(new JavaRandom())
    {
    }

    public PerlinNoiseSampler(JavaRandom rand)
    {
        _permutations = new int[512];
        _x = rand.NextDouble() * 256.0D;
        _y = rand.NextDouble() * 256.0D;
        _z = rand.NextDouble() * 256.0D;

        for (int i = 0; i < 256; i++)
        {
            _permutations[i] = i;
        }

        for (int i = 0; i < 256; ++i)
        {
            int j = rand.NextInt(256 - i) + i;
            (_permutations[i], _permutations[j]) = (_permutations[j], _permutations[i]);
            _permutations[i + 256] = _permutations[i];
        }
    }

    public double GenerateNoise(double x, double y)
    {
        return GenerateNoise(x, y, 0.0D);
    }

    private double GenerateNoise(double x, double y, double z)
    {
        x += _x;
        y += _y;
        z += _z;

        int xInt = Floor(x);
        int yInt = Floor(y);
        int zInt = Floor(z);

        int xMod255 = xInt & 255;
        int yMod255 = yInt & 255;
        int zMod255 = zInt & 255;

        x -= xInt;
        y -= yInt;
        z -= zInt;

        double sX = x * x * x * (x * (x * 6.0D - 15.0D) + 10.0D);
        double sY = y * y * y * (y * (y * 6.0D - 15.0D) + 10.0D);
        double sZ = z * z * z * (z * (z * 6.0D - 15.0D) + 10.0D);

        ref int p = ref MemoryMarshal.GetArrayDataReference(_permutations);

        int a = Unsafe.Add(ref p, xMod255) + yMod255;
        int aa = Unsafe.Add(ref p, a) + zMod255;
        int ab = Unsafe.Add(ref p, a + 1) + zMod255;
        int b = Unsafe.Add(ref p, xMod255 + 1) + yMod255;
        int ba = Unsafe.Add(ref p, b) + zMod255;
        int bb = Unsafe.Add(ref p, b + 1) + zMod255;

        return Lerp(sZ, Lerp(sY, Lerp(sX, Grad(Unsafe.Add(ref p, aa), x, y, z),
                    Grad(Unsafe.Add(ref p, ba), x - 1, y, z)),
                Lerp(sX, Grad(Unsafe.Add(ref p, ab), x, y - 1, z),
                    Grad(Unsafe.Add(ref p, bb), x - 1, y - 1, z))),
            Lerp(sY, Lerp(sX, Grad(Unsafe.Add(ref p, aa + 1), x, y, z - 1),
                    Grad(Unsafe.Add(ref p, ba + 1), x - 1, y, z - 1)),
                Lerp(sX, Grad(Unsafe.Add(ref p, ab + 1), x, y - 1, z - 1),
                    Grad(Unsafe.Add(ref p, bb + 1), x - 1, y - 1, z - 1))));
    }

    public void Sample(double[] buffer, double xStart, double yStart, double zStart, int xSize, int ySize, int zSize, double xFrequency, double yFrequency, double zFrequency, double inverseAmplitude)
    {
        int counter = 0;
        double amplitude = 1.0D / inverseAmplitude;

        ref double bufRef = ref MemoryMarshal.GetArrayDataReference(buffer);
        ref int permRef = ref MemoryMarshal.GetArrayDataReference(_permutations);

        if (ySize == 1) // 2D
        {
            for (int x = 0; x < xSize; ++x)
            {
                double xCoord = (xStart + x) * xFrequency + _x;
                int xInt = Floor(xCoord);
                int xMod255 = xInt & 255;
                xCoord -= xInt;
                double xFinal = xCoord * xCoord * xCoord * (xCoord * (xCoord * 6.0D - 15.0D) + 10.0D);

                for (int z = 0; z < zSize; ++z)
                {
                    double zCoord = (zStart + z) * zFrequency + _z;
                    int zInt = Floor(zCoord);
                    int zMod255 = zInt & 255;
                    zCoord -= zInt;
                    double zFinal = zCoord * zCoord * zCoord * (zCoord * (zCoord * 6.0D - 15.0D) + 10.0D);

                    int aa = Unsafe.Add(ref permRef, xMod255);
                    int ab = Unsafe.Add(ref permRef, aa) + zMod255;
                    int ba = Unsafe.Add(ref permRef, xMod255 + 1);
                    int bb = Unsafe.Add(ref permRef, ba) + zMod255;

                    double xLerpZ0 = Lerp(xFinal, Grad(Unsafe.Add(ref permRef, ab), xCoord, zCoord),
                        Grad(Unsafe.Add(ref permRef, bb), xCoord - 1, 0, zCoord));
                    double xLerpZ1 = Lerp(xFinal, Grad(Unsafe.Add(ref permRef, ab + 1), xCoord, 0, zCoord - 1),
                        Grad(Unsafe.Add(ref permRef, bb + 1), xCoord - 1, 0, zCoord - 1));

                    double finalNoise = Lerp(zFinal, xLerpZ0, xLerpZ1);
                    Unsafe.Add(ref bufRef, counter++) += finalNoise * amplitude;
                }
            }
        }
        else // 3D
        {
            int oldY = -1;
            double xLerpY0Z0 = 0.0D, xLerpY1Z0 = 0.0D, xLerpY0Z1 = 0.0D, xLerpY1Z1 = 0.0D;

            for (int x = 0; x < xSize; ++x)
            {
                double xCoord = (xStart + x) * xFrequency + _x;
                int xInt = Floor(xCoord);
                int xMod255 = xInt & 255;
                xCoord -= xInt;
                double xFinal = xCoord * xCoord * xCoord * (xCoord * (xCoord * 6.0D - 15.0D) + 10.0D);

                for (int z = 0; z < zSize; ++z)
                {
                    double zCoord = (zStart + z) * zFrequency + _z;
                    int zInt = Floor(zCoord);
                    int zMod255 = zInt & 255;
                    zCoord -= zInt;
                    double zFinal = zCoord * zCoord * zCoord * (zCoord * (zCoord * 6.0D - 15.0D) + 10.0D);

                    for (int y = 0; y < ySize; ++y)
                    {
                        double yCoord = (yStart + y) * yFrequency + _y;
                        int yInt = Floor(yCoord);
                        int yMod255 = yInt & 255;
                        yCoord -= yInt;
                        double yFinal = yCoord * yCoord * yCoord * (yCoord * (yCoord * 6.0D - 15.0D) + 10.0D);

                        if (y == 0 || yMod255 != oldY)
                        {
                            oldY = yMod255;
                            int a = Unsafe.Add(ref permRef, xMod255) + yMod255;
                            int aa = Unsafe.Add(ref permRef, a) + zMod255;
                            int ab = Unsafe.Add(ref permRef, a + 1) + zMod255;
                            int b = Unsafe.Add(ref permRef, xMod255 + 1) + yMod255;
                            int ba = Unsafe.Add(ref permRef, b) + zMod255;
                            int bb = Unsafe.Add(ref permRef, b + 1) + zMod255;

                            xLerpY0Z0 = Lerp(xFinal,
                                Grad(Unsafe.Add(ref permRef, aa), xCoord, yCoord, zCoord),
                                Grad(Unsafe.Add(ref permRef, ba), xCoord - 1, yCoord, zCoord));
                            xLerpY1Z0 = Lerp(xFinal,
                                Grad(Unsafe.Add(ref permRef, ab), xCoord, yCoord - 1, zCoord),
                                Grad(Unsafe.Add(ref permRef, bb), xCoord - 1, yCoord - 1, zCoord));
                            xLerpY0Z1 = Lerp(xFinal,
                                Grad(Unsafe.Add(ref permRef, aa + 1), xCoord, yCoord, zCoord - 1),
                                Grad(Unsafe.Add(ref permRef, ba + 1), xCoord - 1, yCoord, zCoord - 1));
                            xLerpY1Z1 = Lerp(xFinal,
                                Grad(Unsafe.Add(ref permRef, ab + 1), xCoord, yCoord - 1, zCoord - 1),
                                Grad(Unsafe.Add(ref permRef, bb + 1), xCoord - 1, yCoord - 1, zCoord - 1));
                        }

                        double yLerpZ0 = Lerp(yFinal, xLerpY0Z0, xLerpY1Z0);
                        double yLerpZ1 = Lerp(yFinal, xLerpY0Z1, xLerpY1Z1);
                        double finalNoise = Lerp(zFinal, yLerpZ0, yLerpZ1);

                        Unsafe.Add(ref bufRef, counter++) += finalNoise * amplitude;
                    }
                }
            }
        }
    }
}
