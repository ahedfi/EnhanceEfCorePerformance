using Microsoft.EntityFrameworkCore;
using CustomMethods.Domain;

namespace CustomMethods.Persistence;

/// <summary>
/// Represents the Entity Framework Core DbContext for managing the Blogging database.
/// Provides access to the Blogs table and includes a method for seeding test data.
/// </summary>
public class BloggingDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the collection of blogs in the system.
    /// Each <see cref="Blog"/> instance represents a record in the Blogs table.
    /// </summary>
    public DbSet<Blog> Blogs { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloggingDbContext"/> class with the specified options.
    /// Options configure the database connection and behavior of the DbContext.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions"/> containing configuration for the DbContext.</param>
    public BloggingDbContext(DbContextOptions options) : base(options)
    {
        // Constructor passes the provided options to the base DbContext class.
    }

    /// <summary>
    /// Seeds the Blogs table with a specified number of blog records.
    /// This method is useful for populating the database with initial or test data.
    /// </summary>
    /// <param name="numBlogs">The number of blogs to add to the database.</param>
    public void SeedData(int numBlogs)
    {
        // Ensure the database is in a clean state by deleting and recreating it.
        Database.EnsureDeleted(); // Deletes the database if it exists.
        Database.EnsureCreated(); // Recreates the database schema based on the current model.

        // Generate and add sample blog entries to the Blogs table.
        Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(i => new Blog
        {
            Name = $"Blog Name {i}", // Sets a unique name for each blog.
            Author = $"Author {i}", // Assigns a unique author name for each blog.
            CreationDate = DateTime.Now.AddDays(-i), // Stagger creation dates to simulate historical data.
            Description = $"This is a description for blog {i}.", // Adds a descriptive summary for each blog.
        }));

        // Save all changes to the database.
        SaveChanges();
    }
}
