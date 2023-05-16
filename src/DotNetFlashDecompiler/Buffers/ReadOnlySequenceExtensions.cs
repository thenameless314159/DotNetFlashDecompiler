using System.Text;

namespace System.Buffers;

public static class ReadOnlySequenceExtensions
{
    public static string AsString(this ref ReadOnlySequence<byte> sequence, Encoding? encoding = default)
    {
        encoding ??= Encoding.UTF8;

        return sequence.IsSingleSegment
            ? encoding.GetString(sequence.FirstSpan)
            : string.Create((int)sequence.Length, sequence, (span, seq) =>
            {
                foreach (var segment in seq)
                {
                    encoding.GetChars(segment.Span, span);
                    span = span[segment.Length..];
                }
            });
    }
}