﻿using FlazzySpan.IO;

namespace FlazzySpan.ABC;

public class ASScript : ASContainer // TODO: Check QName usages
{
    public int InitializerIndex { get; set; }
    public ASMethod Initializer => ABC.Methods[InitializerIndex];

    public override ASMultiname QName => Traits[0].QName;

    public ASScript(ABCFile abc)
        : base(abc)
    { }
    public ASScript(ABCFile abc, ref FlashReader input)
        : base(abc)
    {
        InitializerIndex = input.ReadEncodedInt();
        PopulateTraits(ref input);
    }

    public override int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(InitializerIndex);
        size += base.GetSize();
        return size;
    }
    public override void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(InitializerIndex);
        base.WriteTo(ref output);
    }

    public override string ToAS3()
    {
        throw new NotImplementedException();
    }
}
