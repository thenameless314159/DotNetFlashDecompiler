using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DotNetFlashDecompiler;
using DotNetFlashDecompiler.Abstractions;
using DotNetFlashDecompiler.Actionscript;

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
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out double value)
    {
        if (reader.TryReadBigEndian(out long temp))
        {
            value = BitConverter.Int64BitsToDouble(temp);
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadString(this ref SequenceReader<byte> reader, [NotNullWhen(true)] out string? value)
    {
        if (reader.TryReadInt30(out var len) && reader.TryReadExact(len, out ReadOnlySequence<byte> valueSeq))
        {
            value = valueSeq.AsString(Encoding.UTF8);
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadInt30(this ref SequenceReader<byte> reader, out int result)
    {
        result = 0;
        for (int shift = 0; shift < 30; shift += 7)
        {
            if (!reader.TryRead(out byte value))
                return false;

            result |= (value & 0x7F) << shift;

            if ((value & 0x80) == 0)
                return true;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadUInt30(this ref SequenceReader<byte> reader, out uint result)
    {
        if (reader.TryReadInt30(out int temp))
        {
            result = (uint)temp;
            return true;
        }

        result = default;
        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadUInt24(this ref SequenceReader<byte> reader, out uint result)
    {
        result = 0;
        for (int shift = 0; shift < 24; shift += 8)
        {
            if (!reader.TryRead(out byte value))
                return false;

            result |= (uint)value << shift;
        }

        if (result >> 23 == 1)
            result |= 0xff000000;

        return true;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> ReadAS3ItemList<TItem>(this ref SequenceReader<byte> reader, ABCFile file)
        where TItem : AS3Item, IABCReadable<TItem>
    {
        if (!reader.TryReadInt30(out var length))
            return default!;

        var list = new List<TItem>(length);
        foreach (ref var item in CollectionsMarshal.AsSpan(list))
        {
            if (!TItem.TryRead(ref reader, file, out var i))
                throw new InvalidOperationException($"Failed to read item of type : {typeof(TItem).FullName}");

            item = i;
        }

        return list;
    }
}