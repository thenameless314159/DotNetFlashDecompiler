using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class KillIns : Local
{
    public KillIns(int register)
        : base(OPCode.Kill, register)
    { }
    public KillIns(ref FlashReader input)
        : base(OPCode.Kill, ref input)
    { }

    public override void Execute(ASMachine machine)
    {
        machine.Registers[Register] = null;
    }
}
