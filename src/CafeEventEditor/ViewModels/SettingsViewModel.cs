using CafeEventEditor.Components.Models;
using CafeEventEditor.Core;
using ConfigFactory;
using ConfigFactory.Avalonia;
using ConfigFactory.Models;
using FluentAvalonia.UI.Controls;

namespace CafeEventEditor.ViewModels;

public partial class SettingsViewModel : Document
{
    private static readonly ConfigPage _configPage = new();

    static SettingsViewModel()
    {
        if (_configPage.DataContext is ConfigPageModel context) {
            context.SecondaryButtonIsEnabled = false;
            context.Append<Config>();
        }
    }

    public SettingsViewModel() : base("Settings", Symbol.Settings)
    {
        Content = _configPage;
    }
}
