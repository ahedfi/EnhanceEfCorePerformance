using Caching.Persistence;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Caching;

/// <summary>
/// Provides a singleton service provider for managing EF Core DbContext and caching.
/// </summary>
public static class EFServiceProvider
{
    // Connection string to the SQL Server database.
    private const string ConnectionString =
       @"Server=localhost,1433;Database=Blogging;User Id=sa;Password=Aa@123456;TrustServerCertificate=True";

    /// <summary>
    /// The logger factory used to log SQL queries and other EF Core operations to the console.
    /// </summary>
    private static readonly ILoggerFactory LoggerFactory =
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // Logs output to the console.
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information); // Logs SQL queries at the Information level.
            });

    // Lazy-loaded singleton for IServiceProvider to ensure thread safety and one-time initialization.
    private static readonly Lazy<IServiceProvider> _serviceProviderBuilder =
            new Lazy<IServiceProvider>(GetServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Exposes the singleton IServiceProvider instance.
    /// </summary>
    public static IServiceProvider Instance { get; } = _serviceProviderBuilder.Value;

    /// <summary>
    /// Resolves a required service from the service provider.
    /// </summary>
    /// <typeparam name="T">Type of the service to resolve.</typeparam>
    public static T GetRequiredService<T>()
    {
        return Instance.GetRequiredService<T>();
    }

    /// <summary>
    /// Executes an action within a DbContext scope.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void RunInContext(Action<BloggingDbContext> action)
    {
        using var serviceScope = GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<BloggingDbContext>();
        action(context);
    }

    /// <summary>
    /// Executes an asynchronous function within a DbContext scope.
    /// </summary>
    /// <param name="action">The asynchronous function to execute.</param>
    public static async Task RunInContextAsync(Func<BloggingDbContext, Task> action)
    {
        using var serviceScope = GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<BloggingDbContext>();
        await action(context);
    }

    /// <summary>
    /// Configures and builds the service provider, registering DbContext, logging, and caching.
    /// </summary>
    /// <returns>The configured IServiceProvider instance.</returns>
    private static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        // Register the DbContext with SQL Server configuration and logging.
        services.AddDbContext<BloggingDbContext>((serviceProvider, options) =>
            options.UseSqlServer(ConnectionString)
                   .UseLoggerFactory(LoggerFactory) // Log EF Core operations.
                   .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())); // Add second-level cache.

        // Configure EF Core second-level cache with in-memory provider and logging.
        services.AddEFSecondLevelCache(options =>
            options.UseMemoryCacheProvider() // Use in-memory caching for performance improvement.
                   .ConfigureLogging(true)); // Enable logging for caching operations.

        // Register logging services with minimum log level set to Debug.
        services.AddLogging(config =>
        {
            config.AddConsole()
                  .SetMinimumLevel(LogLevel.Debug);
        });

        // Build and return the service provider.
        return services.BuildServiceProvider();
    }
}
