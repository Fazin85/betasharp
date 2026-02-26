namespace BetaSharp;

public class Session(string username, string sessionId, string? skinUrl = null)
{
    public string username = username;
    public string sessionId = sessionId;
    public string? skinUrl = skinUrl;
}
