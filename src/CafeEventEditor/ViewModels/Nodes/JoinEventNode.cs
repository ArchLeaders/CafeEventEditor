using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Components;
using CafeEventEditor.Core.Models;
using CafeEventEditor.Extensions;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public class JoinEventNode : ObservableNode, INodeTemplateProvider, IJoinEventNode, IEventNode
{
    private const string DEFAULT_NAME = "Join Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new JoinEventNode()) {
        Preview = new JoinEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public string Info => $"""
        Name: {Name ?? "~"}
        """;

    public JoinEventNode() : this(DEFAULT_NAME)
    {
    }

    public JoinEventNode(JoinEvent joinEvent) : this(joinEvent.Name)
    {

    }

    public JoinEventNode(string name)
    {
        Name = name;
        JoinEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, view.Padding.Top, 20, 20, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height - view.Padding.Bottom, 20, 20, PinAlignment.Bottom, "Output");
    }

    public Event BuildRecursive(FlowchartBuilderContext context)
    {
        JoinEvent joinEvent = new(Name ?? string.Empty);

        ForkEvent forkEvent = context.ForkEvents.Pop();
        forkEvent.JoinEventIndex = context.GetEventIndex(joinEvent, this);

        joinEvent.NextEventIndex = context.GetEventIndex(this);
        return joinEvent;
    }

    public IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent)
    {
        if (cafeEvent is JoinEvent joinEvent && joinEvent.NextEvent is Event next) {
            return drawing.AppendEvent([node.GetLastPin()], next);
        }

        // Always return a node because
        // forks needs an event to join
        return [node];
    }
}