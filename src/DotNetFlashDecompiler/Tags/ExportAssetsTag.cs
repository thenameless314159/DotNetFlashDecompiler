using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record ExportAssetsTag : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.ExportAssets;

    public IDictionary<ushort, string> Exports { get; init; } = new Dictionary<ushort, string>();

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out ushort assetsCount)) return false;
        
        var assets = new Dictionary<ushort, string>(assetsCount);
        for (int i = 0; i < assetsCount; i++)
        {
            if (!reader.TryReadBigEndian(out ushort id)) return false;
            if (!reader.TryReadTo(out ReadOnlySequence<byte> nameSeq, 0)) return false;

            if (!assets.TryAdd(id, nameSeq.AsString())) return false;
        }

        value = new ExportAssetsTag { Exports = assets };
        return true;
    }
}
