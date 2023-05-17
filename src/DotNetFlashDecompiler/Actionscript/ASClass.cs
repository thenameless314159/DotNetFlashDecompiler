using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASClass(int ConstructorIndex) : ASContainer, IABCReadable<ASClass>
{
    public int InstanceIndex { get; internal set; }

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASClass? value)
    {
        if (!reader.TryReadInt30(out int ctorIndex))
        {
            value = default;
            return false;
        }

        value = new ASClass(ctorIndex) { ABCFile = abcFile };
        return value.TryPopulateTraits(ref reader);
    }
}