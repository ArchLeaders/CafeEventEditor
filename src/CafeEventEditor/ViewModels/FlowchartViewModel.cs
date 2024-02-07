using BfevLibrary.Core;
using CafeEventEditor.Components;
using CafeEventEditor.Components.Models;
using CafeEventEditor.Core.Components;
using CafeEventEditor.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeEventEditor.ViewModels;

public partial class FlowchartViewModel : Document
{
    public static LayoutConfig Layout { get; } = LayoutConfig.Load("FlowchartLayout");

    public CafeWriterHandle Handle { get; set; }

    [ObservableProperty]
    private Flowchart? _flowchart;

    public FlowchartViewModel(string file) : base(Path.GetFileName(file))
    {
        Handle = CafeLoadManager.LoadFromFile(file);
        Flowchart = Handle.Bfev.Flowchart;
        Content = new FlowchartView(this);
    }
}
