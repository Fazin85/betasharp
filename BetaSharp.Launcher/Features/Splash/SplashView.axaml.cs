using Avalonia.Controls;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed partial class SplashView : UserControl
{
    public SplashView(SplashViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
