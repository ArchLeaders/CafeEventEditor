using Avalonia.NodeEditor.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

namespace CafeEventEditor.Components.Models;

public partial class Document : ObservableNodeEditor
{
    [ObservableProperty]
    private string _header;

    [ObservableProperty]
    private IconSource _icon;

    [ObservableProperty]
    private object? _content;

    public virtual Task<bool> CloseRequested()
    {
        return Task.FromResult(true);
    }

    public Document(string header, Symbol icon = Symbol.Document)
    {
        _header = header;
        _icon = new SymbolIconSource {
            Symbol = icon
        };
    }
}
