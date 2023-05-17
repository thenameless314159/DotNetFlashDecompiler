using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record ProductInfoTag(FlashEdition Edition, FlashProduct Product, byte MajorVersion, byte MinorVersion, 
    uint BuildLow, uint BuildHigh, DateTime CompilationDate) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.ProductInfo;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;
        if (!reader.TryReadLittleEndian(out uint product)) return false;
        if (!reader.TryReadLittleEndian(out uint edition)) return false;
        if (!reader.TryRead(out byte majorVersion)) return false;
        if (!reader.TryRead(out byte minorVersion)) return false;
        if (!reader.TryReadLittleEndian(out uint buildLow)) return false;
        if (!reader.TryReadLittleEndian(out uint buildHigh)) return false;
        if (!reader.TryReadLittleEndian(out ulong compilationDate)) return false;

        value = new ProductInfoTag((FlashEdition)edition, (FlashProduct)product, majorVersion, minorVersion, 
            buildLow, buildHigh, DateTime.UnixEpoch.AddMilliseconds(compilationDate));

        return true;
    }
}
