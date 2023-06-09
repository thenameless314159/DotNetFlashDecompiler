﻿using FlazzySpan.IO;

namespace FlazzySpan.Tags;

public class UnknownTag : ITagItem
{
    public TagKind Kind { get; }
    public byte[] Data { get; set; }

    public UnknownTag(TagKind kind)
    {
        Kind = kind;
        Data = Array.Empty<byte>();
    }
    public UnknownTag(ref FlashReader input, TagKind kind)
        : this(kind)
    {
        Data = new byte[input.Length];
        input.ReadBytes(Data);
    }

    public int GetBodySize() => Data.Length;
    public void WriteBodyTo(ref FlashWriter output)
    {
        output.Write(Data);
    }
}
