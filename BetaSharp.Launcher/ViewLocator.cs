using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

internal sealed class ViewLocator(IServiceProvider services) : IDataTemplate
{
    public Control? Build(object? instance)
    {
        var name = instance?.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var type = Type.GetType(name);

        ArgumentNullException.ThrowIfNull(type);

        return (Control?) ActivatorUtilities.CreateInstance(services, type);
    }

    public bool Match(object? instance)
    {
        return instance is INotifyPropertyChanged;
    }
}