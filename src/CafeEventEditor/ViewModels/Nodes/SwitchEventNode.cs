using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Components;
using CafeEventEditor.Core.Converters;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Modals;
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

    public SwitchEventNode(string name)
    {
        Name = name;
        SwitchEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = 280;
        Height = 180;

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");

        PropertyChanged += (s, e) => {
            if (e.PropertyName is not nameof(Info)) {
                OnPropertyChanged(nameof(Info));
            }
        };
    }

    [RelayCommand]
    private void AddCase()
    {
        ArgumentNullException.ThrowIfNull(Pins);
        this.AddPin(GetNextPinOffset(), Height, 10, 10, PinAlignment.Bottom, (Pins.Count - 2).ToString());
    }

    internal double GetNextPinOffset()
    {
        ArgumentNullException.ThrowIfNull(Pins);
        double offset = Pins.Count * 15;

        if (offset + 25 > Width) {
            Width += 15;
        }

        return offset;
    }

    public Event Append(EventHelper events, ActorHelper actors)
    {
        ArgumentNullException.ThrowIfNull(Actor, nameof(Actor));
        ArgumentNullException.ThrowIfNull(Query, nameof(Query));

        SwitchEvent switchEvent = new(Name ?? string.Empty) {
            ActorIndex = actors.GetActorIndex(Actor),
            ActorQueryIndex = ActorHelper.GetActorQueryIndex(Actor, Query),
            Parameters = Parameters.ParseCafeContainer()
        };

        // TODO: Set case indices
        return switchEvent;
    }
}
