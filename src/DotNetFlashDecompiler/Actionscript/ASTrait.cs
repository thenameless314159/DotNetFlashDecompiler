using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASTrait(
    int QNameIndex, 
    TraitKind Kind, 
    TraitAttribute Attributes, 
    int Id, 
    int TypeIndex = -1, 
    int MethodIndex = -1, 
    int FunctionIndex = -1, 
    int ClassIndex = -1,
    int ValueIndex = -1, 
    ConstantKind ValueKind = ConstantKind.Undefined) : AS3Item, IABCReadable<ASTrait>
{
    public IList<int> MetadataIndexes { get; private set; } = new List<int>();
    public bool IsStatic { get; internal set; }

    public ASMethod Method => ABCFile.Methods[MethodIndex];

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASTrait? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var qNameIndex)) return false;
        if (!reader.TryRead(out var flags)) return false;

        var kind = (TraitKind)(flags & 0x0F);
        var attributes = (TraitAttribute)(flags >> 4);

        if (!reader.TryReadInt30(out var id)) return false;

        switch (kind)
        {
            case TraitKind.Slot or TraitKind.Constant:
            {
                if (!reader.TryReadInt30(out var typeIndex)) return false;
                if (!reader.TryReadInt30(out var valueIndex)) return false;

                var valueKind = valueIndex > 0
                    ? reader.TryRead(out byte constantKind) ? (ConstantKind)constantKind : ConstantKind.Undefined
                    : ConstantKind.Undefined;

                value = new ASTrait(qNameIndex, kind, attributes, id, TypeIndex: typeIndex, ValueIndex: valueIndex, ValueKind:valueKind) { ABCFile = abcFile };
                break;
            }
            case TraitKind.Method or TraitKind.Getter or TraitKind.Setter:
            {
                if (!reader.TryReadInt30(out var methodIndex)) 
                    return false;

                value = new ASTrait(qNameIndex, kind, attributes, id, MethodIndex: methodIndex) { ABCFile = abcFile };
                value.Method.Trait = value;
                break;
            }
            case TraitKind.Class:
            {
                if (!reader.TryReadInt30(out var classIndex)) 
                    return false;

                value = new ASTrait(qNameIndex, kind, attributes, id, ClassIndex: classIndex) { ABCFile = abcFile };
                break;
            }
            case TraitKind.Function:
            {
                if (!reader.TryReadInt30(out var functionIndex)) 
                    return false;

                value = new ASTrait(qNameIndex, kind, attributes, id, FunctionIndex: functionIndex) { ABCFile = abcFile };
                break;
            }
            default:
                throw new NotSupportedException($"TraitKind {kind} is not supported.");
        }

        if ((attributes & TraitAttribute.Metadata) == 0) return true;

        if (!reader.TryReadInt30(out var metadataCount)) return false;
        var metadataIndexes = new List<int>(metadataCount);
        for (int i = 0; i < metadataCount; i++)
        {
            if (!reader.TryReadInt30(out var metadataIndex)) 
                return false;

            metadataIndexes.Add(metadataIndex);
        }

        value.MetadataIndexes = metadataIndexes;
        return true;
    }
}