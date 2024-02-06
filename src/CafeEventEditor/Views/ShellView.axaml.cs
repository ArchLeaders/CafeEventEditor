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
}
