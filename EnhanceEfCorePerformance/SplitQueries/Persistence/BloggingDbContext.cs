using Microsoft.EntityFrameworkCore;
using SplitQueries.Domain;

namespace SplitQueries.Persistence
{
    /// <summary>
    /// Represents the Entity Framework Core DbContext for the Blogging database.
    /// Provides access to the Blogs and Posts tables and includes functionality to seed test data.
    /// </summary>
    public class BloggingDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the Blogs table in the database.
        /// Represents a collection of all blogs in the system.
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }

        /// <summary>
        /// Gets or sets the Posts table in the database.
        /// Represents a collection of all posts associated with blogs.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloggingDbContext"/> class with the provided options.
        /// The options typically include the database connection string and other configurations.
        /// </summary>
        /// <param name="options">The DbContextOptions containing configuration settings for the DbContext.</param>
        public BloggingDbContext(DbContextOptions options) : base(options) { }

        /// <summary>
        /// Seeds the database with a specified number of blogs, each containing a collection of posts.
        /// This method is useful for populating the database with sample data for testing or benchmarking purposes.
        /// </summary>
        /// <param name="numBlogs">The number of blog records to add to the database.</param>
        public void SeedData(int numBlogs)
        {
            // Add a range of blog entries to the Blogs table
            Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(blogIndex => new Blog
            {
                Url = $"http://www.someblog{blogIndex}.com", // Generate a unique URL for each blog
                Rating = new Random().Next(0, 10), // Assign a random rating between 0 and 9
                Posts = Enumerable.Range(0, 9).Select(postIndex => new Post
                {
                    Title = $"Title {postIndex}", // Generate a unique title for each post
                    Content = $"This is the content of post {postIndex} for blog {blogIndex}.", // Example content for the post
                }).ToList() // Convert the sequence of posts into a List<Post>
            }));

            // Save the changes to persist the seeded data in the database
            SaveChanges();
        }
    }
}
