using DotNetFlashDecompiler.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
    .Run(args, new BenchmarkConfig());