using CafeEventEditor.Core.Components;
using Revrs;
using Revrs.Extensions;
using SarcLibrary;
using System.Buffers;
using System.Data.Common;
using ZstdSharp;

namespace CafeEventEditor.Core.Helpers;

public static class ZstdHelper
{
    public const uint ZSTD_MAGIC = 0xFD2FB528;
    public const uint DICT_MAGIC = 0xEC30A437;

    private static readonly Compressor _defaultCompressor = new(15);
    private static readonly Decompressor _defaultDecompressor = new();
    private static readonly Dictionary<int, byte[]> _dicts = [];

    public static void LoadDictionaries(string path)
    {
        if (!File.Exists(path)) {
            return;
        }

        using FileStream fs = File.OpenRead(path);
        byte[] buffer = ArrayPool<byte>.Shared.Rent((int)fs.Length);
        int size = fs.Read(buffer);

        Span<byte> data = buffer.AsSpan()[..size];
        if (data.Read<uint>() == ZSTD_MAGIC) {
            data = _defaultDecompressor.Unwrap(data);
        }

        if (data.Read<uint>() == CafeLoadManager.SARC_MAGIC) {
            RevrsReader reader = new(data);
            ImmutableSarc sarc = new(ref reader);

            foreach ((var _, var dictData) in sarc) {
                TryLoadDict(dictData);
            }
        }
        else {
            TryLoadDict(data);
        }

        ArrayPool<byte>.Shared.Return(buffer);
    }

    public static Span<byte> Compress(this Span<byte> buffer, int dictionaryId = -1)
    {
        if (_dicts.TryGetValue(dictionaryId, out byte[]? dict)) {
            Compressor compressor = new();
            compressor.LoadDictionary(dict);
            return compressor.Wrap(buffer);
        }

        return _defaultCompressor.Wrap(buffer);
    }

    public static (bool isZsCompressed, int dictionaryId) TryDecompress(this Span<byte> buffer, out Span<byte> data)
    {
        if (buffer.Length < 5 || buffer.Read<uint>() != ZSTD_MAGIC) {
            data = buffer;
            return (false, -1);
        }

        int id = GetDictionaryId(buffer);
        if (_dicts.TryGetValue(id, out byte[]? dict)) {
            Decompressor decompressor = new();
            decompressor.LoadDictionary(dict);
            data = decompressor.Unwrap(buffer);
            return (true, id);
        }

        data = _defaultDecompressor.Unwrap(buffer);
        return (true, -1);
    }

    private static bool TryLoadDict(Span<byte> data)
    {
        if (data.Read<uint>() != DICT_MAGIC) {
            return false;
        }

        int id = data[4..8].Read<int>();
        _dicts[id] = data.ToArray();
        return true;
    }

    private static int GetDictionaryId(Span<byte> buffer)
    {
        byte descriptor = buffer[4];
        int windowDescriptorSize = ((descriptor & 0b00100000) >> 5) ^ 0b1;
        int dictionaryIdFlag = descriptor & 0b00000011;

        return dictionaryIdFlag switch {
            0x0 => -1,
            0x1 => buffer[5 + windowDescriptorSize],
            0x2 => buffer[(5 + windowDescriptorSize)..].Read<short>(),
            0x3 => buffer[(5 + windowDescriptorSize)..].Read<int>(),
            _ => throw new OverflowException("""
                Two bits cannot exceed 0x3, something terrible has happened!
                """)
        };
    }
}
