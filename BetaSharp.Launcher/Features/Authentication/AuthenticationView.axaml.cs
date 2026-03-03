using Avalonia.Controls;

namespace BetaSharp.Launcher.Features.Authentication;

internal sealed partial class AuthenticationView : UserControl
{
    public AuthenticationView(AuthenticationViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
