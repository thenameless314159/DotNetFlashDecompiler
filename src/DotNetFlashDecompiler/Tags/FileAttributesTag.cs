using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;

namespace DotNetFlashDecompiler.Tags;

public sealed record FileAttributesTag(FileAttributes Attributes) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.FileAttributes;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        if (reader.TryReadBigEndian(out int fileAttributes))
        {
            value = new FileAttributesTag((FileAttributes)fileAttributes);
            return true;
        }

        value = default;
        return false;
    }
}
