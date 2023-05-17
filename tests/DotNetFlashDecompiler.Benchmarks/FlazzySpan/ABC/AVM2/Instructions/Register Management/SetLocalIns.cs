using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class SetLocalIns : Local
{
    public SetLocalIns(int register)
        : base(OPCode.SetLocal, register)
    { }
    public SetLocalIns(ref FlashReader input)
        : base(OPCode.SetLocal, ref input)
    { }
}
