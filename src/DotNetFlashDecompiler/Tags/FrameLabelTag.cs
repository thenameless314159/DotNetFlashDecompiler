using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record FrameLabelTag(string Name) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.FrameLabel;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        if (reader.TryReadTo(out ReadOnlySequence<byte> nameSeq, 0))
        {
            value = new FrameLabelTag(nameSeq.AsString());
            return true;
        }

        value = default;
        return false;
    }
}
