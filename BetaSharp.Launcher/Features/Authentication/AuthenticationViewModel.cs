using System;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Authentication;

// Does this need a better name?
internal sealed partial class AuthenticationViewModel(AccountsService accountsService) : ObservableObject
{
    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        var token = await accountsService.AuthenticateAsync();

        if (token is null)
        {
            // Scream.
            return;
        }

        await accountsService.RefreshAsync(token.Value, DateTimeOffset.Now.AddSeconds(token.Expiration));

        WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Home));
    }
}
