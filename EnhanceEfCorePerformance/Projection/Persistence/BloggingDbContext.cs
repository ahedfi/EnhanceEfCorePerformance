using Projection.Domain;
using Microsoft.EntityFrameworkCore;

namespace Projection.Persistence;

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
    public BloggingDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Seeds the Blogs table with a specified number of blog records.
    /// This method is useful for populating the database with initial or test data.
    /// </summary>
    /// <param name="numBlogs">The number of blogs to add to the database.</param>
    public void SeedData(int numBlogs)
    {
        // Generate a list of blogs with sample data.
        Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(i => new Blog
        {
            Name = $"Blog Name {i}", // Assign a unique name to each blog.
            Author = $"Author {i}", // Assign a unique author to each blog.
            CreationDate = DateTime.Now.AddDays(-i), // Set a creation date in descending order.
            Description = $"This is a description for blog {i}.", // Add a description for the blog.
        }));

        // Persist the changes to the database.
        SaveChanges();
    }
}
