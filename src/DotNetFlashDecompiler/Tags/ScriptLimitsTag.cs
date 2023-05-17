using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Tags;

public sealed record ScriptLimitsTag(ushort MaxRecursionDepth, ushort ScriptTimeoutSeconds) : TagItem, IBufferReadable<TagItem>
{
    public override TagKind Kind => TagKind.ScriptLimits;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;
        if (!reader.TryReadLittleEndian(out ushort maxRecursionDepth)) return false;
        if (!reader.TryReadLittleEndian(out ushort scriptTimeoutSeconds)) return false;
        value = new ScriptLimitsTag(maxRecursionDepth, scriptTimeoutSeconds);
        return true;
    }
}
