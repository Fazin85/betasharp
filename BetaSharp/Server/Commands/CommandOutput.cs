namespace BetaSharp.Server.Commands;

public interface CommandOutput
{
    void SendMessage(string message);

    string GetName();
}