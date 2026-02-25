namespace BetaSharp.Modding;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ModSideAttribute : Attribute
{
    public Side Side { get; }

    public ModSideAttribute(Side side)
    {
        Side = side;
    }
}
