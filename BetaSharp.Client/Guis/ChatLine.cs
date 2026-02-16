namespace BetaSharp.Client.Guis;

public class ChatLine(string message)
{
    public string Message = message;
    public int UpdateCounter = 0;
}
