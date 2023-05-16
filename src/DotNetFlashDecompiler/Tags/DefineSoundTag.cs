using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record DefineSoundTag(ushort Id, int SoundType, int Size, int Format, int SampleRate, uint SoundSampleCount, 
    ReadOnlySequence<byte> Data) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.DefineSound;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryReadBigEndian(out ushort id)) return false;
        if (!reader.TryRead(out byte flags)) return false;

        var sampleRate = ((flags & 0b1100) >> 2) switch
        {
            0 => 5512,
            1 => 11025,
            2 => 22050,
            3 => 44100,
            _ => throw new InvalidDataException("Invalid sample rate value.")
        };

        if (!reader.TryReadBigEndian(out uint soundSampleCount)) return false;
        if (!reader.TryReadExact((int)reader.Remaining, out var data)) return false;

        value = new DefineSoundTag(id, flags & 1, flags & 2, flags >> 4, sampleRate, soundSampleCount, data);
        return true;
    }
}
