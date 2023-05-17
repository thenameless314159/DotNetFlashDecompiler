﻿namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class PushFalseIns : Primitive
{
    public override object Value
    {
        get => false;
        set => throw new NotSupportedException();
    }

    public PushFalseIns()
        : base(OPCode.PushFalse)
    { }
}