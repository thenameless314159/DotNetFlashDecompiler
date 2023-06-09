﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class GetLocalIns : Local
{
    public GetLocalIns(int register)
        : base(OPCode.GetLocal, register)
    { }
    public GetLocalIns(ref FlashReader input)
        : base(OPCode.GetLocal, ref input)
    { }
}
