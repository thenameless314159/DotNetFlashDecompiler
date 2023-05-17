﻿using System.Buffers;

namespace DotNetFlashDecompiler.Actionscript;

public sealed record ASConstantPool : AS3Item
{
    public ASConstantPool(ABCFile abcFile) => ABCFile = abcFile;

    public IList<int> Integers { get; private set; } = default!;
    public IList<uint> UIntegers { get; private set; } = default!;
    public IList<double> Doubles { get; private set; } = default!;
    public IList<string> Strings { get; private set; } = default!;
    public IList<ASNamespace> Namespaces { get; private set; } = default!;
    public IList<ASNamespaceSet> NamespaceSets { get; private set; } = default!;
    public IList<ASMultiname> Multinames { get; private set; } = default!;

    public bool TryUnpack(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadInt30(out int length))
            return false;

        Integers = new List<int>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadInt30(out int integer)) 
                return false;

            Integers.Add(integer);
        }

        if (!reader.TryReadInt30(out length))
            return false;

        UIntegers = new List<uint>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadUInt30(out uint uinteger)) 
                return false;

            UIntegers.Add(uinteger);
        }

        if (!reader.TryReadInt30(out length)) 
            return false;

        Doubles = new List<double>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadBigEndian(out double d)) 
                return false;

            Doubles.Add(d);
        }

        if (!reader.TryReadInt30(out length))
            return false;

        Strings = new List<string>(length);
        for (int i = 0; i < length; i++)
        {
            if (!reader.TryReadString(out var s)) 
                return false;

            Strings.Add(s);
        }

        if (!reader.TryReadInt30(out length))
            return false;

        Namespaces = new List<ASNamespace>(length);
        for (int i = 0; i < length; i++)
        {
            if (!ASNamespace.TryRead(ref reader, ABCFile, out var ns)) 
                return false;

            Namespaces.Add(ns);
        }

        if (!reader.TryReadInt30(out length))
            return false;

        NamespaceSets = new List<ASNamespaceSet>(length);
        for (int i = 0; i < length; i++)
        {
            if (!ASNamespaceSet.TryRead(ref reader, ABCFile, out var nsSet)) 
                return false;

            NamespaceSets.Add(nsSet);
        }

        if (!reader.TryReadInt30(out length))
            return false;

        Multinames = new List<ASMultiname>(length);
        for (int i = 0; i < length; i++)
        {
            if (!ASMultiname.TryRead(ref reader, ABCFile, out var multiname))
                return false;

            Multinames.Add(multiname);
        }

        return true;
    }
}