using CommunityToolkit.Mvvm.ComponentModel;

namespace BetaSharp.Launcher.Features.Shell;

internal sealed partial class ShellViewModel : ObservableObject
{
    public string Greeting => "You must own a legitimate copy of Minecraft Java edition to use this client.";
}