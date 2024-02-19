using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public partial class EntryPointNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Action Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new EntryPointNode()) {
        Preview = new EntryPointNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public EntryPointNode() : this(DEFAULT_NAME)
    {   
    }

    public EntryPointNode(string name)
    {
        Name = name;
        EntryPointNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.DesiredSize.Width;
        Height = view.DesiredSize.Height;

        this.AddPin(Width / 2, 0, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height, 10, 10, PinAlignment.Bottom, "Output");
    }
}
