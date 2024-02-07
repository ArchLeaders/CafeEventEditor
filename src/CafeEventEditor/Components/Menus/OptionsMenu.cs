using CafeEventEditor.Builders.Attributes;
using CafeEventEditor.ViewModels;

namespace CafeEventEditor.Components.Menus;

public class OptionsMenu
{
    private static readonly SettingsViewModel _settings = new();

    [Menu("Settings", "Options", "Ctrl + ,", "fa-solid fa-gear")]
    public static Task Settings()
    {
        if (DocumentManager.Shared.Documents.IndexOf(_settings) == -1) {
            DocumentManager.Shared.Documents.Add(_settings);
        }

        DocumentManager.Shared.Current = _settings;
        return Task.CompletedTask;
    }
}
