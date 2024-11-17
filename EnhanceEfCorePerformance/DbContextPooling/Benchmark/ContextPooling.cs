using DbContextPooling.Persistence;
using DbContextPooling.Domain;
using DbContextPooling.Controllers;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextPooling.Benchmark;

/// <summary>
/// A benchmark class for comparing the performance of Entity Framework Core 
/// with and without DbContext pooling.
/// </summary>
[MemoryDiagnoser] // Enables memory usage diagnostics for benchmarks
public class ContextPooling
{
    /// <summary>
    /// Connection string for the SQL Server database.
    /// Adjusts for a local database instance running in a Docker container or similar setup.
    /// </summary>
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// Provides access to the registered services for the benchmark.
    /// Used to resolve DbContext instances during benchmarking.
    /// </summary>
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// The number of blogs to seed in the database during the setup phase.
    /// Controlled as a parameter for the benchmark to test scalability.
    /// </summary>
    [Params(1)] // You can add more values, e.g., [Params(1, 10, 100)] to benchmark with varying data sizes
    public int NumBlogs { get; set; }

    /// <summary>
    /// Sets up the database, registers services, and seeds test data before running benchmarks.
    /// This method is executed once before any benchmark iterations.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Create a service collection to register dependencies
        var services = new ServiceCollection();

        // Configure options for the DbContext
        var options = new DbContextOptionsBuilder<BloggingDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        // Register DbContext pooling
        services.AddDbContextPool<BloggingDbContext>(c => c.UseSqlServer(ConnectionString));

        // Register DbContext factory (non-pooling)
        services.AddDbContextFactory<BloggingDbContext>(c => c.UseSqlServer(ConnectionString));

        // Build the service provider
        _serviceProvider = services.BuildServiceProvider();

        // Initialize the database with seeded data
        using var context = new BloggingDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.SeedData(NumBlogs);
    }

    /// <summary>
    /// Benchmark method for accessing the database without using DbContext pooling.
    /// Uses an <see cref="IDbContextFactory{TContext}"/> to create a new DbContext instance.
    /// </summary>
    /// <returns>A <see cref="Blog"/> object representing the retrieved data.</returns>
    [Benchmark]
    public Blog WithoutContextPooling()
    {
        // Create a service scope for resolving dependencies
        using var scope = _serviceProvider.CreateScope();

        // Resolve the DbContext factory and create a new DbContext instance
        var dbContext = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<BloggingDbContext>>()
            .CreateDbContext();

        // Use the DbContext in a controller action
        return new BlogController(dbContext).Action();
    }

    /// <summary>
    /// Benchmark method for accessing the database using DbContext pooling.
    /// Retrieves a pre-pooled DbContext instance.
    /// </summary>
    /// <returns>A <see cref="Blog"/> object representing the retrieved data.</returns>
    [Benchmark]
    public Blog WithContextPooling()
    {
        // Create a service scope for resolving dependencies
        using var scope = _serviceProvider.CreateScope();

        // Resolve a pooled DbContext instance
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Use the DbContext in a controller action
        return new BlogController(dbContext).Action();
    }
}
