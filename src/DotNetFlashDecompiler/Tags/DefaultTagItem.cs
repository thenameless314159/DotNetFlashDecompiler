namespace DotNetFlashDecompiler.Tags;

public sealed record DefaultTagItem : TagItem
{
    public override TagKind Kind { get; } = TagKind.Unknown;

    public DefaultTagItem(TagKind kind) => Kind = kind;
    public DefaultTagItem()
    {
    }
}