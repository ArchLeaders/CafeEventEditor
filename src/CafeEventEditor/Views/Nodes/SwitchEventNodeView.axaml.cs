using Avalonia.Controls;
using CafeEventEditor.Extensions;
using CafeEventEditor.ViewModels.Nodes;

namespace CafeEventEditor.Views.Nodes;
public partial class SwitchEventNodeView : UserControl
{
    public SwitchEventNodeView()
    {
        InitializeComponent();
    }

    private void TextEditor_Initialized(object? sender, EventArgs e)
    {
        this.InitializeTextEditor<SwitchEventNode>(sender);
    }
}
