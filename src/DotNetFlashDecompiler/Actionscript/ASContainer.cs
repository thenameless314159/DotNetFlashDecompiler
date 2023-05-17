using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
namespace DotNetFlashDecompiler.Actionscript;

public abstract record ASContainer : AS3Item
{
    public IList<ASTrait> Traits { get; private set; } = new List<ASTrait>();
    public virtual bool IsStatic  { get; internal set; }

    protected internal bool TryPopulateTraits(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadInt30(out var traitsCount)) return false;

        var traits = new List<ASTrait>(traitsCount);
        for (int i = 0; i < traitsCount; i++)
        {
            if (!ASTrait.TryRead(ref reader, ABCFile, out var trait))
                return false;

            if (trait.Kind is TraitKind.Method
                or TraitKind.Getter
                or TraitKind.Setter)
            {
                trait.Method.Container = this;
            }

            if (IsStatic) trait.IsStatic = true;
            Traits.Add(trait);
        }

        Traits = traits;
        return true;
    }
}