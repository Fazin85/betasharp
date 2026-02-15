using Avalonia.Controls;

namespace BetaSharp.Launcher.Features.New;

internal sealed partial class NewView : UserControl
{
    public NewView(NewViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}