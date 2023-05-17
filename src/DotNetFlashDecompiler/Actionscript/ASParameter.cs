using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASParameter(int TypeIndex) : AS3Item, IABCReadable<ASParameter>
{
    public int NameIndex { get; set; } = -1;
    public int ValueIndex { get; set; } = -1;
    public bool IsOptional { get; set; }
    public ConstantKind ValueKind { get; set; } = ConstantKind.Undefined;

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASParameter? value)
    {
        if (!reader.TryReadInt30(out var typeIndex))
        {
            value = default;
            return false;
        }

        value = new ASParameter(typeIndex) { ABCFile = abcFile };
        return true;
    }
}