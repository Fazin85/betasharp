using System;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed partial class SplashViewModel(NavigationService navigationService, AccountsService accountsService) : ObservableObject
{
    [RelayCommand]
    private async Task InitializeAsync()
    {
        // Let everyone appreciate BetaSharp's logo.
        var delay = Task.Delay(TimeSpan.FromSeconds(2.5));

        await accountsService.InitializeAsync();

        var account = await accountsService.GetAsync();

        await delay;

        if (account is null)
        {
            navigationService.Navigate<AuthenticationViewModel>();
            return;
        }

        navigationService.Navigate<HomeViewModel>();
    }
}
