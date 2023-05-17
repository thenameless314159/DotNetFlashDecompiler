using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace DotNetFlashDecompiler.Benchmarks;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        Add(DefaultConfig.Instance);
        AddDiagnoser(MemoryDiagnoser.Default);

        ArtifactsPath = Path.Combine(AppContext.BaseDirectory, "artifacts", DateTime.Now.ToString("yyyy-mm-dd_hh-MM-ss"));
    }
}