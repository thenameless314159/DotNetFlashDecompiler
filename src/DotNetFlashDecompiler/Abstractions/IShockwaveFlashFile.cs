using DotNetFlashDecompiler.Tags;

namespace DotNetFlashDecompiler.Abstractions;

public interface IShockwaveFlashFile
{
    CompressionKind Compression { get; }
    IList<TagItem> Tags { get; }
    FlashFrame Frame { get; }
    byte Version { get; }
}