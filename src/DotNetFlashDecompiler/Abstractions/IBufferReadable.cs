using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace DotNetFlashDecompiler.Abstractions;

public interface IBufferReadable<TReadable>
{
    static abstract bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TReadable? value);
}