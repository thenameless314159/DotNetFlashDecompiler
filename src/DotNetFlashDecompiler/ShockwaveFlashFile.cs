using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetFlashDecompiler.Tags;
using System.Buffers;

namespace DotNetFlashDecompiler;

public sealed record ShockwaveFlashFile(byte Version, CompressionKind Compression, FlashFrame Frame) : IShockwaveFlashFile, IBufferReadable<IShockwaveFlashFile>
{
    public IList<TagItem> Tags { get; init; } = new List<TagItem>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IShockwaveFlashFile Disassemble(byte[] buffer) 
        => Disassemble(new ReadOnlySequence<byte>(buffer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IShockwaveFlashFile Disassemble(ReadOnlyMemory<byte> memory) 
        => Disassemble(new ReadOnlySequence<byte>(memory));

    public static IShockwaveFlashFile Disassemble(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);

        return !TryRead(ref reader, out var decompiledFile) 
            ? throw new ArgumentOutOfRangeException(nameof(sequence), "The given sequence is not a valid Flash file or an unexpected error occurred while trying to disassemble.")
            : decompiledFile;
    }

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out IShockwaveFlashFile? value)
    {
        value = default;
        if (!reader.TryPeek(out byte compression)) return false;
        var compressionKind = (CompressionKind)compression;
        reader.Advance(3);

        if (!reader.TryRead(out byte version)) return false;
        if (!reader.TryReadLittleEndian(out uint length)) return false;

        length -= 8;
        var body = reader.UnreadSequence;

        switch (compressionKind)
        {
            case CompressionKind.None: break;
            // TODO : find a way to avoid this re-allocation
            case CompressionKind.ZLib:
            {
                var decompressed = new byte[length];
                var decompressedSpan = decompressed.AsSpan();
                var totalSize = body.Decompress(ref decompressedSpan);
                body = new ReadOnlySequence<byte>(decompressed[..totalSize]);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(compressionKind),
                    "The given compression kind is not supported.");
        }

        var bodyReader = new SequenceReader<byte>(body);
        if (!FlashFrame.TryRead(ref bodyReader, out var frame))
            return false;

        var tagItems = new List<TagItem>((int)bodyReader.Length / 2);
        while (TagItem.TryRead(ref bodyReader, out var tagItem))
        {
            tagItems.Add(tagItem);

            if (tagItem.Kind == TagKind.End) break;
        }

        value = new ShockwaveFlashFile(version, compressionKind, frame) { Tags = tagItems };
        return true;
    }
}