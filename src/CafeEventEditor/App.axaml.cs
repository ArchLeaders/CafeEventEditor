using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CafeEventEditor.Builders;
using CafeEventEditor.Views;
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

            MenuFactory menuFactory = new(shellView);
            shellView.MainMenu.ItemsSource = menuFactory.Items;

            menuFactory.Append<FileMenu>();

            XamlRoot = shellView;
            desktop.MainWindow = shellView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static Task Exit()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
