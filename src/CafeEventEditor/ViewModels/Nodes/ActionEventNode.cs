using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Converters;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Modals;
using CafeEventEditor.Views.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeEventEditor.ViewModels.Nodes;

public partial class ActionEventNode : ObservableNode, INodeTemplateProvider, IEventNode
{
    private const string DEFAULT_NAME = "Action Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new ActionEventNode(DEFAULT_NAME)) {
        Preview = new ActionEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

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

    public ActionEventNode(string name)
    {
        Name = name;
        ActionEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height- view.Padding.Bottom, 10, 10, PinAlignment.Bottom, "Output");
    }

    public Event Append(EventHelper events, ActorHelper actors)
    {
        ArgumentNullException.ThrowIfNull(Actor, nameof(Actor));
        ArgumentNullException.ThrowIfNull(Action, nameof(Action));

        ActionEvent actionEvent = new(Name ?? string.Empty) {
            ActorIndex = actors.GetActorIndex(Actor),
            ActorActionIndex = ActorHelper.GetActorActionIndex(Actor, Action),
            Parameters = string.IsNullOrEmpty(Parameters) ? [] : Parameters.ParseCafeContainer()
        };

        actionEvent.NextEventIndex = events.GetEventIndex(actionEvent, this.GetNextNode());
        return actionEvent;
    }

    public override string ToString()
    {
        return $"""
            Name: {Name}
            Actor: {Actor?.Name ?? "~"}
            Action: {Action}
            Parameters:
            - {Parameters.Replace("\r", string.Empty).Trim('\n').Replace("\n", "\n- ")}
            """;
    }
}