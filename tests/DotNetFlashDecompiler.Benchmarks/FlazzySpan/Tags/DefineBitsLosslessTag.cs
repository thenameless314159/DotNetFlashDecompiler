﻿using FlazzySpan.IO;

namespace FlazzySpan.Tags;

public class DefineBitsLosslessTag : ITagItem
{
    public TagKind Kind { get; }

    public ushort Id { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public byte ColorTableSize { get; }
    public BitmapFormat Format { get; set; }
    public byte[] CompressedData { get; set; }

    public int Version => Kind == TagKind.DefineBitsLossless ? 1 : 2;

    public DefineBitsLosslessTag(byte version)
    {
        Kind = version == 1 ? TagKind.DefineBitsLossless : TagKind.DefineBitsLossless2;
        CompressedData = Array.Empty<byte>();
    }
    public DefineBitsLosslessTag(ref FlashReader input, byte version)
        : this(version)
    {
        Id = input.ReadUInt16();
        Format = input.ReadByte() switch
        {
            3 => BitmapFormat.ColorMap8,
            4 when Version == 1 => BitmapFormat.Rgb15,
            5 => BitmapFormat.Rgb32,

            _ => throw new InvalidDataException("Invalid bitmap format.")
        };

        Width = input.ReadUInt16();
        Height = input.ReadUInt16();

        if (Format == BitmapFormat.ColorMap8)
            ColorTableSize = input.ReadByte();

        CompressedData = new byte[input.Length - input.Position];
        input.ReadBytes(CompressedData);
    }

    public int GetBodySize()
    {
        int size = 0;
        size += sizeof(ushort);
        size += sizeof(byte);
        size += sizeof(ushort);
        size += sizeof(ushort);
        if (Format == BitmapFormat.ColorMap8)
        {
            size += sizeof(byte);
        }
        size += CompressedData.Length;
        return size;
    }
    public void WriteBodyTo(ref FlashWriter output)
    {
        byte format = Format switch
        {
            BitmapFormat.ColorMap8 => 3,
            BitmapFormat.Rgb15 when Version == 1 => 4,
            BitmapFormat.Rgb32 => 5,

            BitmapFormat.Rgb15 when Version == 2 => throw new Exception($"{BitmapFormat.Rgb15} is only supported on {nameof(DefineBitsLosslessTag)} version 1."),
            _ => throw new InvalidDataException("Invalid bitmap format.")
        };

        output.Write(Id);
        output.Write(format);
        output.Write(Width);
        output.Write(Height);
        if (Format == BitmapFormat.ColorMap8)
        {
            output.Write(ColorTableSize);
        }
        output.Write(CompressedData);
    }
}
