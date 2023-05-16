﻿using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineBitsLossless2Tag(ushort Id, ushort Width, ushort Height, byte ColorTableSize, 
    BitmapFormat Format, ReadOnlySequence<byte> CompressedData) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineBitsLossless2;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out ushort id)) return false;
        if (!reader.TryRead(out byte formatByte)) return false;

        var format = formatByte switch
        {
            3 => BitmapFormat.ColorMap8,
            5 => BitmapFormat.Rgb32,
            _ => throw new InvalidDataException("Invalid bitmap format.")
        };

        if (!reader.TryReadBigEndian(out ushort width)) return false;
        if (!reader.TryReadBigEndian(out ushort height)) return false;

        byte colorTableSize = 0;
        if (format == BitmapFormat.ColorMap8)
            reader.TryRead(out colorTableSize);

        if (!reader.TryReadExact((int)reader.Remaining, out var data)) return false;

        value = new DefineBitsLosslessTag(id, width, height, colorTableSize, format, data);
        return true;
    }
}