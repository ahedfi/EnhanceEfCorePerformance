using Projection.Persistence;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Projection.Benchmark;

/// <summary>
/// A benchmark class to measure the performance of Entity Framework Core operations. 
/// Compares the use of regular LINQ queries against projection queries for optimized data retrieval.
/// </summary>
[MemoryDiagnoser] // Tracks memory usage during benchmark execution.
public class ProjectionQuery
{
    /// <summary>
    /// Connection string for the SQL Server database.
    /// Assumes a local SQL Server instance (e.g., running in a Docker container).
    /// </summary>
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// Dependency injection service provider to resolve the DbContext.
    /// </summary>
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// The number of blogs to seed in the database for benchmarking.
    /// Adjust this parameter to simulate datasets of different sizes.
    /// </summary>
    [Params(1000)] // Specify the dataset size for performance tests.
    public int NumBlogs { get; set; }

    /// <summary>
    /// Sets up the benchmark environment by configuring services and seeding the database.
    /// Called once before running benchmark iterations.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Configure services for dependency injection.
        var services = new ServiceCollection();

        // Register the DbContext with SQL Server configuration.
        services.AddDbContext<BloggingDbContext>(c => c.UseSqlServer(ConnectionString));

        // Build the service provider for resolving dependencies.
        _serviceProvider = services.BuildServiceProvider();

        // Initialize and seed the database.
        using var context = new BloggingDbContext(new DbContextOptionsBuilder<BloggingDbContext>()
            .UseSqlServer(ConnectionString)
            .Options);

        context.Database.EnsureDeleted(); // Ensure the database is reset.
        context.Database.EnsureCreated(); // Recreate the database schema.
        context.SeedData(NumBlogs);       // Populate the database with test data.
    }

    /// <summary>
    /// Benchmark method that retrieves all blogs without projection.
    /// Demonstrates the performance of basic LINQ queries.
    /// </summary>
    [Benchmark]
    public void WithoutProjectionQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Retrieve all blogs using LINQ without projection.
        var blogs = dbContext.Blogs.ToList();
    }

    /// <summary>
    /// Benchmark method that retrieves blog details using projection.
    /// Demonstrates the performance benefits of selecting only necessary fields.
    /// </summary>
    [Benchmark]
    public void WithProjectionQuery()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

        // Retrieve specific fields (Name, Author) using a projection query.
        var blogs = dbContext.Blogs
            .Select(b => new BlogDto
            {
                Name = b.Name,
                Author = b.Author
            }).ToList();
    }
}

/// <summary>
/// Data transfer object (DTO) for projecting specific fields from the Blog entity.
/// Used to reduce the data loaded from the database and optimize performance.
/// </summary>
public class BlogDto
{
    /// <summary>
    /// The name of the blog.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The author of the blog.
    /// </summary>
    public string Author { get; set; }
}
