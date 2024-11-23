namespace SplitQueries.Domain;

/// <summary>
/// Represents a blog entity in the system.
/// Each instance corresponds to a single record in the Blogs table of the database.
/// </summary>
public class Blog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Blog"/> class.
    /// By default, the <see cref="Posts"/> collection is initialized as an empty set.
    /// </summary>
    public Blog()
    {
        Posts = new HashSet<Post>();
    }

    /// <summary>
    /// Gets or sets the unique identifier for the blog.
    /// This serves as the primary key in the Blogs table of the database.
    /// </summary>
    public int BlogId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the blog.
    /// Represents the web address where the blog is hosted.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rating of the blog.
    /// Indicates a score or ranking (e.g., user feedback, quality, or popularity).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the collection of posts associated with this blog.
    /// Each blog can have multiple posts, represented as a one-to-many relationship.
    /// </summary>
    public ICollection<Post> Posts { get; set; }
}
