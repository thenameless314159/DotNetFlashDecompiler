using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASMethodBody(
    int MethodIndex,
    int MaxStack,
    int LocalCount,
    int InitialScopeDepth,
    int MaxScopeDepth,
    ReadOnlySequence<byte> Code) : ASContainer, IABCReadable<ASMethodBody>
{
    public IList<ASException> Exceptions { get; init; } = new List<ASException>();

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASMethodBody? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var methodIndex)) return false;
        if (!reader.TryReadInt30(out var maxStack)) return false;
        if (!reader.TryReadInt30(out var localCount)) return false;
        if (!reader.TryReadInt30(out var initialScopeDepth)) return false;
        if (!reader.TryReadInt30(out var maxScopeDepth)) return false;
        if (!reader.TryReadInt30(out var codeLength)) return false;
        if (!reader.TryReadExact(codeLength, out var code)) return false;

        value = new ASMethodBody(methodIndex, maxStack, localCount, initialScopeDepth, maxScopeDepth, code)
        {
            Exceptions = reader.ReadAS3ItemList<ASException>(abcFile),
            ABCFile = abcFile
        };

        return value.TryPopulateTraits(ref reader);
    }
}