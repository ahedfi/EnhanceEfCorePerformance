using CustomMethods.Persistence;
using CustomMethods.Repositories; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging; 

namespace CustomMethods;

internal class Program
{
    /// <summary>
    /// Connection string for the SQL Server database.
    /// Configures the connection to a local SQL Server instance, such as one running in a Docker container.
    /// </summary>
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// The logger factory used to log SQL queries and other EF Core operations to the console.
    /// Useful for debugging and understanding how EF Core interacts with the database.
    /// </summary>
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            // Adds logging to the console for all EF Core operations.
            builder.AddConsole();

            // Filters the logs to include only SQL queries and commands at the Information level.
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
        });

    /// <summary>
    /// The entry point of the program. Demonstrates initializing the database, seeding data,
    /// and using various filtering methods in the BlogRepository.
    /// </summary>
    /// <param name="args">Command-line arguments (not used in this example).</param>
    static void Main(string[] args)
    {
        // Initialize the DbContext with SQL Server connection and logging configuration.
        var dbContext = new BloggingDbContext(new DbContextOptionsBuilder<BloggingDbContext>()
            .UseSqlServer(ConnectionString) // Configures the DbContext to use SQL Server.
            .UseLoggerFactory(LoggerFactory) // Adds logging for EF Core operations.
            .Options);

        // Seed the database with 100 sample blog entries.
        dbContext.SeedData(100);

        // Initialize the repository for interacting with the Blogs table.
        var repository = new BlogRepository(dbContext);

        // Uncomment the following line to see an exception in runtime when using a custom method
        // that cannot be translated to SQL.
        // repository.FilterBlogsUsingCustomMethod("EF CORE");

        // Filters blogs using client-side evaluation with a custom method.
        repository.FilterBlogsUsingClientSideEvaluation("EF CORE");

        // Use an expression tree to filter blogs by keyword. Executes on the database server.
        repository.FilterBlogsUsingExpressionTree("EF CORE");

        // Use a raw SQL query to filter blogs by keyword. Executes on the database server.
        repository.FilterBlogsUsingFromSql("EF CORE");
    }
}
