namespace DotNetFlashDecompiler.Actionscript;

public abstract record AS3Item
{
    public ABCFile ABCFile { get; protected internal set; } = default!;
}