using BenchmarkDotNet.Running;
using DbContextPooling.Benchmark;
using Microsoft.Identity.Client;
using System;

namespace DbContextPooling;

/// <summary>
/// Entry point for the application.
/// Responsible for running benchmarks defined in the <see cref="ContextPooling"/> class.
/// </summary>
public class Program
{
    /// <summary>
    /// The Main method serves as the entry point for the application.
    /// It executes the benchmarking process using BenchmarkDotNet.
    /// </summary>
    /// <param name="args">Command-line arguments (not used in this implementation).</param>
    static void Main(string[] args)
    {
        // Run the benchmarks defined in the ContextPooling class
        // BenchmarkRunner is a utility provided by BenchmarkDotNet to execute and manage benchmarks
        BenchmarkRunner.Run<ContextPooling>();
    }
}
