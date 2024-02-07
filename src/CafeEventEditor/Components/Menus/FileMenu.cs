using CafeEventEditor.Builders.Attributes;

namespace CafeEventEditor.Components.Menus;

public class FileMenu
{
    [Menu("Exit", "File", "Ctrl + Q", "fa-solid fa-right-from-bracket")]
    public static async Task Exit()
    {
        await App.Exit();
    }
}
