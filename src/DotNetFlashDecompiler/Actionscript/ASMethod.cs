using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DotNetFlashDecompiler.Abstractions;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASMethod(int NameIndex, int ReturnTypeIndex, MethodFlags Flags) : AS3Item, IABCReadable<ASMethod>
{
    public  IList<ASParameter> Parameters { get; init; } = new List<ASParameter>();

    public ASTrait? Trait { get; internal set; }
    public ASMethodBody? Body { get; internal set; }
    public bool IsConstructor { get; internal set; }
    public ASContainer? Container { get; internal set; }
    public bool IsAnonymous => Trait == null && !IsConstructor;

    public static bool TryRead(ref SequenceReader<byte> reader, ABCFile abcFile, [NotNullWhen(true)] out ASMethod? value)
    {
        value = default;
        if (!reader.TryReadInt30(out var parametersCount)) return false;
        if (!reader.TryReadInt30(out var returnTypeIndex)) return false;

        var parameters = new List<ASParameter>(parametersCount);
        for (int i = 0; i < parametersCount; i++)
        {
            if (!ASParameter.TryRead(ref reader, abcFile, out var param)) 
                return false;

            parameters.Add(param);
        }

        if (!reader.TryReadInt30(out var nameIndex)) return false;
        if (!reader.TryRead(out byte methodFlags)) return false;

        var flags = (MethodFlags)methodFlags;
        if ((flags & MethodFlags.HasOptional) != 0)
        {
            if (!reader.TryReadInt30(out var optionalParamCount)) 
                return false;

            var optionalParams = CollectionsMarshal.AsSpan(parameters)[(parameters.Count - optionalParamCount)..];
            foreach (ref readonly var parameter in optionalParams)
            {
                if (!reader.TryReadInt30(out var valueIndex)) return false;
                if (!reader.TryRead(out byte valueKind)) return false;

                parameter.IsOptional = true;
                parameter.ValueIndex = valueIndex;
                parameter.ValueKind = (ConstantKind)valueKind;
            }
        }

        if ((flags & MethodFlags.HasParamNames) != 0)
        {
            foreach (ref readonly var parameter in CollectionsMarshal.AsSpan(parameters))
            {
                if (!reader.TryReadInt30(out var paramNameIndex))
                    return false;

                parameter.NameIndex = paramNameIndex;
            }
        }

        value = new ASMethod(nameIndex, returnTypeIndex, flags) { Parameters = parameters };
        return true;
    }
}