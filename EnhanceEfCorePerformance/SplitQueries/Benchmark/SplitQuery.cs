using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SplitQueries.Persistence;

namespace SplitQueries.Benchmark
{
    /// <summary>
    /// A benchmark class designed to evaluate the performance of Entity Framework Core operations, 
    /// comparing the use of split queries against regular LINQ queries for optimized data retrieval.
    /// </summary>
    [MemoryDiagnoser] // Enables memory usage diagnostics during benchmark execution.
    public class SplitQuery
    {
        /// <summary>
        /// The connection string for the SQL Server database.
        /// Assumes a local SQL Server instance, such as one running in a Docker container.
        /// </summary>
        private const string ConnectionString =
            @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

        /// <summary>
        /// The logger factory used to log SQL queries and other EF Core operations to the console.
        /// </summary>
        private static readonly ILoggerFactory LoggerFactory =
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                {
                    builder.AddConsole(); // Logs output to the console
                    builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information); // Logs SQL queries at the Information level
                });

        /// <summary>
        /// The service provider used for dependency injection, providing access to the DbContext.
        /// </summary>
        private IServiceProvider _serviceProvider;

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
            services.AddDbContext<BloggingDbContext>(c => c.UseSqlServer(ConnectionString)
                                                           .EnableSensitiveDataLogging() // Optional: Includes parameter values in logs
                                                           .UseLoggerFactory(LoggerFactory)); // Attach the logger factory for logging SQL queries

            // Build the service provider.
            _serviceProvider = services.BuildServiceProvider();

            // Initialize and seed the database with the specified number of blogs.
            using var context = new BloggingDbContext(options);
            context.Database.EnsureDeleted(); // Clean up any existing database to ensure a fresh start.
            context.Database.EnsureCreated(); // Create the database schema from the model.
            context.SeedData(NumBlogs);      // Seed the database with the specified number of blogs and posts.
        }

        /// <summary>
        /// Benchmark method that retrieves blogs and their posts without using split queries.
        /// Uses a standard LINQ query to perform the data retrieval in a single query.
        /// </summary>
        [Benchmark]
        public void WithoutSplitQuery()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

            // Executes a standard query to retrieve blogs and their associated posts in one query.
            var query = dbContext.Blogs.Include(b => b.Posts)
                                        .ToList();
        }

        /// <summary>
        /// Benchmark method that retrieves blogs and their posts using split queries.
        /// This method retrieves the main entities and related data in separate queries.
        /// </summary>
        [Benchmark]
        public void WithSplitQuery()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BloggingDbContext>();

            // Executes a query to retrieve blogs and their associated posts using split queries.
            var query = dbContext.Blogs.Include(b => b.Posts)
                                        .AsSplitQuery() // Ensures that posts are loaded in a separate query to optimize performance.
                                        .ToList();
        }
    }
}
