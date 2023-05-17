namespace System.Buffers;

public ref struct BitReader
{
    public BitReader() => (_currentByte, _bitPosition) = (0, 0);
    private byte _currentByte;
    private int _bitPosition;

    public int ReadUBits(ref SequenceReader<byte> reader, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (_bitPosition == 0)
        {
            if (!reader.TryRead(out byte b)) throw new EndOfStreamException();

            _currentByte = b;
        }

        int result = 0;
        for (int i = 0; i < count; i++)
        {
            int bit = (_currentByte >> (7 - _bitPosition)) & 1;
            result += bit << (count - 1 - i);

            if (++_bitPosition == 8)
            {
                _bitPosition = 0; // reset bit position

                if (i != count - 1)
                {
                    if (!reader.TryRead(out byte b)) throw new EndOfStreamException();

                    _currentByte = b;
                }
            }
        }

        return result;
    }

    public int ReadSBits(ref SequenceReader<byte> reader, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

        int result = ReadUBits(ref reader, count);
        int shift = 32 - count;

        return result << shift >> shift;
    }
}