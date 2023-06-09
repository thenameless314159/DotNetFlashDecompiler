﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class InitPropertyIns : ASInstruction
{
    public int PropertyNameIndex { get; set; }
    public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

    public InitPropertyIns(ABCFile abc)
        : base(OPCode.InitProperty, abc)
    { }
    public InitPropertyIns(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        PropertyNameIndex = input.ReadEncodedInt();
    }
    public InitPropertyIns(ABCFile abc, int propertyNameIndex)
        : this(abc)
    {
        PropertyNameIndex = propertyNameIndex;
    }

    public override int GetPopCount()
    {
        return 1 + ResolveMultinamePops(PropertyName) + 1;
    }
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        ResolveMultiname(machine, PropertyName);
        object obj = machine.Values.Pop();
    }

    protected override int GetBodySize()
    {
        return FlashWriter.GetEncodedIntSize(PropertyNameIndex);
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(PropertyNameIndex);
    }
}
