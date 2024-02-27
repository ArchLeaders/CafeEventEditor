using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CafeEventEditor.Core.Modals;
using TextMateSharp.Grammars;

namespace CafeEventEditor.Extensions;

public static class AvaloniaTextEditorExtension
{
    public static void InitializeTextEditor<T>(this UserControl control, object? sender) where T : IParameterizedEvent
    {
        if (sender is not TextEditor editor) {
            return;
        }

        if (control.DataContext is T node) {
            editor.Text = node.Parameters;

            editor.TextChanged += (s, e) => {
                node.Parameters = editor.Text;
            };
        }

        RegistryOptions registryOptions = new(Application.Current?.ActualThemeVariant == ThemeVariant.Dark
            ? ThemeName.DarkPlus : ThemeName.LightPlus
        );

        TextMate.Installation textMateInstallation = editor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar("source.yaml");
    }
}
