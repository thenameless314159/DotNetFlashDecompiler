namespace DotNetFlashDecompiler.Actionscript;

[Flags]
public enum TraitAttribute
{
    None = 0x00,
    Final = 0x01,
    Override = 0x02,
    Metadata = 0x04
}