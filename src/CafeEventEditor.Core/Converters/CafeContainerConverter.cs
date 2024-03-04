using BfevLibrary.Core;
using System.Buffers;
using System.Text;
using VYaml.Emitter;
using VYaml.Parser;

namespace CafeEventEditor.Core.Converters;

public static class CafeContainerConverter
{
    private const string FLOAT_STRING_FORMAT = "0.0############";

    public static Container ParseCafeContainer(this string yml)
    {
        return FromYaml(yml);
    }

    public static Container FromYaml(string yml)
    {
        if (string.IsNullOrEmpty(yml)) {
            return [];
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(yml.Length * 2);
        int bytesWritten = Encoding.UTF8.GetBytes(yml, buffer);

        ReadOnlySequence<byte> sequence = new(buffer);
        YamlParser parser = new(sequence.Slice(0, bytesWritten));
        parser.SkipAfter(ParseEventType.DocumentStart);

        if (parser.CurrentEventType is not ParseEventType.MappingStart) {
            throw new InvalidDataException($"""
                Invalid parameters YAML:
                {yml}
                """);
        }

        parser.SkipAfter(ParseEventType.MappingStart);

        Container result = [];
        while (parser.TryReadScalarAsString(out string? key)) {
            ArgumentNullException.ThrowIfNull(key);

            if (parser.TryGetCurrentTag(out Tag tag)) {
                if (tag.Suffix.Equals("actor", StringComparison.InvariantCultureIgnoreCase)) {
                    parser.SkipAfter(ParseEventType.SequenceStart);
                    string? name = parser.ReadScalarAsString();
                    string? subName = parser.ReadScalarAsString();

                    result[key] = new() {
                        ActorIdentifier = new(name ?? string.Empty, subName ?? string.Empty)
                    };

                    parser.SkipAfter(ParseEventType.SequenceEnd);
                    continue;
                }

                if (tag.Suffix.Equals("a", StringComparison.InvariantCultureIgnoreCase) || tag.Suffix.Equals("arg", StringComparison.InvariantCultureIgnoreCase)) {
                    result[key] = new() {
                        Argument = parser.ReadScalarAsString()
                    };
                    continue;
                }

                if (tag.Suffix.Equals("w", StringComparison.InvariantCultureIgnoreCase)) {
                    result[key] = new() {
                        WString = parser.ReadScalarAsString()
                    };
                    continue;
                }
            }

            if (parser.TryReadScalarAsBool(out bool boolean)) {
                result[key] = new() {
                    Bool = boolean
                };
                continue;
            }

            if (parser.TryReadScalarAsInt32(out int s32)) {
                result[key] = new() {
                    Int = s32
                };
                continue;
            }

            if (parser.TryReadScalarAsFloat(out float f32)) {
                result[key] = new() {
                    Float = f32
                };
                continue;
            }

            if (parser.TryReadScalarAsString(out string? str)) {
                result[key] = new() {
                    String = str
                };
                continue;
            }

            if (parser.CurrentEventType is ParseEventType.SequenceStart) {
                parser.SkipAfter(ParseEventType.SequenceStart);

                ContainerItem item = new();
                while (parser.CurrentEventType is not ParseEventType.SequenceEnd) {
                    if (parser.TryReadScalarAsBool(out boolean)) {
                        item.BoolArray = [.. item.BoolArray ?? [], boolean];
                        continue;
                    }

                    if (parser.TryReadScalarAsInt32(out s32)) {
                        item.IntArray = [.. item.IntArray ?? [], s32];
                        continue;
                    }

                    if (parser.TryReadScalarAsFloat(out f32)) {
                        item.FloatArray = [.. item.FloatArray ?? [], f32];
                        continue;
                    }

                    if (parser.TryReadScalarAsString(out str)) {
                        if (tag is not null && tag.Suffix.Equals("w", StringComparison.InvariantCultureIgnoreCase)) {
                            item.WStringArray = [.. item.WStringArray ?? [], str ?? string.Empty];
                        }
                        else {
                            item.StringArray = [.. item.StringArray ?? [], str ?? string.Empty];
                        }

                        continue;
                    }

                    throw new InvalidOperationException($"""
                        Could not parse scalar ({parser.CurrentMark}, {Enum.GetName(parser.CurrentEventType)})
                        """);
                }

                bool[] filledArrays = [
                    item.BoolArray?.Length > 0,
                    item.IntArray?.Length > 0,
                    item.FloatArray?.Length > 0,
                    item.StringArray?.Length > 0,
                    item.WStringArray?.Length > 0
                ];

                if (filledArrays.Where(x => x).Count() > 1) {
                    throw new InvalidOperationException($"""
                        Arrays can only contain a single scalar type ({parser.CurrentMark})
                        """);
                }

                result[key] = item;
                parser.SkipAfter(ParseEventType.SequenceEnd);
            }
        }

        return result;
    }

    public static string ToYaml(this Container container)
    {
        ArrayBufferWriter<byte> buffer = new();
        Utf8YamlEmitter emitter = new(buffer);
        emitter.BeginMapping(MappingStyle.Block);

        // Used for emitting formatted floats
        byte[] f32RawBuffer = ArrayPool<byte>.Shared.Rent(12);
        Span<byte> f32RawValue = f32RawBuffer.AsSpan()[..12];

        foreach ((string key, ContainerItem item) in container) {
            emitter.WriteString(key);

            if (item.Int is int s32) {
                emitter.WriteInt32(s32);
                continue;
            }

            if (item.IntArray is int[] s32Array) {
                emitter.BeginSequence(SequenceStyle.Flow);
                foreach (int value in s32Array) {
                    emitter.WriteInt32(value);
                }

                emitter.EndSequence();
                continue;
            }

            if (item.Bool is bool boolean) {
                emitter.WriteBool(boolean);
                continue;
            }

            if (item.BoolArray is bool[] booleanArray) {
                emitter.BeginSequence(SequenceStyle.Flow);
                foreach (bool value in booleanArray) {
                    emitter.WriteBool(value);
                }

                emitter.EndSequence();
                continue;
            }

            if (item.Float is float f32) {
                int bytesWritten = Encoding.UTF8.GetBytes(f32.ToString(FLOAT_STRING_FORMAT), f32RawValue);
                emitter.WriteScalar(f32RawValue[..bytesWritten]);
                continue;
            }

            if (item.FloatArray is float[] f32Array) {
                emitter.BeginSequence(SequenceStyle.Flow);
                foreach (float value in f32Array) {
                    int bytesWritten = Encoding.UTF8.GetBytes(value.ToString(FLOAT_STRING_FORMAT), f32RawValue);
                    emitter.WriteScalar(f32RawValue[..bytesWritten]);
                }

                emitter.EndSequence();
                continue;
            }

            if (item.String is string str) {
                emitter.WriteString(str);
                continue;
            }

            if (item.StringArray is string[] strArray) {
                emitter.BeginSequence(SequenceStyle.Flow);
                foreach (string value in strArray) {
                    emitter.WriteString(value);
                }

                emitter.EndSequence();
                continue;
            }

            if (item.WString is string wstr) {
                emitter.Tag("!w");
                emitter.WriteString(wstr);
                continue;
            }

            if (item.WStringArray is string[] wstrArray) {
                emitter.BeginSequence(SequenceStyle.Flow);
                foreach (string value in wstrArray) {
                    emitter.Tag("!w");
                    emitter.WriteString(value);
                }

                emitter.EndSequence();
                continue;
            }

            if (item.Argument is string arg) {
                emitter.Tag("!arg");
                emitter.WriteString(arg);
                continue;
            }

            if (item.ActorIdentifier is (string name, string subName)) {
                emitter.Tag("!actor");
                emitter.BeginSequence(SequenceStyle.Flow);
                emitter.WriteString(name);
                emitter.WriteString(subName);
                emitter.EndSequence();
                continue;
            }
        }

        emitter.EndMapping();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }
}
