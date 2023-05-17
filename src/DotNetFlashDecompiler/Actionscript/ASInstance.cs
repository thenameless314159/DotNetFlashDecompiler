using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASInstance(
    int QNameIndex,
    int SuperIndex,
    int ProtectedNamespaceIndex,
    ClassFlags Flags, 
    int ConstructorIndex) : ASContainer, IABCReadable<ASInstance>
{
    public IList<int> InterfaceIndexes { get; init; } = new List<int>();

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASInstance? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var qNameIndex)) return false;
        if (!reader.TryReadInt30(out var superIndex)) return false;
        if (!reader.TryRead(out byte classFlags)) return false;

        var flags = (ClassFlags)classFlags;

        var protectedNamespaceIndex = -1;
        if ((flags & ClassFlags.ProtectedNamespace) != 0)
        {
            if (!reader.TryReadInt30(out protectedNamespaceIndex))
                return false;
        }

        if (!reader.TryReadInt30(out var interfaceCount)) return false;
        var interfaceIndexes = new List<int>(interfaceCount);
        for (int i = 0; i < interfaceCount; i++)
        {
            if (!reader.TryReadInt30(out var index))
                return false;

            interfaceIndexes.Add(index);
        }

        if (!reader.TryReadInt30(out var constructorIndex)) 
            return false;
        
        if (constructorIndex >= 0) {
            var ctor = abcFile.Methods[constructorIndex];
            ctor.IsConstructor = true;
            ctor.Container = value;
        }

        value = new ASInstance(qNameIndex, superIndex, protectedNamespaceIndex, flags, constructorIndex) 
        {
            InterfaceIndexes = interfaceIndexes,
            ABCFile = abcFile
        };

        return value.TryPopulateTraits(ref reader);
    }
}
