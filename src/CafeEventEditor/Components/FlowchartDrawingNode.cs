using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;

namespace CafeEventEditor.Components;

public partial class FlowchartDrawingNode : ObservableDrawingNode
{
    public FlowchartDrawingNode(Flowchart flowchart)
    {
        Name = flowchart.Name;
        Height = 1000;
        Width = 1000;

        GenerateNodes(flowchart);
    }

    private void GenerateNodes(Flowchart flowchart)
    {
        throw new NotImplementedException();
    }

    public void ToFlowchart()
    {
        ArgumentNullException.ThrowIfNull(Name);

        // Flowchart flowchart = new(Name);

        // TODO: Convert the node tree to Events and EntryPoints
        throw new NotImplementedException();
    }

    public override bool CanConnectPin(IPin pin)
    {
        bool isConnecting = _editor.IsConnectorMoving();
        if (pin.Name is "Input") {
            return isConnecting;
        }

        if (pin.Parent is INode node && node.Name == "Fork") {
            return true;
        }

        return base.CanConnectPin(pin);
    }
}
