using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler;

public sealed record FlashRectangle(int X, int Width, int Y, int Height) : IBufferReadable<FlashRectangle>
{
    public int TwipsWidth => Width * 20;
    public int TwipsHeight => Height * 20;

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out FlashRectangle? value)
    {
        var bitReader = new BitReader();
        
        try
        {
            int maxBitsCount = bitReader.ReadUBits(ref reader, 5);

            value = new FlashRectangle(
                bitReader.ReadSBits(ref reader, maxBitsCount),
                bitReader.ReadSBits(ref reader, maxBitsCount) / 20,
                bitReader.ReadSBits(ref reader, maxBitsCount),
                bitReader.ReadSBits(ref reader, maxBitsCount) / 20);

            return true;
        }
        catch (EndOfStreamException)
        {
            value = default;
            return false;
        }
    }
}