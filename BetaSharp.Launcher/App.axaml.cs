using System;
using System.IO;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetaSharp.Launcher.Features;
using BetaSharp.Launcher.Features.Shell;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

internal sealed partial class App : Application
{
    // Move to whatever the client uses?
    public static string Folder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BetaSharp Launcher");

    private readonly IServiceProvider _services = Bootstrapper.Build();

    public override void Initialize()
    {
        DataTemplates.Add(_services.GetRequiredService<ViewLocator>());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _services.GetRequiredService<ShellView>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(AccountService.Account))]
[JsonSerializable(typeof(XboxService.XboxUserRequest))]
[JsonSerializable(typeof(XboxService.XboxUserRequest.UserProperties))]
[JsonSerializable(typeof(XboxService.XboxUserResponse))]
[JsonSerializable(typeof(XboxService.XboxUserResponse.UserDisplayClaims))]
[JsonSerializable(typeof(XboxService.XboxUserResponse.UserDisplayClaims.UserXui))]
[JsonSerializable(typeof(XboxService.XboxTokenRequest))]
[JsonSerializable(typeof(XboxService.XboxTokenRequest.TokenProperties))]
[JsonSerializable(typeof(XboxService.XboxTokenResponse))]
[JsonSerializable(typeof(MinecraftService.MinecraftTokenRequest))]
[JsonSerializable(typeof(MinecraftService.MinecraftTokenResponse))]
[JsonSerializable(typeof(MinecraftService.MinecraftProfileResponse))]
[JsonSerializable(typeof(MinecraftService.MinecraftProfileResponse.Skin))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
