using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASItemInfo(int KeyIndex, int ValueIndex) : AS3Item, IABCReadable<ASItemInfo>
{
    public string Key => ABCFile.ConstantPool.Strings[KeyIndex];
    public string Value => ABCFile.ConstantPool.Strings[ValueIndex]; 
    
    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASItemInfo? value)
    {
        if (!reader.TryReadInt30(out var keyIndex))
        {
            value = default;
            return false;
        }

        if (!reader.TryReadInt30(out var valueIndex))
        {
            value = default;
            return false;
        }

        value = new ASItemInfo(keyIndex, valueIndex) { ABCFile = abcFile };
        return true;
    }
}
