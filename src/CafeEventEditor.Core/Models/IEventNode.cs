using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;

namespace CafeEventEditor.Core.Models;

public interface IEventNode : INode
{
    Event AppendCafeEvent(EventHelper events, ActorHelper actors);
    IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent);
    string Info { get; }
}
