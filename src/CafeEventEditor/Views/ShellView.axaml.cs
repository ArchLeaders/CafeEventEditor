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
            DocumentManager mgr = DocumentManager.Shared;

            int index = mgr.Documents.IndexOf(doc);
            mgr.Documents.RemoveAt(index);
            mgr.Current = null;

            if (mgr.Documents.Count == 0) {
                return;
            }

            mgr.Current = index == mgr.Documents.Count
                ? mgr.Documents[--index] : mgr.Documents[index];
        }
    }
}
