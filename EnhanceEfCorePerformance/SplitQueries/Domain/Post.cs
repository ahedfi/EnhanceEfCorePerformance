namespace SplitQueries.Domain;

/// <summary>
/// Represents a post entity in the system.
/// Each instance corresponds to a single record in the Posts table of the database.
/// </summary>
public class Post
{
    /// <summary>
    /// Gets or sets the unique identifier for the post.
    /// This serves as the primary key in the Posts table of the database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the post.
    /// Represents the main heading or subject of the post.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the post.
    /// Contains the body or text of the post.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the associated blog.
    /// Represents a foreign key linking the post to a specific blog.
    /// </summary>
    public int BlogId { get; set; }

    /// <summary>
    /// Gets or sets the associated <see cref="Blog"/> entity.
    /// Represents the navigation property for the relationship between a post and its blog.
    /// </summary>
    public Blog Blog { get; set; }
}
