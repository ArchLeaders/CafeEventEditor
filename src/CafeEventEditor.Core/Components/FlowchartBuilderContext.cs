#pragma warning disable CA1822

using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;
using BfevLibrary.Core.Collections;
using CafeEventEditor.Core.Models;

namespace CafeEventEditor.Core.Components;

public class FlowchartBuilderContext
{
    private readonly Dictionary<IPin, List<IConnector>> _connectorsByStartPin = [];

    public ActorList Actors { get; }
    public EventList Events { get; }
    public Stack<ForkEvent> ForkEvents { get; } = [];
    public Dictionary<IEventNode, short> Indices { get; } = [];

    public FlowchartBuilderContext(Flowchart flowchart, IList<IConnector> connectors)
    {
        flowchart.Actors = Actors = new(flowchart);
        flowchart.Events = Events = new(flowchart);

        foreach (IConnector connector in connectors) {
            if (connector.Start is null) {
                continue;
            }

            if (!_connectorsByStartPin.TryGetValue(connector.Start, out List<IConnector>? result)) {
                _connectorsByStartPin[connector.Start] = result = [];
            }

            result.Add(connector);
        }
    }

    public short GetEventIndex(INode node, int skipPinCount = 1)
    {
        if (GetNodeChild(node, skipPinCount) is not IEventNode nextEventNode) {
            return -1;
        }

        if (Indices.TryGetValue(nextEventNode, out short index)) {
            return nextEventNode is IJoinEventNode ? (short)-1 : index;
        }

        if (nextEventNode.BuildRecursive(this) is not Event cafeEvent) {
            return -1;
        }

        // Check again because BuildRecursive
        // can mutate the indices
        if (Indices.TryGetValue(nextEventNode, out index)) {
            return nextEventNode is IJoinEventNode ? (short)-1 : index;
        }

        Indices[nextEventNode] = index = Convert.ToInt16(Events.Count);
        Events.Add(cafeEvent);
        return cafeEvent is JoinEvent ? (short)-1 : index;
    }

    public short GetEventIndex(Event cafeEvent, IEventNode node)
    {
        if (!Indices.TryGetValue(node, out short index)) {
            Indices[node] = index = Convert.ToInt16(Events.Count);
            Events.Add(cafeEvent);
        }

        return index;
    }

    public IEventNode? GetNodeChild(INode node, int skipPinCount = 1)
    {
        IEventNode[] nodes = GetNodeChildren(node, skipPinCount).ToArray();

        if (nodes.Length == 0) {
            return null;
        }

        if (nodes.Length > 1) {
            throw new InvalidOperationException("""
                GetNextEventNode should only return one node, use GetNextEventNodes is multiple results are expected.
                """);
        }

        return nodes[0];
    }

    public IEnumerable<IEventNode> GetNodeChildren(INode node, int skipPinCount = 1)
    {
        if (node.Pins is null || node.Parent is not IDrawingNode drawing) {
            return [];
        }

        List<IEventNode> result = [];
        if (_connectorsByStartPin.TryGetValue(node.Pins[skipPinCount], out List<IConnector>? connectors)) {
            return connectors
                .Select(x => (x.End?.Parent as IEventNode)!)
                .Where(x => x is not null);
        }

        return [];
    }

    public short GetActorIndex(Actor actor)
    {
        int index = Actors.IndexOf(actor);
        if (index == -1) {
            index = Actors.Count;
            Actors.Add(actor);
        }

        return Convert.ToInt16(index);
    }

    public short GetActorActionIndex(Actor actor, string action)
    {
        int index = actor.Actions.IndexOf(action);
        if (index == -1) {
            index = actor.Actions.Count;
            actor.Actions.Add(action);
        }

        return Convert.ToInt16(index);
    }

    public short GetActorQueryIndex(Actor actor, string query)
    {
        int index = actor.Queries.IndexOf(query);
        if (index == -1) {
            index = actor.Queries.Count;
            actor.Queries.Add(query);
        }

        return Convert.ToInt16(index);
    }
}
