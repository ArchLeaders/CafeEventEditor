using BfevLibrary;

namespace CafeEventEditor.Core.Components;

public delegate void CafeWriter(Span<byte> data);

public class CafeWriterHandle(BfevFile bfev, CafeWriter writer)
{
    public BfevFile Bfev { get; set; } = bfev;
    public CafeWriter Writer { get; set; } = writer;
}
