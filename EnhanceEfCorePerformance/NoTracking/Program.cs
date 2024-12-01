using BenchmarkDotNet.Running;
using NoTracking.Benchmark;

namespace NoTracking;

/// <summary>
/// Entry point for the application.
/// Executes performance benchmarks defined in the <see cref="NoTrackingQuery"/> class.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point of the application.
    /// Utilizes BenchmarkDotNet to run and manage the benchmarking process.
    /// </summary>
    /// <param name="args">Command-line arguments (currently not used).</param>
    static void Main(string[] args)
    {
        // Executes the benchmarks specified in the CompiledQuery class.
        // BenchmarkRunner is a BenchmarkDotNet utility for running benchmarks and generating results.
        BenchmarkRunner.Run<NoTrackingQuery>();
    }
}
