using System.Threading.Tasks;
using BetaSharp.Launcher.Features.New;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed partial class SplashViewModel(GitHubService gitHubService, NewViewModel newViewModel) : ObservableObject
{
    [RelayCommand]
    private async Task InitializeAsync()
    {
        await gitHubService.GetUpdatesAsync();
        WeakReferenceMessenger.Default.Send(new NavigationMessage(newViewModel));
    }
}
