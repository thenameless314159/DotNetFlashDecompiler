using System.IO.Compression;

namespace System.Buffers;

public static unsafe class ZLibExtensions
{
    public static int Decompress(this ref ReadOnlySequence<byte> input, ref Span<byte> output)
        => !input.IsSingleSegment
            ? DecompressSlow(input, ref output)
            : Decompress(input.First.Span, ref output);

    private static int Decompress(ReadOnlySpan<byte> input, ref Span<byte> output)
    {
        fixed (byte* span = &input.GetPinnableReference())
        {
            using var stream = new UnmanagedMemoryStream(span, input.Length);
            using var zlibStream = new ZLibStream(stream, CompressionMode.Decompress);

            int totalRead = 0;
            while (totalRead < output.Length)
            {
                int bytesRead = zlibStream.Read(output[totalRead..]);
                if (bytesRead == 0) break;
                totalRead += bytesRead;
            }

            return totalRead;
        }
    }

    private static int DecompressSlow(ReadOnlySequence<byte> input, ref Span<byte> output)
    {
        int totalRead = 0;
        Span<byte> local = output;

        foreach (ReadOnlyMemory<byte> segment in input)
        {
            totalRead += Decompress(segment.Span, ref local);
            if (totalRead < input.Length) local = output[totalRead..];
        }

        return totalRead;
    }
}