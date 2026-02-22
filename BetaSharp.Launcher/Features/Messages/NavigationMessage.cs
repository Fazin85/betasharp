namespace BetaSharp.Launcher.Features.Messages;

internal sealed class NavigationMessage(Destination destination)
{
    public Destination Destination => destination;
}

internal enum Destination
{
    Splash,
    Authentication,
    Home
}
