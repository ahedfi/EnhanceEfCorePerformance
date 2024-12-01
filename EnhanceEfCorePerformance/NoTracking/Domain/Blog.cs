namespace NoTracking.Domain;

/// <summary>
/// Represents a blog entity with details about the blog, including its name, author, creation date, and description.
/// </summary>
public class Blog
{
    /// <summary>
    /// Gets or sets the unique identifier for the blog.
    /// This serves as the primary key in the database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the blog.
    /// This is the title or main identifier for the blog.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the author of the blog.
    /// This indicates who created or maintains the blog.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the blog.
    /// This stores the date and time when the blog was first created.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the description of the blog.
    /// Provides a summary or additional information about the blog's content or purpose.
    /// </summary>
    public string Description { get; set; }
}
