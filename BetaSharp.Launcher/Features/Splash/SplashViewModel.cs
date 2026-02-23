using System;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed partial class SplashViewModel(AccountsService accountsService) : ObservableObject
{
    [RelayCommand]
    private async Task InitializeAsync()
    {
        // Let everyone appreciate BetaSharp's logo.
        var delay = Task.Delay(TimeSpan.FromSeconds(2.5));

        await accountsService.InitializeAsync();

        var account = await accountsService.GetAsync();

        await delay;

        WeakReferenceMessenger.Default.Send(new NavigationMessage(account is null ? Destination.Authentication : Destination.Home));
    }
}
