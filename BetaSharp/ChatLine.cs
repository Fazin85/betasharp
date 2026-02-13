namespace BetaSharp;

public class ChatLine(string message)
{
    public string Message = message;
    public int UpdateCounter = 0;
}