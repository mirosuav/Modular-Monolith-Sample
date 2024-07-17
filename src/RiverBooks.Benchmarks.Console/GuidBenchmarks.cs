using BenchmarkDotNet.Attributes;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.Benchmarks.Console;

[MemoryDiagnoser]
public class GuidBenchmarks
{
    [Benchmark]
    public Guid OldGuid() => Guid.NewGuid();

    [Benchmark]
    public Guid UUidv7() => Uuid7.Guid();
}

/*
 
Results

   | Method  | Mean      | Error    | StdDev   | Gen0   | Allocated |
   |-------- |----------:|---------:|---------:|-------:|----------:|
   | OldGuid |  77.96 ns | 0.783 ns | 0.694 ns |      - |         - |
   | UUidv7  | 145.30 ns | 1.731 ns | 1.351 ns | 0.0050 |      32 B |

   | Method  | Mean      | Error    | StdDev   | Gen0   | Allocated |
   |-------- |----------:|---------:|---------:|-------:|----------:|
   | OldGuid |  77.06 ns | 0.764 ns | 0.638 ns |      - |         - |
   | UUidv7  | 143.01 ns | 1.386 ns | 1.082 ns | 0.0050 |      32 B |

*/