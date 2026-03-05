namespace Beta3D;

public class Std140SizeCalculator
{
    public int Size { get; private set; }

    private Std140SizeCalculator Align(int alignment)
    {
        Size = (Size + alignment - 1) & ~(alignment - 1);
        return this;
    }

    public Std140SizeCalculator Float() { Align(4); Size += 4; return this; }
    public Std140SizeCalculator Int() { Align(4); Size += 4; return this; }
    public Std140SizeCalculator Vec2() { Align(8); Size += 8; return this; }
    public Std140SizeCalculator IVec2() { Align(8); Size += 8; return this; }
    public Std140SizeCalculator Vec3() { Align(16); Size += 16; return this; }
    public Std140SizeCalculator IVec3() { Align(16); Size += 16; return this; }
    public Std140SizeCalculator Vec4() { Align(16); Size += 16; return this; }
    public Std140SizeCalculator IVec4() { Align(16); Size += 16; return this; }
    public Std140SizeCalculator Mat4() { Align(16); Size += 64; return this; }
}
