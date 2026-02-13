namespace BetaSharp;

public class JString : java.lang.Object
{
    public string value;

    public JString(string s)
    {
        value = s;
    }

    public override bool equals(object obj)
    {
        return obj is JString other && value.Equals(other.value);
    }

    public override int hashCode()
    {
        return value.GetHashCode();
    }
}