using Avalonia.Controls;
using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Converters;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Modals;
using CafeEventEditor.ViewModels.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CafeEventEditor.Components;

public partial class FlowchartDrawingNode : ObservableDrawingNode
{
    private const double PADDING_X = 30.0;
    private const double PADDING_Y = 30.0;

    private readonly Flowchart _flowchart;
    private readonly Dictionary<Event, INode> _cache = [];
    private double _xOffset = 0;
    private double _yOffset = 0;

    [ObservableProperty]
    private ObservableCollection<Actor> _actors;

    [ObservableProperty]
    private ObservableCollection<EntryPointNode> _entryPointNodes = [];

    public FlowchartDrawingNode(Flowchart flowchart)
    {
        _flowchart = flowchart;

        Actors = flowchart.Actors;
        Name = flowchart.Name;
        Height = 5000;
        Width = 5000;
        EnableSnap = true;
        SnapX = 10;
        SnapY = 10;

        GenerateNodes();
    }

    public Flowchart ToFlowchart()
    {
        ArgumentNullException.ThrowIfNull(Name);
        ArgumentNullException.ThrowIfNull(Nodes);
        ArgumentNullException.ThrowIfNull(Connectors);

        EventHelper events = new();
        ActorHelper actors = new();
        Flowchart flowchart = new(Name);

        foreach (var node in Nodes) {
            if (node is IEventNode eventNode) {
                eventNode.Append(events, actors);
                continue;
            }
            
            if (node is EntryPointNode entryPoint) {
                ArgumentNullException.ThrowIfNull(entryPoint.Name);
                flowchart.EntryPoints.Add(entryPoint.Name, entryPoint.GetEntryPoint());
            }
        }

        return flowchart;
    }

    public override bool CanConnectPin(IPin pin)
    {
        if (_editor.IsConnectorMoving()) {
            return pin.Name is "Input";
        }

        if (pin.Parent is ForkEventNode) {
            return true;
        }

        return base.CanConnectPin(pin);
    }

    private void GenerateNodes()
    {
        ArgumentNullException.ThrowIfNull(Nodes, nameof(Nodes));

        foreach (var (name, entryPoint) in _flowchart.EntryPoints) {
            EntryPointNode entryPointNode = new(name) {
                Parent = this,
                X = _xOffset,
                Y = _yOffset = 0
            };

            Nodes.Add(entryPointNode);
            EntryPointNodes.Add(entryPointNode);
            _yOffset += entryPointNode.Height + PADDING_Y;

            if (GetNextEvent(entryPoint.EventIndex) is Event cafeEvent) {
                AppendEvent(GetLastPin(entryPointNode), cafeEvent);
            }

            _xOffset += PADDING_X + entryPointNode.Width;
        }
    }

    private INode AppendEvent(IPin parent, Event cafeEvent)
    {
        if (_cache.TryGetValue(cafeEvent, out INode? cache)) {
            Connectors?.Add(new ObservableConnector() {
                Parent = this,
                Start = parent,
                End = GetFirstPin(cache)
            });

            return cache;
        }

        INode result = cafeEvent.Type switch {
            EventType.Action => AppendActionEvent(parent, (ActionEvent)cafeEvent),
            EventType.Fork => AppendForkEvent(parent, (ForkEvent)cafeEvent),
            EventType.Join => AppendJoinEvent(parent, (JoinEvent)cafeEvent),
            EventType.Subflow => AppendSubflowEvent(parent, (SubflowEvent)cafeEvent),
            EventType.Switch => AppendSwitchEvent(parent, (SwitchEvent)cafeEvent),
            _ => throw new InvalidOperationException($"""
                Invalid event type '{cafeEvent.Type}'
                """)
        };

        return _cache[cafeEvent] = result;
    }

    private ActionEventNode AppendActionEvent(IPin parent, ActionEvent actionEvent)
    {
        ActionEventNode node = new(actionEvent.Name) {
            Actor = actionEvent.Actor,
            Action = actionEvent.ActorAction,
            Parameters = actionEvent.Parameters?.ToYaml() ?? string.Empty,
            Parent = this,
            X = _xOffset,
            Y = _yOffset
        };

        Nodes?.Add(node);
        Connectors?.Add(new ObservableConnector() {
            Parent = this,
            Start = parent,
            End = GetFirstPin(node)
        });

        if (actionEvent.NextEvent is Event nextEvent) {
            _yOffset += node.Height + PADDING_Y;
            AppendEvent(GetLastPin(node), nextEvent);
        }

        return node;
    }

    private ForkEventNode AppendForkEvent(IPin parent, ForkEvent forkEvent)
    {
        ForkEventNode node = new(forkEvent.Name) {
            Parent = this,
            X = _xOffset,
            Y = _yOffset
        };

        Nodes?.Add(node);
        Connectors?.Add(new ObservableConnector() {
            Parent = this,
            Start = parent,
            End = GetFirstPin(node)
        });

        return node;
    }

    private JoinEventNode AppendJoinEvent(IPin parent, JoinEvent joinEvent)
    {
        JoinEventNode node = new(joinEvent.Name) {
            Parent = this,
            X = _xOffset,
            Y = _yOffset
        };

        Nodes?.Add(node);
        Connectors?.Add(new ObservableConnector() {
            Parent = this,
            Start = parent,
            End = GetFirstPin(node)
        });

        if (joinEvent.NextEvent is Event nextEvent) {
            _yOffset += node.Height + PADDING_Y;
            AppendEvent(GetLastPin(node), nextEvent);
        }

        return node;
    }

    private SubflowEventNode AppendSubflowEvent(IPin parent, SubflowEvent subflowEvent)
    {
        SubflowEventNode node = new(subflowEvent.Name) {
            Parent = this,
            X = _xOffset,
            Y = _yOffset
        };

        Nodes?.Add(node);
        Connectors?.Add(new ObservableConnector() {
            Parent = this,
            Start = parent,
            End = GetFirstPin(node)
        });

        if (subflowEvent.NextEvent is Event nextEvent) {
            _yOffset += node.Height + PADDING_Y;
            AppendEvent(GetLastPin(node), nextEvent);
        }

        return node;
    }

    private SwitchEventNode AppendSwitchEvent(IPin parent, SwitchEvent switchEvent)
    {
        SwitchEventNode node = new(switchEvent.Name) {
            Parent = this,
            X = _xOffset,
            Y = _yOffset
        };

        Nodes?.Add(node);
        Connectors?.Add(new ObservableConnector() {
            Parent = this,
            Start = parent,
            End = GetFirstPin(node)
        });

        double height = node.Height;
        if (node.Content is UserControl view) {
            height -= view.Padding.Bottom;
        }

        // Set the cache early so
        // that the cases find it
        _cache[switchEvent] = node;

        INode? last = null;
        double yOffset = _yOffset += node.Height + PADDING_Y + 50;
        foreach (var switchCase in switchEvent.SwitchCases) {
            if (GetNextEvent(switchCase.EventIndex) is Event cafeEvent) {
                _yOffset = yOffset;
                node.AddPin(node.GetNextPinOffset(), height, 10, 10, PinAlignment.Bottom, switchCase.Value.ToString());

                if (_cache.TryGetValue(cafeEvent, out INode? existing)) {
                    Connectors?.Add(new ObservableConnector() {
                        Parent = this,
                        Start = GetLastPin(node),
                        End = GetFirstPin(existing)
                    });
                    continue;
                }

                if (last is not null) {
                    _xOffset += last.Width + PADDING_X;
                }

                last = AppendEvent(GetLastPin(node), cafeEvent);
            }
        }

        return node;
    }

    private static IPin GetFirstPin(INode node)
    {
        ArgumentNullException.ThrowIfNull(node.Pins);
        return node.Pins[0];
    }

    private static IPin GetLastPin(INode node)
    {
        ArgumentNullException.ThrowIfNull(node.Pins);
        return node.Pins[^1];
    }

    private Event? GetNextEvent(int index)
    {
        if (index > -1 && index < _flowchart.Events.Count) {
            return _flowchart.Events[index];
        }

        return null;
    }
}
