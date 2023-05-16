using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;

namespace DotNetFlashDecompiler.Tags;

public sealed record MetadataTag(string Metadata) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.Metadata;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        if (reader.TryReadTo(out ReadOnlySequence<byte> metaSeq, 0))
        {
            value = new FrameLabelTag(metaSeq.AsString());
            return true;
        }

        value = default;
        return false;
    }
}
