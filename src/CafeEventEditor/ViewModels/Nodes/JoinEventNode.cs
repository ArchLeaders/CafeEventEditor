using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public class JoinEventNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Join Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new JoinEventNode()) {
        Preview = new JoinEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public JoinEventNode() : this(DEFAULT_NAME)
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

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height- view.Padding.Bottom, 10, 10, PinAlignment.Bottom, "Output");
    }
}