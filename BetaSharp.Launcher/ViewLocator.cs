using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Splash;

namespace BetaSharp.Launcher;

internal sealed class ViewLocator(SplashView splashView, AuthenticationView authenticationView, HomeView homeView) : IDataTemplate
{
    public Control Build(object? instance)
    {
        string? name = instance?.GetType().Name.Replace("ViewModel", "View", StringComparison.Ordinal);

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return name switch
        {
            nameof(SplashView) => splashView,
            nameof(AuthenticationView) => authenticationView,
            nameof(HomeView) => homeView,
            _ => throw new ArgumentOutOfRangeException(nameof(instance))
        };
    }

    public bool Match(object? instance)
    {
        return instance is INotifyPropertyChanged;
    }
}
