using Avalonia.Controls;
using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Components;
using CafeEventEditor.Core.Components;
using CafeEventEditor.Core.Converters;
using CafeEventEditor.Core.Models;
using CafeEventEditor.Extensions;
using CafeEventEditor.Views.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeEventEditor.ViewModels.Nodes;

public partial class SwitchEventNode : ObservableNode, INodeTemplateProvider, IEventNode, IParameterizedEvent
{
    private const string DEFAULT_NAME = "Switch Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new SwitchEventNode()) {
        Preview = new SwitchEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public ObservableCollection<Actor>? Actors {
        get {
            if (Parent is FlowchartDrawingNode drawing) {
                return drawing.Actors;
            }

            return [];
        }
    }

    public string Info => $"""
        Name: {Name}
        Actor: {Actor?.Name ?? "~"}
        Query: {Query}
        Parameters:
        - {Parameters.Replace("\r", string.Empty).Trim('\n').Replace("\n", "\n- ")}
        """;

    [ObservableProperty]
    private Actor? _actor;

    [ObservableProperty]
    private string? _query;

    [ObservableProperty]
    private string _parameters = string.Empty;

    public SwitchEventNode() : this(DEFAULT_NAME)
    {
    }

    public SwitchEventNode(SwitchEvent switchEvent) : this(switchEvent.Name)
    {
        Actor = switchEvent.Actor;
        Query = switchEvent.ActorQuery;
        Parameters = switchEvent.Parameters?.ToYaml() ?? string.Empty;
    }

    public SwitchEventNode(string name)
    {
        Name = name;
        SwitchEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = 280;
        Height = 180;

        this.AddPin(Width / 2, view.Padding.Top, 20, 20, PinAlignment.Top, "Input");

        PropertyChanged += (s, e) => {
            if (e.PropertyName is not nameof(Info)) {
                OnPropertyChanged(nameof(Info));
            }
        };
    }

    [RelayCommand]
    public void AddCase()
    {
        ArgumentNullException.ThrowIfNull(Pins);
        AddCase(Pins.Count - 2);
    }

    public void AddCase(int index)
    {
        double height = Height;
        if (Content is UserControl control) {
            height -= control.Padding.Bottom;
        }

        this.AddPin(GetNextPinOffset(), height, 20, 20, PinAlignment.Bottom, $"[{index}]");
    }

    private double GetNextPinOffset()
    {
        ArgumentNullException.ThrowIfNull(Pins);
        double offset = Pins.Count * 25;

        if (offset + 25 > Width) {
            Width += 25;
        }

        return offset;
    }

    public Event BuildRecursive(FlowchartBuilderContext context)
    {
        ArgumentNullException.ThrowIfNull(Pins, nameof(Pins));
        ArgumentNullException.ThrowIfNull(Actor, nameof(Actor));
        ArgumentNullException.ThrowIfNull(Query, nameof(Query));

        SwitchEvent switchEvent = new(Name ?? string.Empty) {
            ActorIndex = context.GetActorIndex(Actor),
            ActorQueryIndex = context.GetActorQueryIndex(Actor, Query),
            Parameters = Parameters.ParseCafeContainer()
        };

        int switchCaseCount = Pins.Count - 1;
        switchEvent.SwitchCases = new(switchCaseCount);

        for (int i = 0; i < switchCaseCount;) {
            switchEvent.SwitchCases.Add(
                new(i, (ushort)context.GetEventIndex(this, ++i))
            );
        }

        return switchEvent;
    }

    public IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent)
    {
        if (node is not SwitchEventNode switchEventNode) {
            throw new ArgumentException($"""
                Expected '{nameof(SwitchEventNode)}' but received '{node.GetType().Name}'
                """, nameof(node));
        }

        if (cafeEvent is not SwitchEvent switchEvent) {
            throw new ArgumentException($"""
                Expected '{nameof(SwitchEvent)}' but received '{cafeEvent.Type}Event'
                """, nameof(cafeEvent));
        }

        double initialYOffset = drawing.YOffset;

        List<INode> cases = [];
        foreach (var (caseIndex, caseEvent) in switchEvent.SwitchCases.Select(x => (x.Value, Event: drawing.GetEvent(x.EventIndex)!)).Where(x => x.Event is not null)) {
            switchEventNode.AddCase(caseIndex);

            drawing.YOffset = initialYOffset;
            cases.AddRange(
                drawing.AppendEvent([node.GetLastPin()], caseEvent, moveX: cases.Count > 0)
            );

            drawing.UseAbsoluteXOffset();
        }

        return cases;
    }
}
