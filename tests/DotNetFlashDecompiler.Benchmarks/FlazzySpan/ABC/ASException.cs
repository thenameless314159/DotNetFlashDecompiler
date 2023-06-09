﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC;

public class ASException : IFlashItem, IAS3Item
{
    public ABCFile ABC { get; }

    public int To { get; set; }
    public int From { get; set; }
    public int Target { get; set; }

    public int VariableNameIndex { get; set; }
    public ASMultiname VariableName => ABC.Pool.Multinames[VariableNameIndex];

    public int ExceptionTypeIndex { get; set; }
    public ASMultiname ExceptionType => ABC.Pool.Multinames[ExceptionTypeIndex];

    public ASException(ABCFile abc)
    {
        ABC = abc;
    }
    public ASException(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        From = input.ReadEncodedInt();
        To = input.ReadEncodedInt();
        Target = input.ReadEncodedInt();
        ExceptionTypeIndex = input.ReadEncodedInt();
        VariableNameIndex = input.ReadEncodedInt();
    }

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(From);
        size += FlashWriter.GetEncodedIntSize(To);
        size += FlashWriter.GetEncodedIntSize(Target);
        size += FlashWriter.GetEncodedIntSize(ExceptionTypeIndex);
        size += FlashWriter.GetEncodedIntSize(VariableNameIndex);
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(From);
        output.WriteEncodedInt(To);
        output.WriteEncodedInt(Target);
        output.WriteEncodedInt(ExceptionTypeIndex);
        output.WriteEncodedInt(VariableNameIndex);
    }

    public string ToAS3()
    {
        throw new NotImplementedException();
    }
}
