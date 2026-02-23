using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BetaSharp.Launcher.Features.Alert;

internal sealed partial class AlertView : Window
{
    public AlertView()
    {
        InitializeComponent();
        // Dispatcher.UIThread.Post(() => AlertBlock.Text = Message, DispatcherPriority.Loaded);
    }

    private void CloseClick(object? sender, RoutedEventArgs eventArgs)
    {
        Close();
    }
}
