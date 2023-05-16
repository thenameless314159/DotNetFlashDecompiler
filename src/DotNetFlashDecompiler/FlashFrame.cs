using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler;

public sealed record FlashFrame(FlashRectangle Area, ushort Rate, ushort Count) : IBufferReadable<FlashFrame>
{
    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out FlashFrame? value)
    {
        value = default;

        if (!FlashRectangle.TryRead(ref reader, out var area)) return false;
        if (!reader.TryReadBigEndian(out ushort rate)) return false;
        if (!reader.TryReadBigEndian(out ushort count)) return false;

        value = new FlashFrame(area, (ushort)(rate >> 8), count);
        return true;
    }
}