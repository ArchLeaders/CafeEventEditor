using Avalonia.NodeEditor.Controls;
using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
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

    [ObservableProperty]
    private NodeZoomBorder? _zoomBorder = new();

    [ObservableProperty]
    private INode? _selected;

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

        Drawing.SelectionChanged += (s, e) => {
            if (s is ObservableDrawingNode drawing) {
                Selected = drawing.GetSelectedNodes()?.FirstOrDefault();
            }
        };
    }

    public Task Save()
    {
        if (Drawing is not FlowchartDrawingNode drawing) {
            throw new InvalidOperationException($"""
                Expected {nameof(FlowchartDrawingNode)} but found '{Drawing?.GetType().Name ?? "NULL"}'
                """);
        }

        Handle.Bfev.Flowcharts.Clear();

        Flowchart flowchart = drawing.BuildFlowchart();
        Handle.Bfev.Flowcharts[flowchart.Name] = flowchart;
        Handle.Bfev.FileName = flowchart.Name;

        byte[] data = Handle.Bfev.ToBinary();
        Handle.Writer(data);

        return Task.CompletedTask;
    }

    partial void OnZoomBorderChanged(NodeZoomBorder? value)
    {
        if (Drawing is null || value is null) {
            return;
        }

        value.Pan((Drawing.Width / 2) - 200, (Drawing.Height / 2) - 200);

        value.ZoomOut();
        value.ZoomOut();
        value.ZoomOut();
        value.ZoomOut();
        value.ZoomOut();
        value.ZoomOut();
    }
}
