using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CafeEventEditor.ViewModels.Nodes;
using TextMateSharp.Grammars;

namespace CafeEventEditor.Views.Nodes;
public partial class ActionEventNodeView : UserControl
{
    public ActionEventNodeView()
    {
        InitializeComponent();
    }

    private void TextEditor_Initialized(object? sender, EventArgs e)
    {
        if (sender is not TextEditor editor) {
            return;
        }

        if (DataContext is ActionEventNode node) {
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
