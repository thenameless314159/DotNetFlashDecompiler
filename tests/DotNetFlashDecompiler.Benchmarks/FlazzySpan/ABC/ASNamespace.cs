﻿using FlazzySpan.IO;

using System.Diagnostics;

namespace FlazzySpan.ABC;

/// <summary>
/// Represents a namespace in the bytecode.
/// </summary>
[DebuggerDisplay("{Kind}: \"{Name}\"")]
public class ASNamespace : IEquatable<ASNamespace>, IFlashItem, IPoolConstant
{
    public ASConstantPool Pool { get; init; }

    /// <summary>
    /// Gets or sets the index of the string in <see cref="ASConstantPool.Strings"/> representing the namespace name.
    /// </summary>
    public int NameIndex { get; set; }

    /// <summary>
    /// Gets the name of the namespace.
    /// </summary>
    public string Name => Pool.Strings[NameIndex];

    /// <summary>
    /// Gets or sets the kind of namespace this entry should be interpreted as by the loader.
    /// </summary>
    public NamespaceKind Kind { get; set; }

    public static bool operator ==(ASNamespace left, ASNamespace right)
    {
        return EqualityComparer<ASNamespace>.Default.Equals(left, right);
    }
    public static bool operator !=(ASNamespace left, ASNamespace right)
    {
        return !(left == right);
    }

    public ASNamespace(ASConstantPool pool)
    {
        Pool = pool;
    }
    public ASNamespace(ASConstantPool pool, ref FlashReader input)
        : this(pool)
    {
        Kind = (NamespaceKind)input.ReadByte();
        if (!Enum.IsDefined(Kind))
        {
            throw new InvalidCastException($"Invalid namespace kind for value {Kind:0x00}.");
        }
        NameIndex = input.ReadEncodedInt();
    }

    public string GetAS3Modifiers() => Kind switch
    {
        NamespaceKind.Package => "public",
        NamespaceKind.Private => "private",
        NamespaceKind.Explicit => "explicit",
        NamespaceKind.StaticProtected or NamespaceKind.Protected => "protected",

        _ => string.Empty
    };

    public int GetSize()
    {
        int size = 0;
        size += sizeof(byte);
        size += FlashWriter.GetEncodedIntSize(NameIndex);
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.Write((byte)Kind);
        output.WriteEncodedInt(NameIndex);
    }

    public override int GetHashCode() 
        => HashCode.Combine(Name, Kind);
    
    public bool Equals(ASNamespace other)
        => Name == other.Name && Kind == other.Kind;
    public override bool Equals(object obj) 
        => obj is ASNamespace other && Equals(other);
}