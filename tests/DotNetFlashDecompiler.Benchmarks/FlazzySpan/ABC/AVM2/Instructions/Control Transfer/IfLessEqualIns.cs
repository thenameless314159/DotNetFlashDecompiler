﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class IfLessEqualIns : Jumper
{
    public IfLessEqualIns()
        : base(OPCode.IfLe)
    { }
    public IfLessEqualIns(ref FlashReader input)
        : base(OPCode.IfLe, ref input)
    { }

    public override bool? RunCondition(ASMachine machine)
    {
        var right = machine.Values.Pop();
        var left = machine.Values.Pop();
        if (left == null || right == null) return null;

        return Convert.ToDouble(left) <= Convert.ToDouble(right);
    }
}
