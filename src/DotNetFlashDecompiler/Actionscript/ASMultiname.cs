using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASMultiname(MultinameKind Kind, int NameIndex, int QNameIndex, int NamespaceIndex, int NamespaceSetIndex) : AS3Item, IABCReadable<ASMultiname>
{
    public IList<int> TypeIndexes { get; init; } = new List<int>();

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASMultiname? value)
    {
        value = default;
        if (!reader.TryRead(out byte multinameKind)) return false;
        var kind = (MultinameKind)multinameKind;

        value = kind switch {
            MultinameKind.QName or
            MultinameKind.QNameA => ReadQName(ref reader, kind),
            MultinameKind.TypeName => ReadTypes(ref reader, kind),
            MultinameKind.RTQName or
            MultinameKind.RTQNameA => ReadRTQName(ref reader, kind),
            MultinameKind.RTQNameL or
            MultinameKind.RTQNameLA => ReadRTQNameL(ref reader, kind),
            MultinameKind.Multiname or
            MultinameKind.MultinameA => ReadMultiname(ref reader, kind),
            MultinameKind.MultinameL or
            MultinameKind.MultinameLA => ReadMultinameL(ref reader, kind),
            _ => null,
        };

        if (value == null) 
            return false;

        value.ABCFile = abcFile;
        return true;
    }

    static ASMultiname? ReadQName(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        if (!reader.TryReadInt30(out var nameIndex)) return null;
        if (!reader.TryReadInt30(out var namespaceIndex)) return null;
        return new ASMultiname(kind, nameIndex, -1, namespaceIndex, -1);
    }

    static ASMultiname? ReadTypes(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        if (!reader.TryReadInt30(out var qNameIndex)) return null;
        if (!reader.TryReadInt30(out var length)) return null;
        var typeIndexes = new List<int>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadInt30(out var index))
                return null;

            typeIndexes.Add(index);
        }

        return new ASMultiname(kind, -1, qNameIndex, -1, -1) { TypeIndexes = typeIndexes };
    }

    static ASMultiname? ReadRTQName(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        if (!reader.TryReadInt30(out var nameIndex)) return null;
        return new ASMultiname(kind, nameIndex, -1, -1, -1);
    }

    static ASMultiname ReadRTQNameL(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        // No data
        return new ASMultiname(kind, -1, -1, -1, -1);
    }

    static ASMultiname? ReadMultiname(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        if (!reader.TryReadInt30(out var nameIndex)) return null;
        if (!reader.TryReadInt30(out var namespaceSetIndex)) return null;
        return new ASMultiname(kind, nameIndex, -1, -1, namespaceSetIndex);
    }

    static ASMultiname? ReadMultinameL(ref SequenceReader<byte> reader, MultinameKind kind)
    {
        if (!reader.TryReadInt30(out var namespaceSetIndex)) return null;
        return new ASMultiname(kind, -1, -1, -1, namespaceSetIndex);
    }
}