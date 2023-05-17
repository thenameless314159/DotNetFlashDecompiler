using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace DotNetFlashDecompiler.Benchmarks;

public class DotNetFlashDecompilerBenchmarks
{
    private const string FilePath = @".\Resources\TestFlashFile.swf";
    private byte[] _fileData = Array.Empty<byte>();


    [GlobalSetup]
    public void Setup()
    {
        _fileData = File.ReadAllBytes(FilePath);
    }

    /// <summary>
    /// Baseline disassembling swf file tags then parsing DoABCTags
    /// </summary>
    [Benchmark]
    public void FlashDecompiler()
    {
        var disassembled = ShockwaveFlashFile.Disassemble(_fileData);
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan((List<Tags.TagItem>)disassembled.Tags))
        {
            if (tag.Kind != Tags.TagKind.DoABC) continue;

            _ = Actionscript.ABCFile.From((Tags.DoABCTag)tag);
        }
    }

    [Benchmark]
    public void Flazzy_Span()
    {
        var disassembled = FlazzySpan.ShockwaveFlash.Read(_fileData);
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan(disassembled.Tags))
        {
            if (tag.Kind != FlazzySpan.Tags.TagKind.DoABC) continue;

            var reader = new FlazzySpan.IO.FlashReader(((FlazzySpan.Tags.DoABCTag)tag).ABCData);
            _ = new FlazzySpan.ABC.ABCFile(ref reader);
        }
    }

    [Benchmark]
    public void Flazzy_Array()
    {
        var disassembled = new Flazzy.ShockwaveFlash(_fileData);
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan(disassembled.Tags))
        {
            if (tag.Kind != Flazzy.Tags.TagKind.DoABC) continue;
            
            _ = new Flazzy.ABC.ABCFile(((Flazzy.Tags.DoABCTag)tag).ABCData);
        }
    }
}