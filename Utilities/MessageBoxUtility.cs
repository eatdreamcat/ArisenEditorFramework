using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ArisenEditorFramework.Utilities
{
    public static class MessageBoxUtility
    {
        public static async Task ShowMessageBoxStandard(string title, string text, ButtonEnum @enum = ButtonEnum.Ok, 
            Icon icon = Icon.None, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            var box = MessageBoxManager
            .GetMessageBoxStandard(title, text,
                @enum);
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (desktop.MainWindow != null)
                {
                    await box.ShowWindowDialogAsync(desktop.MainWindow);
                }
                else 
                {
                    await box.ShowAsync();
                }
            }
            else
            {
                await box.ShowAsync();
            }
        }
    }
}
