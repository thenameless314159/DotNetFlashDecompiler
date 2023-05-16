using System.Runtime.CompilerServices;
using DotNetFlashDecompiler;

namespace System.Buffers;

public static class SequenceReaderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out ushort value)
    {
        if (reader.TryReadBigEndian(out short temp))
        {
            value = (ushort)temp;
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out uint value)
    {
        if (reader.TryReadBigEndian(out int temp))
        {
            value = (uint)temp;
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out ulong value)
    {
        if (reader.TryReadBigEndian(out long temp))
        {
            value = (ulong)temp;
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadImageFormat(this ref SequenceReader<byte> reader, out ImageFormat format)
    {
        if (reader.TryReadLittleEndian(out int i) && i == -654321153 ||
            reader.TryReadLittleEndian(out short s) && s == -9985)
        {
            format = ImageFormat.JPEG;
            return true;
        }

        if (reader.TryReadLittleEndian(out i) && i == 944130375 &&
            reader.TryReadLittleEndian(out s) && s == 24889)
        {
            format = ImageFormat.GIF98a;
            return true;
        }

        if (reader.TryReadLittleEndian(out long value) && value == 727905341920923785)
        {
            format = ImageFormat.PNG;
            return true;
        }

        format = default;
        return false;
    }
}