using CompiledQueries.Domain;
using Microsoft.EntityFrameworkCore;

namespace CompiledQueries.Persistence;

/// <summary>
/// Represents the Entity Framework Core DbContext for the Blogging database.
/// Provides access to the Blogs table and includes a method for seeding test data.
/// </summary>
public class BloggingDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the Blogs table in the database.
    /// This is a collection of all the blogs in the system.
    /// </summary>
    public DbSet<Blog> Blogs { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloggingDbContext"/> class with the provided options.
    /// The options include the connection string and any other configuration for the DbContext.
    /// </summary>
    /// <param name="options">The DbContextOptions containing configuration for the DbContext.</param>
    public BloggingDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Seeds the database with a specified number of blog records.
    /// This is useful for populating the database with test data before benchmarks or other operations.
    /// </summary>
    /// <param name="numBlogs">The number of blogs to seed in the database.</param>
    public void SeedData(int numBlogs)
    {
        // Add 'numBlogs' blog entries to the Blogs DbSet.
        Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(i => new Blog
        {
            Url = $"http://www.someblog{i}.com", // Create a blog URL based on the index
            Rating = new Random().Next(0, 10)
        }));

        // Save the changes to the database to persist the seeded blogs
        SaveChanges();
    }
}
