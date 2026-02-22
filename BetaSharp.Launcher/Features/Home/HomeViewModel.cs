using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BetaSharp.Launcher.Features.Messages;
using BetaSharp.Launcher.Features.Mojang;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeViewModel(
    AuthenticationService authenticationService,
    AccountService accountService,
    MojangClient minecraftService,
    DownloadingService downloadingService) : ObservableObject
{
    [ObservableProperty]
    public partial bool IsReady { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = "...";

    [ObservableProperty]
    public partial CroppedBitmap? Face { get; set; }

    private string? _token;
    private DateTimeOffset _expiration;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsReady = false;

        await Task.Yield();

        var account = await accountService.GetAsync();

        if (account is null)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Authentication));
            return;
        }

        Name = account.Name;

        // ArgumentException.ThrowIfNullOrWhiteSpace(account.Skin);

        // Face = await minecraftService.GetFaceAsync(account.Skin);

        _token = account.Token;
        _expiration = account.Expiration;

        IsReady = true;
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        if (DateTimeOffset.Now > _expiration)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Authentication));
            return;
        }

        await downloadingService.DownloadAsync();

        ArgumentException.ThrowIfNullOrWhiteSpace(_token);

        using var process = Process.Start(Path.Combine(AppContext.BaseDirectory, "Client", "BetaSharp.Client"), [Name, _token]);

        ArgumentNullException.ThrowIfNull(process);

        await process.WaitForExitAsync();
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        await authenticationService.SignOutAsync();
        WeakReferenceMessenger.Default.Send(new NavigationMessage(Destination.Authentication));
    }
}
