using Avalonia.Controls;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeView : UserControl
{
    public HomeView(HomeViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
