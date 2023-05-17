using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASNamespaceSet : AS3Item, IABCReadable<ASNamespaceSet>
{
    public IList<int> NamespaceIndexes { get; init; } = new List<int>();
    
    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASNamespaceSet? value)
    {
        value = default;

        if (!reader.TryReadInt30(out var length)) return false;

        var indexes = new List<int>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadInt30(out var index)) 
                return false;

            indexes.Add(index);
        }

        value = new ASNamespaceSet { NamespaceIndexes = indexes, ABCFile = abcFile};
        return true;
    }
}