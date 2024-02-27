using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Modals;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public class ForkEventNode : ObservableNode, INodeTemplateProvider, IEventNode
{
    private const string DEFAULT_NAME = "Fork Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new ForkEventNode()) {
        Preview = new ForkEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public string Info => $"""
        Name: {Name ?? "~"}
        """;

    public ForkEventNode() : this(DEFAULT_NAME)
    {
    }

    public ForkEventNode(string name)
    {
        Name = name;
        ForkEventNodeView view = new() {
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
        ForkEvent forkEvent = new(Name ?? string.Empty);

        throw new NotImplementedException();
    }
}