using CafeEventEditor.Components;
using CafeEventEditor.Components.Models;
using CafeEventEditor.Core.Components;
using CafeEventEditor.Services;
using CafeEventEditor.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeEventEditor.ViewModels;

public partial class FlowchartViewModel : Document
{
    public static LayoutConfig Layout { get; } = LayoutConfig.Load("FlowchartLayout");

    public CafeWriterHandle Handle { get; set; }

    [ObservableProperty]
    private FlowchartDrawingNode? _drawing;

    public FlowchartViewModel(string file) : base(Path.GetFileName(file))
    {
        Handle = CafeLoadManager.LoadFromFile(file);
        if (Handle.Bfev.Flowchart is null) {
            throw new InvalidDataException($"""
                No flowchart could be found in the event flow '{file}'.
                """);
        }

        Content = new FlowchartView(this);
        Drawing = new FlowchartDrawingNode(Handle.Bfev.Flowchart);
        Factory = new FlowchartNodeFactory();
        Templates = Factory.CreateTemplates();
    }
}
