using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASMetadata(int NameIndex) : AS3Item, IABCReadable<ASMetadata>
{
    public IList<ASItemInfo> Items { get; init; } = new List<ASItemInfo>();

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASMetadata? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var nameIndex)) return false;
        if (!reader.TryReadInt30(out var itemCount)) return false;

        var items = new List<ASItemInfo>(itemCount);
        for (int i = 0; i < itemCount; i++)
        {
            if (!ASItemInfo.TryRead(ref reader, abcFile, out var item))
                return false;

            items.Add(item);
        }

        value = new ASMetadata(nameIndex) { Items = items, ABCFile = abcFile };
        return true;
    }
}
