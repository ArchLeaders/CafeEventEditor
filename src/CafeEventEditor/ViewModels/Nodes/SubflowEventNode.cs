using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public class SubflowEventNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Subflow Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new SubflowEventNode()) {
        Preview = new SubflowEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public SubflowEventNode() : this(DEFAULT_NAME)
    {
    }

    public SubflowEventNode(string name)
    {
        Name = name;
        SubflowEventNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height- view.Padding.Bottom, 10, 10, PinAlignment.Bottom, "Output");
    }
}