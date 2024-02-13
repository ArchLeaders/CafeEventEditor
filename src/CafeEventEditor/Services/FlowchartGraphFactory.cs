using AvaloniaGraphControl;
using BfevLibrary.Core;

namespace CafeEventEditor.Services;

public class FlowchartGraphFactory
{
    private readonly Flowchart _flowchart;
    private readonly Graph _graph = new();
    private readonly Dictionary<Event, int> _edges = [];
    private Event? _tail;

    public FlowchartGraphFactory(Flowchart flowchart)
    {
        _flowchart = flowchart;
    }

    public Graph Build()
    {
        foreach ((var name, var entry) in _flowchart.EntryPoints) {
            if (GetEvent(entry.EventIndex) is Event tail) {
                _graph.Edges.Add(new(name, _tail = tail));
                AppendNextEvents();
            }
        }

        return _graph;
    }

    public void AppendNextEvents()
    {
        while (_tail is not null) {
            _tail = AppendNextEvent();
        }
    }

    public Event? AppendNextEvent()
    {
        if (_tail is null || _edges.ContainsKey(_tail)) {
            return null;
        }

        _edges.Add(_tail, _flowchart.Events.IndexOf(_tail));

        if (_tail is ActionEvent actionEvent) {
            return AppendActionEvent(actionEvent);
        }

        if (_tail is SwitchEvent switchEvent) {
            return AppendSwitchEvent(switchEvent);
        }

        if (_tail is SubflowEvent subflowEvent) {
            return AppendSubflowEvent(subflowEvent);
        }

        if (_tail is ForkEvent forkEvent) {
            return AppendForkEvent(forkEvent);
        }

        if (_tail is JoinEvent joinEvent) {
            return AppendJoinEvent(joinEvent);
        }

        return null;
    }

    private Event? AppendActionEvent(ActionEvent actionEvent)
    {
        if (actionEvent.NextEvent is Event next) {
            _graph.Edges.Add(new(actionEvent, next));
            return next;
        }

        return null;
    }

    private Event? AppendSwitchEvent(SwitchEvent switchEvent)
    {
        foreach (var switchCase in switchEvent.SwitchCases) {
            if (GetEvent(switchCase.EventIndex) is Event next) {
                _graph.Edges.Add(new(switchEvent, next, switchCase.Value));
                _tail = next;
                AppendNextEvents();
            }
        }

        return null;
    }

    private Event? AppendSubflowEvent(SubflowEvent subflowEvent)
    {
        if (subflowEvent.NextEvent is Event next) {
            _graph.Edges.Add(new(subflowEvent, next));
            return next;
        }

        return null;
    }

    private Event? AppendForkEvent(ForkEvent forkEvent)
    {
        

        return null;
    }

    private Event? AppendJoinEvent(JoinEvent joinEvent)
    {
        if (joinEvent.NextEvent is Event next) {
            _graph.Edges.Add(new(joinEvent, next));
            return next;
        }

        return null;
    }

    private Event? GetEvent(int index)
    {
        if (index >= 0 && _flowchart.Events.Count > index) {
            return _flowchart.Events[index];
        }

        return null;
    }
}
