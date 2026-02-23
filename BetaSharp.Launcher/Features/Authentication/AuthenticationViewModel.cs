using System;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Alert;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Authentication;

// Does this need a better name?
internal sealed partial class AuthenticationViewModel(AccountsService accountsService, AlertService alertService) : ObservableObject
{
    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        await accountsService.DeleteAsync();

        var token = await accountsService.AuthenticateAsync();

        if (token is null)
        {
            await alertService.ShowAsync("Authentication Failure", "The selected Microsoft account does not own a copy of Minecraft Java edition");
            return;
        }

        await accountsService.RefreshAsync(token.Value, DateTimeOffset.Now.AddSeconds(token.Expiration));

        WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Home));
    }
}
