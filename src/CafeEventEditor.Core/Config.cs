using CafeEventEditor.Core.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;

namespace CafeEventEditor.Core;

public partial class Config : ConfigModule<Config>
{
    public override string Name { get; } = "CafeEventEditor";

    public static Action<string>? SetTheme { get; set; }

    [ObservableProperty]
    [property: ConfigFactory.Core.Attributes.Config(
        Header = "Theme",
        Description = "",
        Group = "Application")]
    [property: ConfigFactory.Core.Attributes.DropdownConfig("Dark", "Light")]
    private string _theme = "Dark";

    [ObservableProperty]
    [property: ConfigFactory.Core.Attributes.Config(
        Header = "ZSTD Dictionaries",
        Description = "The absolute path to a *.pack.zs or ZSTD dictionary file.",
        Group = "Compression")]
    [property: ConfigFactory.Core.Attributes.BrowserConfig(
        BrowserMode = ConfigFactory.Core.Attributes.BrowserMode.OpenFile,
        InstanceBrowserKey = "dict-path-browser",
        Filter = "Any:*.*|Pack:*.pack.zs|Dict:*.dict")]
    private string _dictionaries = string.Empty;

    partial void OnThemeChanged(string value)
    {
        SetTheme?.Invoke(value);
    }

    partial void OnDictionariesChanged(string value)
    {
        ZstdHelper.LoadDictionaries(value);
    }
}
