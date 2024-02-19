using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;
using CafeEventEditor.Components;
using CafeEventEditor.ViewModels.Nodes;

namespace CafeEventEditor.Services;

public class FlowchartNodeFactory(Flowchart flowchart) : INodeFactory
{
    private readonly Flowchart _flowchart = flowchart;

    public IDrawingNode CreateDrawing(string? name = null)
    {
        return new FlowchartDrawingNode(_flowchart) {
            
        };
    }

    public IList<INodeTemplate> CreateTemplates()
    {
        return [];
    }
}
