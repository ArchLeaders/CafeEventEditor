using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Core.Helpers;
using CafeEventEditor.Core.Modals;
using CafeEventEditor.Views.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeEventEditor.ViewModels.Nodes;

public partial class SubflowEventNode : ObservableNode, INodeTemplateProvider, IEventNode, IParameterizedEvent
{
    private const string DEFAULT_NAME = "Subflow Event";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new SubflowEventNode()) {
        Preview = new SubflowEventNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public string Info => $"""
        Name: {Name}
        Flowchart Name: {FlowchartName}
        Entry Point Name: {EntryPointName}
        Parameters:
        - {Parameters.Replace("\r", string.Empty).Trim('\n').Replace("\n", "\n- ")}
        """;

    [ObservableProperty]
    private string _flowchartName = string.Empty;

    [ObservableProperty]
    private string _entryPointName = string.Empty;

    [ObservableProperty]
    private string _parameters = string.Empty;

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
        Width = 280;
        Height = 180;

        this.AddPin(Width / 2, view.Padding.Top, 10, 10, PinAlignment.Top, "Input");
        this.AddPin(Width / 2, Height- view.Padding.Bottom, 10, 10, PinAlignment.Bottom, "Output");

        PropertyChanged += (s, e) => {
            if (e.PropertyName is not nameof(Info)) {
                OnPropertyChanged(nameof(Info));
            }
        };
    }

    public Event Append(EventHelper events, ActorHelper actors)
    {
        throw new NotImplementedException();
    }
}