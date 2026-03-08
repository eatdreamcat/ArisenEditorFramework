using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ArisenEditorFramework.Assets;

public partial class AssetBrowserControl : UserControl
{
    public AssetBrowserControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control control && control.DataContext is AssetItemViewModel item)
        {
            if (item.IsDirectory)
            {
                if (DataContext is AssetBrowserViewModel vm)
                {
                    vm.CurrentPath = item.FullPath;
                }
            }
        }
    }

    private async void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.DataContext is AssetItemViewModel item && !item.IsDirectory)
        {
            var dragData = new DataObject();
            // We can provide multiple formats: the view model, the path, or a generic "HierarchyItem" 
            // if we want it to be compatible with our existing inspector drop zones
            dragData.Set("AssetItem", item);
            dragData.Set("HierarchyItem", item); // Map to HierarchyItem for Inspector compatibility
            dragData.Set(DataFormats.Files, new[] { item.FullPath });

            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy | DragDropEffects.Move);
        }
    }
}
