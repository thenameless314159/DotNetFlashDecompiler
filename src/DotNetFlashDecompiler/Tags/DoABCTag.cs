using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DoABCTag(uint Flags, string Name, ReadOnlySequence<byte> Data) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DoABC;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out uint flags)) return false;
        if (!reader.TryReadTo(out ReadOnlySequence<byte> nameSeq, 0)) return false;
        if (!reader.TryReadExact((int)reader.Remaining, out var data)) return false;

        value = new DoABCTag(flags, nameSeq.AsString(), data);
        return true;
    }
}
