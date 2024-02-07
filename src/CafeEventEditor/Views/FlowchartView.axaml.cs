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
    }
}
