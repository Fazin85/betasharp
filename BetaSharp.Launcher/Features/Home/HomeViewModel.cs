using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeViewModel(AccountsService accountsService, ClientService clientService, SkinService skinService) : ObservableObject
{
    [ObservableProperty]
    public partial Account? Account { get; set; }

    [ObservableProperty]
    public partial CroppedBitmap? Face { get; set; }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        // This doesn't get updated on sign out.
        Account = await accountsService.GetAsync();

        ArgumentNullException.ThrowIfNull(Account);

        if (!string.IsNullOrWhiteSpace(Account.Skin))
        {
            Face = await skinService.GetFaceAsync(Account.Skin);
        }
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        // Check if account's token has expired.
        ArgumentNullException.ThrowIfNull(Account);

        await clientService.DownloadAsync();

        // Probably should move this into a service.
        using var process = Process.Start(Path.Combine(AppContext.BaseDirectory, "Client", "BetaSharp.Client"), [Account.Name, Account.Token]);

        ArgumentNullException.ThrowIfNull(process);

        await process.WaitForExitAsync();
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Authentication));
        await accountsService.DeleteAsync();
    }
}
