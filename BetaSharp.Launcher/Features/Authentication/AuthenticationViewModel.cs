using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Authentication;

// Does this need a better name?
internal sealed partial class AuthenticationViewModel(
    AccountService accountService,
    AuthenticationService authenticationService,
    XboxService xboxService,
    MinecraftService minecraftService) : ObservableObject
{
    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        string microsoft = await authenticationService.AuthenticateAsync();

        var xbox = await xboxService.GetAsync(microsoft);

        var minecraft = await minecraftService.GetTokenAsync(xbox.Token, xbox.Hash);

        var profile = await minecraftService.GetProfileAsync(minecraft.Token);

        await accountService.UpdateAsync(profile.Name, profile.Skin, minecraft.Token, minecraft.Expiration);

        WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Home));
    }
}
