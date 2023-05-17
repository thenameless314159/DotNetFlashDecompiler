using System.Buffers;
using DotNetFlashDecompiler.Tags;
using System.Diagnostics.CodeAnalysis;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ABCFile : IBufferReadable<ABCFile>
{
    public ABCFile(Version version) => (Version, ConstantPool) = (version, new ASConstantPool(this));
    public ASConstantPool ConstantPool { get; }
    public Version Version { get; }

    public IList<ASMethod> Methods { get; private set; } = default!;
    public IList<ASMetadata> Metadata { get; private set; } = default!;
    public IList<ASInstance> Instances { get; private set; } = default!;
    public IList<ASClass> Classes { get; private set; } = default!;
    public IList<ASScript> Scripts { get; private set; } = default!;
    public IList<ASMethodBody> MethodBodies { get; private set; } = default!;

    public static ABCFile From(DoABCTag tag) 
    {
        var reader = new SequenceReader<byte>(tag.Data);

        return !TryRead(ref reader, out var abcFile)
            ? throw new InvalidOperationException($"Failed to read item of type : {typeof(ABCFile).FullName}")
            : abcFile;
    }

    public static bool TryRead(ref SequenceReader<byte> reader, [NotNullWhen(true)] out ABCFile? value)
    {
        value = default;
        if (!reader.TryReadLittleEndian(out ushort minor)) return false;
        if (!reader.TryReadLittleEndian(out ushort major)) return false;
        
        var abcFile = new ABCFile(new Version(major, minor));
        if (!abcFile.ConstantPool.TryUnpack(ref reader)) return false;
        
        abcFile.Methods = reader.ReadAS3ItemList<ASMethod>(abcFile);
        abcFile.Metadata = reader.ReadAS3ItemList<ASMetadata>(abcFile);
        abcFile.Instances = reader.ReadAS3ItemList<ASInstance>(abcFile);
        abcFile.Classes = GetClassesFromInstances(ref reader, abcFile);
        abcFile.Scripts = reader.ReadAS3ItemList<ASScript>(abcFile);
        abcFile.MethodBodies = reader.ReadAS3ItemList<ASMethodBody>(abcFile);

        value = abcFile;
        return true;
    }

    static IList<ASClass> GetClassesFromInstances(ref SequenceReader<byte> reader, ABCFile file)
    {
        var list = new List<ASClass>(file.Instances.Count);
        for (int i = 0; i < file.Instances.Count; i++)
        {
            if (!ASClass.TryRead(ref reader, file, out var asClass))
                throw new InvalidOperationException($"Failed to read item of type : {typeof(ASClass).FullName}");

            asClass.InstanceIndex = i;
        }

        return list;
    }

}