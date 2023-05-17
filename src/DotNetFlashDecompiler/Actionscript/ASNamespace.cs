using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASNamespace(NamespaceKind Kind, int NameIndex) : AS3Item, IABCReadable<ASNamespace>
{
    private string? _cachedName;
    public string Name => _cachedName ??= GetName();

    public string GetName() => ABCFile.ConstantPool.Strings[NameIndex];
    
    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASNamespace? value)
    {
        value = default;
        if (!reader.TryRead(out byte namespaceKind)) return false;
        if (!reader.TryReadInt30(out int nameIndex)) return false;
        
        var kind = (NamespaceKind)namespaceKind;
        if (!Enum.IsDefined(kind)) return false;

        value = new ASNamespace(kind, nameIndex) { ABCFile = abcFile };
        return true;
    }
}