using Avalonia.Controls;
using CafeEventEditor.ViewModels;

namespace CafeEventEditor.Views;
public partial class FlowchartView : UserControl
{
    public FlowchartView()
    {
        InitializeComponent();
        DataContext = new FlowchartViewModel(@"D:\bin\Bfev\DmF_SY_FairyWorking.bfevfl");
    }

    public FlowchartView(FlowchartViewModel context)
    {
        InitializeComponent();
        DataContext = context;
        Editor.TemplateApplied += (s, e) => {
            context.ZoomBorder = Editor.ZoomControl ?? throw new InvalidOperationException("""
                Editor.ZoomBorder should not be null
                """);
        };

    }

    private void LeftPanel_DragCompleted(object? sender, Avalonia.Input.VectorEventArgs e)
    {
        SetLayout();
    }

    private void RightPanel_DragCompleted(object? sender, Avalonia.Input.VectorEventArgs e)
    {
        SetLayout();
    }

    private void SetLayout()
    {
        FlowchartViewModel.Layout.LeftPanel = Root.ColumnDefinitions[0].Width;
        FlowchartViewModel.Layout.CenterPanel = Root.ColumnDefinitions[2].Width;
        FlowchartViewModel.Layout.RightPanel = Root.ColumnDefinitions[4].Width;
        FlowchartViewModel.Layout.Save();
    }
}
