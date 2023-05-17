using BenchmarkDotNet.Running;
using DotNetFlashDecompiler.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
    .Run(args, new BenchmarkConfig());