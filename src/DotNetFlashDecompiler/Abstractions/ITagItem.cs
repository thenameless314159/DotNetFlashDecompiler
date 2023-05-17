using DotNetFlashDecompiler.Tags;

namespace DotNetFlashDecompiler.Abstractions;

public interface ITagItem
{
    TagKind Kind { get; }
}