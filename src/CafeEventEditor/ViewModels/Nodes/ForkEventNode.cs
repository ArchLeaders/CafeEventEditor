using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Components;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Models;
using CafeEventEditor.Extensions;
using CafeEventEditor.Views.Nodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public ForkEventNode(ForkEvent forkEvent) : this(forkEvent.Name)
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

        this.AddPin(Width / 2, view.Padding.Top, 20, 20, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height- view.Padding.Bottom, 20, 20, PinAlignment.Bottom, "Output");
    }

    public Event AppendCafeEvent(EventHelper events, ActorHelper actors)
    {
        ForkEvent forkEvent = new(Name ?? string.Empty);

        throw new NotImplementedException();
    }

    public IEnumerable<INode> AppendRecursive(IFlowchartDrawingNode drawing, INode node, Event cafeEvent)
    {
        if (cafeEvent is not ForkEvent forkEvent) {
            throw new ArgumentException($"""
                Expected '{nameof(ForkEvent)}' but received '{cafeEvent.Type}Event'
                """, nameof(cafeEvent));
        }

        if (drawing.GetEvent(forkEvent.JoinEventIndex) is not JoinEvent joinEvent) {
            throw new InvalidDataException($"""
                Invalid {nameof(ForkEvent)}: The connected {nameof(JoinEvent)} is null.
                """);
        }

        double initialXOffset = drawing.XOffset;
        double initialYOffset = drawing.YOffset;

        // Recursively create branches
        List<INode> branches = [];
        foreach (Event next in forkEvent.ForkEventIndicies.Select(x => drawing.GetEvent(x)!).Where(x => x is not null)) {
            drawing.YOffset = initialYOffset;
            branches.AddRange(
                drawing.AppendEvent([node.GetLastPin()], next, moveX: branches.Count > 0)
            );
        }

        // Reset X offsets
        if (drawing.CurrentForkOffset < drawing.XOffset) {
            drawing.CurrentForkOffset = drawing.XOffset;
        }

        drawing.XOffset = initialXOffset;
        drawing.YOffset = branches.Max(x => x.Y + x.Height) + FlowchartDrawingNode.Y_PADDING;

        return drawing.AppendEvent(branches.Select(x => x.GetLastPin()), joinEvent);
    }
}