using Caching.Persistence;
using EFCoreSecondLevelCacheInterceptor;

namespace Caching
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize the database and seed it with sample data.
            InitDb();

            // Example 1: Fetch all blogs from the database, then cache the results.
            EFServiceProvider.RunInContext(context =>
            {
                var blogs = context.Blogs.Cacheable().ToList(); // Executes the query and caches the result.
                Console.WriteLine($"Name From DB: {blogs.First().Name}"); // Outputs the name of the first blog fetched from the database.
            });

            // Example 2: Fetch blogs from the cache (result of the previous query).
            EFServiceProvider.RunInContext(context =>
            {
                var blogs = context.Blogs.Cacheable().ToList(); // Retrieves the cached result.
                Console.WriteLine($"Name From Cache: {blogs.First().Name}"); // Outputs the name of the first blog fetched from the cache.
            });

            // Example 3: Filter blogs by a condition, fetch results from the database, and cache them.
            EFServiceProvider.RunInContext(context =>
            {
                var blogs = context.Blogs.Where(blog => blog.Author.Contains("Author")).Cacheable().ToList();
                // Filters blogs by author name containing "Author" and caches the results.
                Console.WriteLine($"Name From DB: {blogs.First().Name}"); // Outputs the name of the first blog from the database.
            });

            // Example 4: Fetch filtered results from the cache (result of the previous query).
            EFServiceProvider.RunInContext(context =>
            {
                var blogs = context.Blogs.Where(blog => blog.Author.Contains("Author")).Cacheable().ToList();
                // Retrieves the cached results for the filtered query.
                Console.WriteLine($"Name From Cache: {blogs.First().Name}"); // Outputs the name of the first blog fetched from the cache.
            });
        }

        /// <summary>
        /// Initializes the database with seed data.
        /// </summary>
        private static void InitDb()
        {
            var dbContext = EFServiceProvider.GetRequiredService<BloggingDbContext>(); // Gets the database context.
            dbContext.SeedData(10); // Seeds the database with 10 sample blog entries.
        }
    }
}
