using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineBitsLosslessTag(ushort Id, ushort Width, ushort Height, byte ColorTableSize, 
    BitmapFormat Format, ReadOnlySequence<byte> CompressedData) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineBitsLossless;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadLittleEndian(out ushort id)) return false;
        if (!reader.TryRead(out byte formatByte)) return false;

        var format = formatByte switch
        {
            3 => BitmapFormat.ColorMap8,
            4 => BitmapFormat.Rgb15,
            5 => BitmapFormat.Rgb32,
            _ => throw new InvalidDataException("Invalid bitmap format.")
        };

        if (!reader.TryReadLittleEndian(out ushort width)) return false;
        if (!reader.TryReadLittleEndian(out ushort height)) return false;

        byte colorTableSize = 0;
        if (format == BitmapFormat.ColorMap8)
            reader.TryRead(out colorTableSize);

        if (!reader.TryReadExact((int)reader.Remaining, out var data)) return false;

        value = new DefineBitsLosslessTag(id, width, height, colorTableSize, format, data);
        return true;
    }
}
