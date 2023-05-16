using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;

namespace DotNetFlashDecompiler.Tags;

public abstract record TagItem : ITagItem, IBufferReadable<TagItem>
{
    public abstract TagKind Kind { get; }

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;
        if (!reader.TryReadBigEndian(out ushort tagCode))
            return false;

        var (tagKind, length) = ((TagKind)(tagCode >> 6), tagCode & 63);
        if (length > 62 && !reader.TryReadBigEndian(out length))
            return false;

        if (reader.TryReadExact(length, out var tagData))
            return false;

        var tagReader = new SequenceReader<byte>(tagData);

        return tagKind switch
        {
            TagKind.DefineBinaryData => DefineBinaryDataTag.TryRead(ref tagReader, out value),
            TagKind.DefineBitsJPEG3 => DefineBitsJPEG3.TryRead(ref tagReader, out value),
            TagKind.DefineBitsLossless => DefineBitsLosslessTag.TryRead(ref tagReader, out value),
            TagKind.DefineBitsLossless2 => DefineBitsLossless2Tag.TryRead(ref tagReader, out value),
            TagKind.DefineFontName => DefineFontNameTag.TryRead(ref tagReader, out value),
            TagKind.DefineSound => DefineSoundTag.TryRead(ref tagReader, out value),
            TagKind.DoABC => DoABCTag.TryRead(ref tagReader, out value),
            TagKind.ExportAssets => ExportAssetsTag.TryRead(ref tagReader, out value),
            TagKind.FileAttributes => FileAttributesTag.TryRead(ref tagReader, out value),
            TagKind.FrameLabel => FrameLabelTag.TryRead(ref tagReader, out value),
            TagKind.Metadata => MetadataTag.TryRead(ref tagReader, out value),
            TagKind.ProductInfo => ProductInfoTag.TryRead(ref tagReader, out value),
            TagKind.ScriptLimits => ScriptLimitsTag.TryRead(ref tagReader, out value),
            TagKind.SetBackgroundColor => SetBackgroundColorTag.TryRead(ref tagReader, out value),
            TagKind.SymbolClass => SymbolClassTag.TryRead(ref tagReader, out value),
            _ => GetDefaultTagItem(tagKind, out value)
        };
    }

    static bool GetDefaultTagItem(TagKind kind, out TagItem? tagItem)
    {
        tagItem = new DefaultTagItem(kind);
        return true;
    }
}