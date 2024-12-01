using CompiledQueries.Persistence;
using CompiledQueries.Domain;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CompiledQueries.Benchmark;

/// <summary>
/// A benchmark class designed to evaluate the performance of Entity Framework Core operations, 
/// comparing the use of compiled queries against regular LINQ queries for optimized data retrieval.
/// </summary>
[MemoryDiagnoser] // Enables memory usage diagnostics during benchmark execution.
public class CompiledQuery
{
    /// <summary>
    /// The connection string for the SQL Server database.
    /// Assumes a local SQL Server instance, such as one running in a Docker container.
    /// </summary>
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// A compiled query that retrieves blogs with a specific rating.
    /// This leverages EF Core's compiled query feature for faster execution by pre-compiling the query.
    /// </summary>
    private static readonly Func<BloggingDbContext, int, IEnumerable<Blog>> GetHighestRatedBlogs =
        EF.CompileQuery((BloggingDbContext context, int maxRating) =>
            context.Blogs
                   .Where(b => b.Rating == maxRating) // Filter blogs with the max rating.
        );

    /// <summary>
    /// The service provider used for dependency injection, providing access to the DbContext.
    /// </summary>
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// The maximum rating value found in the seeded blogs. This is used to filter the highest-rated blogs 
    /// in both benchmark methods. It is determined during the database setup process.
    /// </summary>
    private int _maxRating;

    /// <summary>
    /// The number of blogs to seed in the database. This determines the size of the dataset for benchmarking.
    /// </summary>
    [Params(1, 10)] // Change these values to test performance with different dataset sizes.
    public int NumBlogs { get; set; }

    /// <summary>
    /// Sets up the database, configures services, and seeds the required data.
    /// This method is called once before the benchmark iterations start.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Configure dependency injection.
        var services = new ServiceCollection();

        // Configure DbContext to use SQL Server with the specified connection string.
        var options = new DbContextOptionsBuilder<BloggingDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        // Register DbContext with the service collection.
        services.AddDbContext<BloggingDbContext>(c => c.UseSqlServer(ConnectionString));

        // Build the service provider.
        _serviceProvider = services.BuildServiceProvider();

        // Initialize and seed the database with the specified number of blogs.
        using var context = new BloggingDbContext(options);
        context.Database.EnsureDeleted(); // Clean up any existing database.
        context.Database.EnsureCreated(); // Create the database schema.
        context.SeedData(NumBlogs);      // Seed the database with blogs.
        _maxRating = context.Blogs.Max(blog => blog.Rating); // Determine the maximum blog rating.
    }

    /// <summary>
    /// Benchmark method that retrieves the highest-rated blogs without using a compiled query.
    /// Uses a standard LINQ query to perform the data retrieval.
    /// </summary>
    [Benchmark]
    public void WithoutCompiledQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Execute the LINQ query to retrieve blogs with the maximum rating.
        var result = dbContext.Blogs
                        .Where(b => b.Rating == _maxRating) // Filter blogs with the max rating.
                        .ToList();
    }

    /// <summary>
    /// Benchmark method that retrieves the highest-rated blogs using a precompiled query.
    /// Demonstrates the performance improvement offered by compiled queries.
    /// </summary>
    [Benchmark]
    public void WithCompiledQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Execute the precompiled query to retrieve blogs with the maximum rating.
        var result = GetHighestRatedBlogs(dbContext, _maxRating);
    }
}
