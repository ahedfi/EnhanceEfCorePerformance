using DbContextPooling.Domain;
using DbContextPooling.Persistence;

namespace DbContextPooling.Controllers;

/// <summary>
/// Represents a simple controller for handling operations on the Blogs table.
/// Provides methods to interact with the database context and retrieve blog data.
/// </summary>
public class BlogController
{
    /// <summary>
    /// The database context used to interact with the Blogs table.
    /// </summary>
    private readonly BloggingDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlogController"/> class with the specified database context.
    /// </summary>
    /// <param name="context">An instance of <see cref="BloggingDbContext"/> for database operations.</param>
    public BlogController(BloggingDbContext context) => _context = context;

    /// <summary>
    /// Retrieves the first blog entry from the Blogs table.
    /// </summary>
    /// <returns>
    /// A <see cref="Blog"/> object representing the first entry in the Blogs table.
    /// If the table is empty, an exception will be thrown.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Blogs table is empty when <see cref="Queryable.First()"/> is called.
    /// </exception>
    public Blog Action() => _context.Blogs.First();
}

