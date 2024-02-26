using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;

namespace CafeEventEditor.Core.Modals;

public interface IEventNode
{
    public Event Append(EventHelper events, ActorHelper actors);
}
