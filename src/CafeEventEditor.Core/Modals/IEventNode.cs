using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;
using System.ComponentModel;

namespace CafeEventEditor.Core.Modals;

public interface IEventNode
{
    Event Append(EventHelper events, ActorHelper actors);
    string Info { get; }
}
