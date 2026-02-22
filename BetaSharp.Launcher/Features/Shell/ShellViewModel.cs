using System;
using System.ComponentModel;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Messages;
using BetaSharp.Launcher.Features.Splash;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Shell;

internal sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    public partial INotifyPropertyChanged Current { get; set; }

    public ShellViewModel(SplashViewModel splashViewModel, AuthenticationViewModel authenticationViewModel, HomeViewModel homeViewModel)
    {
        Current = splashViewModel;

        WeakReferenceMessenger.Default.Register<ShellViewModel, NavigationMessage>(this, (instance, message) => instance.Current = message.Destination switch
        {
            Destination.Splash => splashViewModel,
            Destination.Authentication => authenticationViewModel,
            Destination.Home => homeViewModel,
            _ => throw new ArgumentOutOfRangeException(nameof(message))
        });
    }
}
