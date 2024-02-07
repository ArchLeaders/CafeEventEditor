using CafeEventEditor.Components;
using CafeEventEditor.Components.Models;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace CafeEventEditor.Views;
public partial class ShellView : AppWindow
{
    public ShellView()
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

#pragma warning disable IDE0051 // Used by the XAML
    private static async void TabItemCloseRequested(TabViewItem _, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is Document doc && await doc.CloseRequested()) {
            DocumentManager.Shared.Documents.Remove(doc);
        }
    }
}
