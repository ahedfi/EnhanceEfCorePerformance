namespace DbContextPooling.Domain;

/// <summary>
/// Represents a blog entity in the system.
/// Each instance corresponds to a single record in the Blogs table of the database.
/// </summary>
public class Blog
{
    /// <summary>
    /// Gets or sets the unique identifier for the blog.
    /// This is the primary key of the Blogs table.
    /// </summary>
    public int BlogId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the blog.
    /// This property stores the web address of the blog.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rating of the blog.
    /// This can be used to store a score or ranking for the blog (e.g., user feedback or quality score).
    /// </summary>
    public int Rating { get; set; }
}

