using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher.Features.Shell;

internal sealed class NavigationService(IServiceProvider services, ShellViewModel shellViewModel)
{
    public void Navigate<T>() where T : INotifyPropertyChanged
    {
        shellViewModel.Current = services.GetRequiredService<T>();
    }
}
