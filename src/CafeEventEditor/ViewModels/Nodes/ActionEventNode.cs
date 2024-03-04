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
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CafeEventEditor.ViewModels.Nodes;

[DebuggerDisplay("{Name} (Action), Actor = {Actor?.Name}, Action = {Action}")]
public partial class ActionEventNode : ObservableNode, INodeTemplateProvider, IEventNode, IParameterizedEvent
{
    private const string DEFAULT_NAME = "Action Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new ActionEventNode(DEFAULT_NAME)) {
        Preview = new ActionEventNode(DEFAULT_NAME),
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
        Action: {Action}
        Parameters:
        - {Parameters.Replace("\r", string.Empty).Trim('\n').Replace("\n", "\n- ")}
        """;

    [ObservableProperty]
    private Actor? _actor;

    [ObservableProperty]
    private string? _action;

    [ObservableProperty]
    private string _parameters = string.Empty;

    [Obsolete("This ctor is only for the designer", error: true)]
    public ActionEventNode()
    {
    }

    public ActionEventNode(ActionEvent actionEvent) : this(actionEvent.Name)
    {
        Actor = actionEvent.Actor;
        Action = actionEvent.ActorAction;
        Parameters = actionEvent.Parameters?.ToYaml() ?? string.Empty;
    }

    public ActionEventNode(string name)
    {
        Name = name;
        ActionEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = 280;
        Height = 180;

        this.AddPin(Width / 2, view.Padding.Top, 20, 20, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height - view.Padding.Bottom, 20, 20, PinAlignment.Bottom, "Output");

        PropertyChanged += (s, e) => {
            if (e.PropertyName is not nameof(Info)) {
                OnPropertyChanged(nameof(Info));
            }
        };
    }

    public Event BuildRecursive(FlowchartBuilderContext context)
    {
        ArgumentNullException.ThrowIfNull(Actor, nameof(Actor));
        ArgumentNullException.ThrowIfNull(Action, nameof(Action));

        return new ActionEvent(Name ?? string.Empty) {
            ActorIndex = context.GetActorIndex(Actor),
            ActorActionIndex = context.GetActorActionIndex(Actor, Action),
            Parameters = string.IsNullOrEmpty(Parameters) ? [] : Parameters.ParseCafeContainer(),
            NextEventIndex = context.GetEventIndex(this)
        };
    }

    public IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent)
    {
        if (cafeEvent is ActionEvent actionEvent && actionEvent.NextEvent is Event next) {
            return drawing.AppendEvent([node.GetLastPin()], next);
        }

        // Always return a node because
        // forks needs an event to join
        return [node];
    }
}