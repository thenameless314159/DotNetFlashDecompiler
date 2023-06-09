﻿using FlazzySpan.IO;
using FlazzySpan.Tags;

using System.Buffers.Binary;

namespace FlazzySpan;

internal static class Utils
{
    internal static ImageFormat GetImageFormat(ReadOnlySpan<byte> data)
    {
        if (BinaryPrimitives.ReadInt32LittleEndian(data) == -654321153 ||
            BinaryPrimitives.ReadInt16LittleEndian(data) == -9985)
            return ImageFormat.JPEG;

        if (BinaryPrimitives.ReadInt32LittleEndian(data) == 944130375 &&
            BinaryPrimitives.ReadInt16LittleEndian(data) == 24889)
            return ImageFormat.GIF98a;

        if (BinaryPrimitives.ReadInt64LittleEndian(data) == 727905341920923785)
            return ImageFormat.PNG;

        throw new ArgumentException("Provided data contains an unknown image format.");
    }

    internal static int GetMaxPaddedBitsNeeded(ReadOnlySpan<int> values)
    {
        int maxBits = 0;
        for (int i = 0; i < values.Length; i++)
        {
            maxBits = Math.Max(maxBits, BitWriter.CountSBits(values[i]));
        }
        return maxBits;
    }
    internal static int GetByteSize(int bitSize)
    {
        const int MULTIPLER = 8;
        // Round up to multiple of 8
        int paddedSize = (bitSize + (MULTIPLER - 1)) & ~(MULTIPLER - 1);
        // Divide by 8 to byte size
        return paddedSize >> 3;
    }
}
