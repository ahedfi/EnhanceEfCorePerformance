using CustomMethods.Domain;
using CustomMethods.Extensions; 
using CustomMethods.Persistence; 
using Microsoft.EntityFrameworkCore; 

namespace CustomMethods.Repositories;

/// <summary>
/// Provides repository methods for querying blogs in the database.
/// Encapsulates logic for filtering blogs using various techniques.
/// </summary>
public class BlogRepository
{
    private readonly BloggingDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlogRepository"/> class with the specified DbContext.
    /// </summary>
    /// <param name="dbContext">The <see cref="BloggingDbContext"/> used for database access.</param>
    public BlogRepository(BloggingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Filters blogs using a custom method for keyword matching.
    /// The filtering occurs in memory and may not be efficient for large datasets.
    /// </summary>
    /// <param name="keyword">The keyword to search for in blog names.</param>
    /// <returns>A collection of blogs whose names contain the specified keyword.</returns>
    public IEnumerable<Blog> FilterBlogsUsingCustomMethod(string keyword)
    {
        // Performs filtering using the ContainsKeyword custom method.
        // Note: This filtering is not translatable to SQL and occurs in memory after fetching data.
        return _dbContext.Blogs
            .Where(blog => ContainsKeyword(blog.Name, keyword)) // Calls the custom method.
            .ToList(); // Materializes the results as a list.
    }

    /// <summary>
    /// Filters blogs using client-side evaluation with a custom method.
    /// This approach retrieves all blogs from the database and performs filtering in memory.
    /// </summary>
    /// <param name="keyword">The keyword to search for in the blog names.</param>
    /// <returns>
    /// A collection of blogs that contain the specified keyword in their names.
    /// Filtering is case-insensitive and performed in memory.
    /// </returns>
    /// <remarks>
    /// This method uses client-side evaluation, which can be inefficient for large datasets 
    /// because all records are loaded into memory before filtering. 
    /// It is recommended to use server-side filtering whenever possible.
    /// </remarks>
    public IEnumerable<Blog> FilterBlogsUsingClientSideEvaluation(string keyword)
    {
        // Fetches all blogs from the database and converts them to an in-memory collection.
        // AsEnumerable transitions the query from an IQueryable (deferred execution) to an IEnumerable (immediate execution).
        return _dbContext.Blogs
            .AsEnumerable()
            .Where(blog => ContainsKeyword(blog.Name, keyword)) // Invokes the custom ContainsKeyword method for filtering.
            .ToList(); // Materializes the filtered results as a list.
    }

    /// <summary>
    /// Filters blogs using a dynamically generated expression tree for keyword matching.
    /// The filtering is translatable to SQL and executed on the database server.
    /// </summary>
    /// <param name="keyword">The keyword to search for in blog names.</param>
    /// <returns>A collection of blogs whose names contain the specified keyword.</returns>
    public IEnumerable<Blog> FilterBlogsUsingExpressionTree(string keyword)
    {
        // Creates a dynamic expression for the 'Name' property with the given keyword.
        var expression = QueryExtensions.CreateContainsKeywordExpression<Blog>("Name", keyword);

        // Executes the query on the database server using the generated expression.
        return _dbContext.Blogs.Where(expression).ToList();
    }

    /// <summary>
    /// Filters blogs using raw SQL and parameterized queries.
    /// Useful for scenarios where LINQ is insufficient or complex SQL is required.
    /// </summary>
    /// <param name="keyword">The keyword to search for in blog names.</param>
    /// <returns>A collection of blogs whose names contain the specified keyword.</returns>
    public IEnumerable<Blog> FilterBlogsUsingFromSql(string keyword)
    {
        // Define the raw SQL query with a parameter placeholder.
        string sqlQuery = @"
                SELECT * FROM Blogs
                WHERE Name LIKE '%' + {0} + '%'
            ";

        // Executes the raw SQL query with the parameter safely injected.
        return _dbContext.Blogs
            .FromSqlRaw(sqlQuery, keyword) // Parameterized to avoid SQL injection.
            .ToList(); // Materializes the results as a list.
    }

    /// <summary>
    /// Custom method for performing case-insensitive keyword matching in strings.
    /// Used by <see cref="FilterBlogsUsingCustomMethod"/> for in-memory filtering.
    /// </summary>
    /// <param name="input">The input string to search within.</param>
    /// <param name="keyword">The keyword to search for.</param>
    /// <returns>True if the keyword is found in the input string; otherwise, false.</returns>
    private static bool ContainsKeyword(string input, string keyword)
    {
        // Checks if the input string contains the keyword, ignoring case.
        return input != null && input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
