namespace System.Buffers;

public ref struct BitReader
{
    public BitReader(ref SequenceReader<byte> reader) => _reader = reader;

    private SequenceReader<byte> _reader; // mutable struct, do not make readonly
    private byte _currentByte = 0;
    private int _bitPosition = 0;

    public int ReadUBits(int count)
    {
        if (count is <= 0 or > 8) throw new ArgumentOutOfRangeException(nameof(count));
        if (_bitPosition == 0 && !_reader.TryRead(out _currentByte))
            throw new EndOfStreamException();

        int remainingBitsInByte = 8 - _bitPosition;
        int bitsToRead = Math.Min(count, remainingBitsInByte);

        int result = _currentByte >> remainingBitsInByte - bitsToRead & (1 << bitsToRead) - 1;
        _bitPosition += bitsToRead;
        count -= bitsToRead;

        while (count > 0)
        {
            if (!_reader.TryRead(out _currentByte)) throw new EndOfStreamException();
            bitsToRead = Math.Min(count, 8);

            result = result << bitsToRead | _currentByte >> 8 - bitsToRead & (1 << bitsToRead) - 1;
            _bitPosition = bitsToRead;
            count -= bitsToRead;
        }

        return result;
    }

    public int ReadSBits(int count)
    {
        if (count is <= 0 or > 8) throw new ArgumentOutOfRangeException(nameof(count));

        int result = ReadUBits(count);
        int shift = 32 - count;

        return result << shift >> shift;
    }
}