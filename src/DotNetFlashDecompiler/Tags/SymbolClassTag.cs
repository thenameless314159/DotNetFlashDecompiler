using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record SymbolClassTag : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.SymbolClass;

    public IDictionary<ushort, string> Symbols { get; init; } = new Dictionary<ushort, string>();

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out ushort symbolsCount)) return false;

        var symbols = new Dictionary<ushort, string>(symbolsCount);
        for (int i = 0; i < symbolsCount; i++)
        {
            if (!reader.TryReadBigEndian(out ushort id)) return false;
            if (!reader.TryReadTo(out ReadOnlySequence<byte> nameSeq, 0)) return false;

            if (!symbols.TryAdd(id, nameSeq.AsString())) return false;
        }

        value = new SymbolClassTag { Symbols = symbols };
        return true;
    }
}
