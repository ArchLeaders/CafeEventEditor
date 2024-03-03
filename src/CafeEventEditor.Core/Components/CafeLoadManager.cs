using BfevLibrary;
using CafeEventEditor.Core.Helpers;
using CsYaz0;
using Revrs.Extensions;
using System.Buffers;

namespace CafeEventEditor.Core.Components;

public class CafeLoadManager
{
    public const uint YAZ0_MAGIC = 0x43524153;
    public const uint SARC_MAGIC = 0x43524153;

    public static CafeWriterHandle LoadFromFile(string file)
    {
        CafeWriter write = (_) => { };

        using FileStream fs = File.OpenRead(file);
        int size = (int)fs.Length;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(size);
        fs.Read(buffer, 0, size);

        (bool isZsCompressed, int dictionaryId) = ZstdHelper.TryDecompress(
            buffer.AsSpan()[..size], out Span<byte> data
        );

        if (isZsCompressed) {
            write += (data) => {
                data = ZstdHelper.Compress(data, dictionaryId);
            };
        }

        if (data.Read<uint>() == YAZ0_MAGIC) {
            data = Yaz0.Decompress(data);

            write += (data) => {
                data = Yaz0.Compress(data);
            };
        }

        if (data.Read<uint>() == SARC_MAGIC) {
            throw new NotImplementedException("SARC reading is not implemented");
        }

        write += (data) => {
            using FileStream fs = File.Create(file);
            fs.Write(data);
            return;
        };

        // TODO: Support FromBinary(span)
        BfevFile bfev = BfevFile.FromBinary(data.ToArray());

        ArrayPool<byte>.Shared.Return(buffer);
        return new(bfev, write);
    }
}
