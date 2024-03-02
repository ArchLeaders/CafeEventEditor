using Avalonia.NodeEditor.Core;
using BfevLibrary.Core;

namespace CafeEventEditor.Core.Models;

public interface IFlowchartDrawingNode : IDrawingNode
{
    public double CurrentForkOffset { get; set; }
    public double XOffset { get; set; }
    public double YOffset { get; set; }
    public IEnumerable<INode> AppendEvent(IEnumerable<IPin> parents, Event cafeEvent, bool moveX = false);
    public Event? GetEvent(int index);
    public void MoveX(INode node);
    public void MoveX(double offset);
    public void MoveY(INode node);
    public void MoveY(double offset);
    public void UseAbsoluteXOffset();
}
