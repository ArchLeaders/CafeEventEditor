using Avalonia.NodeEditor.Core;

namespace CafeEventEditor.Extensions;

public static class NodeExtension
{
    public static IPin GetFirstPin(this INode node)
    {
        ArgumentNullException.ThrowIfNull(node.Pins);
        return node.Pins[0];
    }

    public static IPin GetLastPin(this INode node)
    {
        ArgumentNullException.ThrowIfNull(node.Pins);
        return node.Pins[^1];
    }
}
