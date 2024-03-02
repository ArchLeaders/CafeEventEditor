using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Models;
using CafeEventEditor.Extensions;
using CafeEventEditor.ViewModels.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CafeEventEditor.Components;

public partial class FlowchartDrawingNode : ObservableDrawingNode, IFlowchartDrawingNode
{
    private readonly Flowchart _flowchart;
    private readonly Dictionary<Event, IEventNode> _drawnEvents = [];

    [ObservableProperty]
    private ObservableCollection<Actor> _actors;

    [ObservableProperty]
    private ObservableCollection<EntryPointNode> _entryPointNodes = [];

    public double CurrentForkOffset { get; set; } = -1;
    public double XOffset { get; set; } = 0;
    public double YOffset { get; set; } = 0;

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

        GenerateDrawing();
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
                eventNode.AppendCafeEvent(events, actors);
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

    private void GenerateDrawing()
    {
        ArgumentNullException.ThrowIfNull(Nodes, nameof(Nodes));

        foreach (var (name, entryPoint) in _flowchart.EntryPoints) {
            UseAbsoluteXOffset();

            EntryPointNode entryPointNode = new(name) {
                Parent = this,
                X = XOffset,
                Y = YOffset = 0
            };

            if (XOffset > 0) {
                MoveX(entryPointNode);
                entryPointNode.X = XOffset;
            }

            CurrentForkOffset = -1;

            Nodes.Add(entryPointNode);
            MoveY(entryPointNode);

            if (GetEvent(entryPoint.EventIndex) is Event cafeEvent) {
                AppendEvent([entryPointNode.GetLastPin()], cafeEvent);
            }
        }

        Width = YOffset;
        Height = YOffset;
    }

    public IEnumerable<INode> AppendEvent(IEnumerable<IPin> parents, Event cafeEvent, bool moveX = false)
    {
        ArgumentNullException.ThrowIfNull(Nodes, nameof(Nodes));
        ArgumentNullException.ThrowIfNull(Connectors, nameof(Connectors));

        if (_drawnEvents.TryGetValue(cafeEvent, out IEventNode? drawn)) {
            foreach (var parent in parents) {
                Connectors.Add(new ObservableConnector() {
                    Parent = this,
                    Start = parent,
                    End = drawn.GetFirstPin()
                });
            }

            // Assume that a fork will only have
            // new children so returning the end
            // of each branch is not needed here
            return [];
        }

        IEventNode result = _drawnEvents[cafeEvent] = cafeEvent.Type switch {
            EventType.Action => new ActionEventNode((ActionEvent)cafeEvent),
            EventType.Fork => new ForkEventNode((ForkEvent)cafeEvent),
            EventType.Join => new JoinEventNode((JoinEvent)cafeEvent),
            EventType.Subflow => new SubflowEventNode((SubflowEvent)cafeEvent),
            EventType.Switch => new SwitchEventNode((SwitchEvent)cafeEvent),
            _ => throw new InvalidOperationException($"""
                Invalid event type '{cafeEvent.Type}'
                """)
        };

        if (moveX) {
            MoveX(result);
        }

        result.Parent = this;
        result.X = XOffset;
        result.Y = YOffset;

        Nodes.Add(result);
        foreach (var parent in parents) {
            Connectors.Add(new ObservableConnector() {
                Parent = this,
                Start = parent,
                End = result.GetFirstPin()
            });
        }

        MoveY(result);

        return result.AppendRecursive(this, result, cafeEvent);
    }

    public Event? GetEvent(int index)
    {
        return index < _flowchart.Events.Count && index > -1 ? _flowchart.Events[index] : null;
    }

    public const double X_PADDING = 30.0;
    public const double Y_PADDING = 30.0;

    public void MoveX(INode node) => MoveX(node.Width);
    public void MoveX(double offset)
    {
        XOffset += offset + X_PADDING;
    }

    public void MoveY(INode node) => MoveY(node.Height);
    public void MoveY(double offset)
    {
        YOffset += offset + Y_PADDING;
    }

    public void UseAbsoluteXOffset()
    {
        XOffset = CurrentForkOffset > 0 ? CurrentForkOffset : XOffset;
    }
}
