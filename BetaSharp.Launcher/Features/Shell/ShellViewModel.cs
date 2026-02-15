using System.ComponentModel;
using BetaSharp.Launcher.Features.Splash;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BetaSharp.Launcher.Features.Shell;

internal sealed partial class ShellViewModel(SplashViewModel splashViewModel) : ObservableObject
{
    [ObservableProperty]
    public partial INotifyPropertyChanged Current { get; set; } = splashViewModel;
}