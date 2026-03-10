using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Alert;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Sessions;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    public partial Session? Session { get; set; }

    private readonly NavigationService _navigationService;
    private readonly StorageService _storageService;
    private readonly ClientService _clientService;
    private readonly AlertService _alertService;

    public HomeViewModel(
        NavigationService navigationService,
        StorageService storageService,
        ClientService clientService,
        AlertService alertService)
    {
        _navigationService = navigationService;
        _storageService = storageService;
        _clientService = clientService;
        _alertService = alertService;

        WeakReferenceMessenger.Default.Register<HomeViewModel, SessionMessage>(
            this,
            static (viewModel, message) => viewModel.Session = message.Session);
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        if (Session?.HasExpired ?? true)
        {
            _navigationService.Navigate<AuthenticationViewModel>();
            return;
        }

        try
        {
            ClientLaunchTarget target = ResolveClientLaunchTarget();

            await _clientService.DownloadAsync(target.WorkingDirectory);

            var info = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = target.Launcher,
                UseShellExecute = false,
                WorkingDirectory = target.WorkingDirectory
            };

            if (target.ClientEntryPoint != null)
            {
                info.ArgumentList.Add(target.ClientEntryPoint);
            }

            info.ArgumentList.Add(Session.Name);
            info.ArgumentList.Add(Session.Token);

            // Probably should move this into a service/view-model.
            using var process = Process.Start(info);

            ArgumentNullException.ThrowIfNull(process);

            await process.WaitForExitAsync();
        }
        catch (FileNotFoundException exception)
        {
            await _alertService.ShowAsync("Client Missing", exception.Message);
        }
    }

    private static ClientLaunchTarget ResolveClientLaunchTarget()
    {
        string rid = ResolveRuntimeIdentifier();
        string executableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "BetaSharp.Client.exe"
            : "BetaSharp.Client";

        string[] roots =
        [
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Client")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rid, "Client"))
        ];

        var attempted = new HashSet<string>(StringComparer.Ordinal);

        foreach (string clientRoot in roots)
        {
            string ridDirectory = Path.Combine(clientRoot, rid);
            string ridExecutable = Path.Combine(ridDirectory, executableName);
            if (File.Exists(ridExecutable))
            {
                return new ClientLaunchTarget(ridExecutable, null, ridDirectory);
            }

            string rootExecutable = Path.Combine(clientRoot, executableName);
            if (File.Exists(rootExecutable))
            {
                return new ClientLaunchTarget(rootExecutable, null, clientRoot);
            }

            string ridDll = Path.Combine(ridDirectory, "BetaSharp.Client.dll");
            if (File.Exists(ridDll))
            {
                return new ClientLaunchTarget("dotnet", ridDll, ridDirectory);
            }

            string rootDll = Path.Combine(clientRoot, "BetaSharp.Client.dll");
            if (File.Exists(rootDll))
            {
                return new ClientLaunchTarget("dotnet", rootDll, clientRoot);
            }

            attempted.Add(ridExecutable);
            attempted.Add(rootExecutable);
            attempted.Add(ridDll);
            attempted.Add(rootDll);
        }

        throw new FileNotFoundException(
            $"Could not locate a runnable client. Searched:{Environment.NewLine}- " +
            string.Join($"{Environment.NewLine}- ", attempted));
    }

    private static string ResolveRuntimeIdentifier()
    {
        string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "win"
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? "osx"
                : "linux";

        string architecture = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant()
        };

        return $"{os}-{architecture}";
    }

    [RelayCommand]
    private void SignOut()
    {
        _navigationService.Navigate<AuthenticationViewModel>();
        _storageService.Delete(nameof(Session));
    }

    private sealed record ClientLaunchTarget(string Launcher, string? ClientEntryPoint, string WorkingDirectory);
}
