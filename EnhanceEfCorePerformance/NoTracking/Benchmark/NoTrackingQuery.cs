using NoTracking.Persistence;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NoTracking.Benchmark;

/// <summary>
/// A benchmark class to measure the performance of Entity Framework Core operations. 
/// Compares the use of standard LINQ queries with tracked entities against queries with `AsNoTracking` for optimized performance.
/// </summary>
[MemoryDiagnoser] // Enables memory usage diagnostics during benchmark execution.
public class NoTrackingQuery
{
    /// <summary>
    /// Connection string for the SQL Server database.
    /// Configures the connection to a local SQL Server instance, such as one running in a Docker container.
    /// </summary>
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// Provides access to the configured services, including the DbContext, using dependency injection.
    /// </summary>
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// Specifies the number of blog records to seed in the database for benchmarking purposes.
    /// This value determines the dataset size for performance testing.
    /// </summary>
    [Params(1000)] // Adjust the dataset size for benchmarking.
    public int NumBlogs { get; set; }

    /// <summary>
    /// Initializes the benchmark environment by configuring services, setting up the database, and seeding test data.
    /// This method is executed once before the benchmark iterations begin.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Create a service collection for dependency injection.
        var services = new ServiceCollection();

        // Register the DbContext with SQL Server configuration in the service collection.
        services.AddDbContext<BloggingDbContext>(c => c.UseSqlServer(ConnectionString));

        // Build the service provider to resolve dependencies.
        _serviceProvider = services.BuildServiceProvider();

        // Initialize the database and seed test data.
        using var context = new BloggingDbContext(new DbContextOptionsBuilder<BloggingDbContext>()
            .UseSqlServer(ConnectionString)
            .Options);

        context.Database.EnsureDeleted(); // Delete the database if it exists.
        context.Database.EnsureCreated(); // Create the database schema.
        context.SeedData(NumBlogs);       // Populate the database with test data.
    }

    /// <summary>
    /// Benchmark method to retrieve all blogs with tracked entities.
    /// Demonstrates the performance of LINQ queries where entities are tracked by the DbContext.
    /// </summary>
    [Benchmark]
    public void WithoutNoTrackingQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Retrieve all blogs, enabling entity tracking by the DbContext.
        var blogs = dbContext.Blogs
            .ToList();
    }

    /// <summary>
    /// Benchmark method to retrieve all blogs using `AsNoTracking`.
    /// Demonstrates the performance benefits of disabling change tracking for read-only queries.
    /// </summary>
    [Benchmark]
    public void WithNoTrackingQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Retrieve all blogs without tracking changes to the entities.
        var blogs = dbContext.Blogs
            .AsNoTracking()
            .ToList();
    }
}
