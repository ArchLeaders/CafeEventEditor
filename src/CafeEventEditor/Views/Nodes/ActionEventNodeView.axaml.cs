using Avalonia.Controls;
using CafeEventEditor.Extensions;
using CafeEventEditor.ViewModels.Nodes;

namespace CafeEventEditor.Views.Nodes;
public partial class ActionEventNodeView : UserControl
{
    public ActionEventNodeView()
    {
        InitializeComponent();
    }

    private void TextEditor_Initialized(object? sender, EventArgs e)
    {
        this.InitializeTextEditor<ActionEventNode>(sender);
    }
}
