﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class IfStrictEqualIns : Jumper
{
    public IfStrictEqualIns()
        : base(OPCode.IfStrictEq)
    { }
    public IfStrictEqualIns(ref FlashReader input)
        : base(OPCode.IfStrictEq, ref input)
    { }

    public override bool? RunCondition(ASMachine machine)
    {
        //var right = (machine.Values.Pop() as IComparable);
        //var left = (machine.Values.Pop() as IComparable);
        //if (left == null || right == null) return null;

        //return (left.CompareTo(right) == 0);
        return null;
    }
}
