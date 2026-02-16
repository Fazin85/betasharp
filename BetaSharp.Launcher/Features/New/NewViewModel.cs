using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.New.Authentication;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.New;

internal sealed partial class NewViewModel(AuthenticationService authenticationService, DownloadingService downloadingService, HomeViewModel homeViewModel) : ObservableObject
{
    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        var authentication = authenticationService.AuthenticateAsync();

        await Task.WhenAll(authentication, downloadingService.DownloadAsync());

        var session = await authentication;

        WeakReferenceMessenger.Default.Send(new NavigationMessage(homeViewModel));
    }
}
