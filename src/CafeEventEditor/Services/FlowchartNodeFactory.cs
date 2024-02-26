using Avalonia.NodeEditor.Core;
using CafeEventEditor.ViewModels.Nodes;

namespace CafeEventEditor.Services;

public class FlowchartNodeFactory : INodeFactory
{
    public IDrawingNode CreateDrawing(string? name = null)
    {
        throw new NotImplementedException();
    }

    public IList<INodeTemplate> CreateTemplates()
    {
        return [
            EntryPointNode.Template,
            ActionEventNode.Template,
            ForkEventNode.Template,
            JoinEventNode.Template,
            SubflowEventNode.Template,
            SwitchEventNode.Template,
        ];
    }
}
