using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.New;

internal sealed partial class NewViewModel(AuthenticationService authenticationService, DownloadingService downloadingService) : ObservableObject
{
    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        // What to do if the browser tab was closed?
        var token = await authenticationService.RequestMinecraftTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var name = await authenticationService.RequestMinecraftNameAsync(token);

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await downloadingService.DownloadMinecraftAsync();

        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "BetaSharpClient",
            Arguments = $"{name} {token}",
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        process.Start();
    }
}