using DotNetFlashDecompiler.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNetFlashDecompiler.Tags;
using System.Buffers;

namespace DotNetFlashDecompiler;

public sealed record FlashFile(byte Version, CompressionKind Compression, FlashFrame Frame) : IFlashFile, IBufferReadable<IFlashFile>
{
    private List<TagItem>? _tags;

    public IList<TagItem> Tags
    {
        get => _tags ??= new List<TagItem>(); 
        init => _tags = value as List<TagItem>;
    }

    /// <summary>
    /// Allow to get the inner span from the <see pref="Tags"/> list for high-performance scenarios.
    /// </summary>
    /// <returns>A span of <see cref="TagItem"/>.</returns>
    public Span<TagItem> GetTags() => CollectionsMarshal.AsSpan(_tags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFlashFile Disassemble(byte[] buffer) 
        => Disassemble(new ReadOnlySequence<byte>(buffer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFlashFile Disassemble(ReadOnlyMemory<byte> memory) 
        => Disassemble(new ReadOnlySequence<byte>(memory));

    public static IFlashFile Disassemble(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);

        return !TryRead(ref reader, out var decompiledFile) 
            ? throw new ArgumentOutOfRangeException(nameof(sequence), "The given sequence is not a valid Flash file or an unexpected error occurred while trying to disassemble.")
            : decompiledFile;
    }

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out IFlashFile? value)
    {
        value = default;
        if (!reader.TryPeek(out byte compression)) return false;
        var compressionKind = (CompressionKind)compression;
        reader.Advance(3);

        if (!reader.TryRead(out byte version)) return false;
        if (!reader.TryReadBigEndian(out uint length)) return false;

        var body = reader.Sequence.Slice(reader.Consumed, length - 8);
        if (compressionKind != CompressionKind.None)
        {
            if (compressionKind != CompressionKind.ZLib)
                throw new ArgumentOutOfRangeException(nameof(compressionKind),
                    "The given compression kind is not supported.");

            var decompressed = new byte[body.Length];
            var decompressedSpan = decompressed.AsSpan();
            var totalSize = body.Decompress(ref decompressedSpan);
            body = new ReadOnlySequence<byte>(decompressed[..totalSize]);
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

        value = new FlashFile(version, compressionKind, frame) { Tags = tagItems };
        return true;
    }
}