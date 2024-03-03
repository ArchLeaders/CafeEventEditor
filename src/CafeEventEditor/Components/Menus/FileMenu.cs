using CafeEventEditor.Core.Modals;
using CafeEventEditor.Services.Attributes;
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

    [Menu("Save", "File", "Ctrl + S", "fa-regular fa-floppy-disk")]
    public static async Task Save()
    {
        if (DocumentManager.Shared.Current is FlowchartViewModel flowchartViewModel) {
            AppStatus.Set($"Saving", "fa-solid fa-floppy-disk",
                isWorkingStatus: true, logLevel: LogLevel.Info);

            await flowchartViewModel.Save();
            AppStatus.Set($"Saved Successfully", "fa-regular fa-circle-check",
                isWorkingStatus: false, temporaryStatusTime: 1.5, LogLevel.Info);
        }
    }

    [Menu("Exit", "File", "Ctrl + Q", "fa-solid fa-right-from-bracket", IsSeparator = true)]
    public static async Task Exit()
    {
        await App.Exit();
    }
}
