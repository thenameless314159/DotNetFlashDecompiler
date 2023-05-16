using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler;

public sealed record FlashRectangle(int X, int Y, int Width, int Height) : IBufferReadable<FlashRectangle>
{
    public int TwipsWidth => Width * 20;
    public int TwipsHeight => Height * 20;

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out FlashRectangle? value)
    {
        var bitReader = new BitReader(ref reader);
        
        try
        {
            int maxBitsCount = bitReader.ReadUBits(5);
            value = new FlashRectangle(
                bitReader.ReadSBits(maxBitsCount),
                bitReader.ReadSBits(maxBitsCount) / 20,
                bitReader.ReadSBits(maxBitsCount),
                bitReader.ReadSBits(maxBitsCount) / 20);

            return true;
        }
        catch (EndOfStreamException)
        {
            value = default;
            return false;
        }
    }
}