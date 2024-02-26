using Avalonia.NodeEditor.Core;

namespace CafeEventEditor.Core.Helpers;

public static class EventNodeHelper
{
    public static INode? GetNextNode(this INode node, int skipPinCount = 1)
        => GetNextNodes(node, skipPinCount).FirstOrDefault();
    public static IEnumerable<INode?> GetNextNodes(this INode node, int skipPinCount = 1)
    {
        if (node.Pins is null || node.Parent is not IDrawingNode drawing || drawing.Connectors is null) {
            return [];
        }

        List<INode> result = [];
        foreach (var pin in node.Pins.ToArray()[skipPinCount..]) {
            foreach (var connector in drawing.Connectors.Where(x => x.Start == pin)) {
                if (connector.End is INode target) {
                    result.Add(target);
                }
            }
        }

        return result;
    }
}
