using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineFontNameTag(ushort Id, string Name, string Copyright) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineFontName;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out ushort id)) return false;
        if (!reader.TryReadTo(out ReadOnlySequence<byte> nameSeq, 0)) return false;
        if (!reader.TryReadTo(out ReadOnlySequence<byte> copyrightSeq, 0)) return false;

        value = new DefineFontNameTag(id, nameSeq.AsString(), copyrightSeq.AsString());
        return true;
    }
}
