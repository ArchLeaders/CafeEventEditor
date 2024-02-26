using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public class SwitchEventNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Switch Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new SwitchEventNode()) {
        Preview = new SwitchEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

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
        Width = 200;
        Height = 120;

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");
    }

    public double GetNextPinOffset()
    {
        ArgumentNullException.ThrowIfNull(Pins);
        double offset = Pins.Count * 15;

        if (offset + 25 > Width) {
            Width += 15;
        }

        return offset;
    }
}
