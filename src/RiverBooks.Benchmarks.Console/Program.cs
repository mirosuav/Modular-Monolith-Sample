using BenchmarkDotNet.Running;

namespace RiverBooks.Benchmarks.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        System.Console.WriteLine("Benchmarks started...");
        BenchmarkRunner.Run<GuidBenchmarks>();
    }
}