using CafeEventEditor.Builders.Attributes;
using CafeEventEditor.ViewModels;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;

namespace CafeEventEditor.Components.Menus;

public class FileMenu
{
    [Menu("Open", "File", "Ctrl + O", "fa-regular fa-folder-open")]
    public static async Task Open()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Open Event File", "Cafe Event Files:*.bfevfl;*.bfevtm;*.bfevfl.zs;*.bfevtm.zs;*.sbeventpack", instanceBrowserKey: "open-event-browser");
        if (await dialog.ShowDialog() is string file) {
            FlowchartViewModel flowchartEditor = new(file);
            DocumentManager.Shared.Documents.Add(flowchartEditor);
            DocumentManager.Shared.Current = flowchartEditor;
        }
    }

    [Menu("Exit", "File", "Ctrl + Q", "fa-solid fa-right-from-bracket", IsSeparator = true)]
    public static async Task Exit()
    {
        await App.Exit();
    }
}
