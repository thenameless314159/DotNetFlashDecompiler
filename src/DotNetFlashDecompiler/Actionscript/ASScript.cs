using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASScript(int InitializerIndex) : ASContainer, IABCReadable<ASScript>
{
    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASScript? value)
    {
        if (!reader.TryReadInt30(out int initIndex))
        {
            value = default;
            return false;
        }

        value = new ASScript(initIndex) { ABCFile = abcFile };
        return value.TryPopulateTraits(ref reader);
    }
}