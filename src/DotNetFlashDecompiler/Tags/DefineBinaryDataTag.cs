using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineBinaryDataTag(ushort Id, ReadOnlySequence<byte> Data) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineBinaryData;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadLittleEndian(out ushort id)) return false;
        if (!reader.TryReadLittleEndian(out uint reserved) && reserved != 0) return false;
        if (!reader.TryReadExact((int)reader.Remaining, out var data)) return false;

        value = new DefineBinaryDataTag(id, data);
        return true;
    }
}
