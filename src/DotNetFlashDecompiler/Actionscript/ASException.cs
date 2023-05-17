using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASException(int From, int To, int Target, int ExceptionTypeIndex, int VariableNameIndex) : AS3Item, IABCReadable<ASException>
{
    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASException? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var from)) return false;
        if (!reader.TryReadInt30(out var to)) return false;
        if (!reader.TryReadInt30(out var target)) return false;
        if (!reader.TryReadInt30(out var exceptionTypeIndex)) return false;
        if (!reader.TryReadInt30(out var variableNameIndex)) return false;

        value = new ASException(from, to, target, exceptionTypeIndex, variableNameIndex) { ABCFile = abcFile };
        return true;
    }
}