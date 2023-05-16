using DotNetFlashDecompiler.Tags;

namespace DotNetFlashDecompiler.Abstractions;

public interface IFlashFile
{
    CompressionKind Compression { get; }
    IList<TagItem> Tags { get; }
    FlashFrame Frame { get; }
    byte Version { get; }
}