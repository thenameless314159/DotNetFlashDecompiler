using DotNetFlashDecompiler.Actionscript;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;

namespace DotNetFlashDecompiler.Abstractions
{
    public interface IABCReadable<TReadable> where TReadable : AS3Item
    {
        static abstract bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out TReadable? value);
    }
}
