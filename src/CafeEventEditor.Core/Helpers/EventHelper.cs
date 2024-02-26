using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;
using BfevLibrary.Core.Collections;
using CafeEventEditor.Core.Modals;

namespace CafeEventEditor.Core.Helpers;

public class EventHelper
{
    public EventList Events { get; } = [];
    public Dictionary<IEventNode, int> Indices { get; } = [];

    public short GetEventIndex(Event @event, INode? node)
    {
        if (node is not IEventNode eventNode) {
            return -1;
        }

        if (!Indices.TryGetValue(eventNode, out int index)) {
            Indices[eventNode] = index = Events.Count;
            Events.Add(@event);
        }

        return Convert.ToInt16(index);
    }
}
