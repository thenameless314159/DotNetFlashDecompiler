using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class JumpIns : Jumper
{
    public JumpIns()
        : base(OPCode.Jump)
    { }
    public JumpIns(ref FlashReader input)
        : base(OPCode.Jump, ref input)
    { }

    public override bool? RunCondition(ASMachine machine) => true;
}
