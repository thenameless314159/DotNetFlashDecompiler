﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC.AVM2.Instructions;

public sealed class CallSuperVoidIns : ASInstruction
{
    public int MethodNameIndex { get; set; }
    public ASMultiname MethodName => ABC.Pool.Multinames[MethodNameIndex];

    public int ArgCount { get; set; }

    public CallSuperVoidIns(ABCFile abc)
        : base(OPCode.CallSuperVoid, abc)
    { }
    public CallSuperVoidIns(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        MethodNameIndex = input.ReadEncodedInt();
        ArgCount = input.ReadEncodedInt();
    }
    public CallSuperVoidIns(ABCFile abc, int methodNameIndex)
        : this(abc)
    {
        MethodNameIndex = methodNameIndex;
    }
    public CallSuperVoidIns(ABCFile abc, int methodNameIndex, int argCount)
        : this(abc)
    {
        MethodNameIndex = methodNameIndex;
        ArgCount = argCount;
    }

    public override int GetPopCount()
    {
        return ArgCount + ResolveMultinamePops(MethodName) + 1;
    }
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        for (int i = 0; i < ArgCount; i++)
        {
            machine.Values.Pop();
        }
        ResolveMultiname(machine, MethodName);
        object receiver = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(MethodNameIndex);
        size += FlashWriter.GetEncodedIntSize(ArgCount);
        return size;
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(MethodNameIndex);
        output.WriteEncodedInt(ArgCount);
    }
}