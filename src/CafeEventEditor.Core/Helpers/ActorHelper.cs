using BfevLibrary.Core;
using BfevLibrary.Core.Collections;

namespace CafeEventEditor.Core.Helpers;

public class ActorHelper
{
    public ActorList Actors { get; } = [];

    public short GetActorIndex(Actor actor)
    {
        int index = Actors.IndexOf(actor);
        if (index == -1) {
            index = Actors.Count;
            Actors.Add(actor);
        }

        return Convert.ToInt16(index);
    }

    public static short GetActorActionIndex(Actor actor, string action)
    {
        int index = actor.Actions.IndexOf(action);
        if (index == -1) {
            index = actor.Actions.Count;
            actor.Actions.Add(action);
        }

        return Convert.ToInt16(index);
    }

    public static short GetActorQueryIndex(Actor actor, string query)
    {
        int index = actor.Queries.IndexOf(query);
        if (index == -1) {
            index = actor.Queries.Count;
            actor.Queries.Add(query);
        }

        return Convert.ToInt16(index);
    }
}
