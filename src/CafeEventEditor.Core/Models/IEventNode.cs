using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;
using CafeEventEditor.Core.Components;

namespace CafeEventEditor.Core.Models;

public interface IEventNode : INode
{
    Event BuildRecursive(FlowchartBuilderContext context);
    IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent);
    string Info { get; }
}
