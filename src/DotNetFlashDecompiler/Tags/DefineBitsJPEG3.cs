using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineBitsJPEG3(ushort Id,  ReadOnlySequence<byte> Data, ImageFormat Format, ReadOnlySequence<byte> AlphaData)
    : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineBitsJPEG3;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadLittleEndian(out ushort id)) return false;
        if (!reader.TryReadLittleEndian(out int dataLength)) return false;
        if (!reader.TryReadExact(dataLength, out var data)) return false;
        if (!reader.TryReadImageFormat(out var format)) return false;

        if (!reader.TryReadExact(format == ImageFormat.JPEG ? (int)reader.Remaining : 8, out var alphaData))
            return false;

        value = new DefineBitsJPEG3(id, data, format, alphaData);
        return true;
    }
}
