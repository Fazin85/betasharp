using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeViewModel(AccountsService accountsService, ClientService clientService) : ObservableObject
{
    [ObservableProperty]
    public partial Account? Account { get; set; }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        Account = await accountsService.GetAsync();

        ArgumentNullException.ThrowIfNull(Account);
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        // Check if account's token has expired.
        ArgumentNullException.ThrowIfNull(Account);

        await clientService.DownloadAsync();

        ArgumentException.ThrowIfNullOrWhiteSpace(Account.Token);

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
