using java.lang;

namespace BetaSharp;

public class UnexpectedThrowable : java.lang.Object
{
    public readonly string description;
    public readonly Throwable exception;

    public UnexpectedThrowable(string var1, Throwable var2)
    {
        description = var1;
        exception = var2;
    }
}