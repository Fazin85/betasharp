using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed partial class SplashViewModel(AuthenticationService authenticationService, GitHubService gitHubService, HomeViewModel homeViewModel) : ObservableObject
{
    [RelayCommand]
    private async Task InitializeAsync()
    {
        await authenticationService.InitializeAsync();
        await gitHubService.GetUpdatesAsync();

        WeakReferenceMessenger.Default.Send(new NavigationMessage(homeViewModel));
    }
}
