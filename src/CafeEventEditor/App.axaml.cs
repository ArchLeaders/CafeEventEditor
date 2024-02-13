using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CafeEventEditor.Services;
using CafeEventEditor.Components;
using CafeEventEditor.Components.Menus;
using CafeEventEditor.Components.Models;
using CafeEventEditor.Core;
using CafeEventEditor.Views;
using ConfigFactory.Avalonia.Helpers;
using FluentAvalonia.UI.Controls;
using System.Reflection;

namespace CafeEventEditor;

public partial class App : Application
{
    public static string? Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
    public static string Title { get; } = $"Cafe Event Editor";
    public static string ReleaseUrl { get; } = $"https://github.com/ArchLeaders/CafeEventEditor/releases/{Version}";
    public static TopLevel? XamlRoot { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            ShellView shellView = new();
            XamlRoot = desktop.MainWindow = shellView;

            MenuFactory menuFactory = new(shellView);
            shellView.MainMenu.ItemsSource = menuFactory.Items;
            menuFactory.Append<FileMenu>();
            menuFactory.Append<OptionsMenu>();

            BrowserDialog.StorageProvider = desktop.MainWindow.StorageProvider;
            Config.SetTheme = (theme) => {
                RequestedThemeVariant = theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
            };

            Config.SetTheme(Config.Shared.Theme);

            DocumentManager.Shared.Documents.Add(new Document("Welcome", Symbol.Home) {
                Content = new WelcomeView()
            });
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static Task Exit()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
