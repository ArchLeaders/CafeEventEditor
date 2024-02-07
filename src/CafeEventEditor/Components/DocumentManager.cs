using CafeEventEditor.Components.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CafeEventEditor.Components;

public partial class DocumentManager : ObservableObject
{
    private static readonly Lazy<DocumentManager> _shared = new(() => new());
    public static DocumentManager Shared => _shared.Value;

    [ObservableProperty]
    private ObservableCollection<Document> _documents = [];

    [ObservableProperty]
    private Document? _current;
}
