using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace DotNetFlashDecompiler.Benchmarks;

public class DotNetFlashDecompilerBenchmarks
{
    private const string FilePath = @".\Resources\TestFlashFile.swf";
    private byte[] _fileData = Array.Empty<byte>();

    [Params(false, true)]
    public bool ParseABCFile { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _fileData = File.ReadAllBytes(FilePath);
    }

    /// <summary>
    /// Baseline disassembling swf file tags then parsing DoABCTags
    /// </summary>
    [Benchmark(Baseline = true)]
    public void DotNetFlashDecompiler()
    {
        var disassembled = ShockwaveFlashFile.Disassemble(_fileData);

        if (!ParseABCFile) return;

        var abcFiles = new List<Actionscript.ABCFile>();
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan((List<Tags.TagItem>)disassembled.Tags))
        {
            if (tag.Kind != Tags.TagKind.DoABC) continue;

            abcFiles.Add(Actionscript.ABCFile.From((Tags.DoABCTag)tag));
        }
    }

    [Benchmark]
    public void Flazzy_Span()
    {
        var disassembled = FlazzySpan.ShockwaveFlash.Read(_fileData);

        if (!ParseABCFile) return;

        var abcFiles = new List<FlazzySpan.ABC.ABCFile>();
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan(disassembled.Tags))
        {
            if (tag.Kind != FlazzySpan.Tags.TagKind.DoABC) continue;

            var reader = new FlazzySpan.IO.FlashReader(((FlazzySpan.Tags.DoABCTag)tag).ABCData);
            abcFiles.Add(new FlazzySpan.ABC.ABCFile(ref reader));
        }
    }

    [Benchmark]
    public void Flazzy_Array()
    {
        var disassembled = new Flazzy.ShockwaveFlash(_fileData);
        disassembled.Disassemble();

        if (!ParseABCFile) return;

        var abcFiles = new List<Flazzy.ABC.ABCFile>();
        foreach (ref readonly var tag in CollectionsMarshal.AsSpan(disassembled.Tags))
        {
            if (tag.Kind != Flazzy.Tags.TagKind.DoABC) continue;
            
            abcFiles.Add(new Flazzy.ABC.ABCFile(((Flazzy.Tags.DoABCTag)tag).ABCData));
        }
    }
}