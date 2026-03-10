using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ArisenEditorFramework.UI.Common;

public partial class LoadingWindow : Window
{
    public LoadingWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
