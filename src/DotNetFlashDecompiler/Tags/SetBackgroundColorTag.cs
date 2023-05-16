using System.Diagnostics.CodeAnalysis;
using System.Buffers;
using System.Drawing;

namespace DotNetFlashDecompiler.Tags;

public sealed record SetBackgroundColorTag(Color BackgroundColor) : TagItem
{
    public override TagKind Kind => TagKind.SetBackgroundColor;

    public new static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out TagItem? value)
    {
        value = default;

        if (!reader.TryRead(out byte r)) return false;
        if (!reader.TryRead(out byte g)) return false;
        if (!reader.TryRead(out byte b)) return false;

        value = new SetBackgroundColorTag(Color.FromArgb(r, g, b));
        return true;
    }
}
