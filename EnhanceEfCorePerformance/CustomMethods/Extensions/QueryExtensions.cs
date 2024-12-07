using Microsoft.EntityFrameworkCore; 
using System.Linq.Expressions;

namespace CustomMethods.Extensions;

/// <summary>
/// Provides extension methods for creating dynamic query expressions.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// Creates a dynamic LINQ expression to perform a case-sensitive "contains" check 
    /// on a specified property using EF.Functions.Like.
    /// </summary>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <param name="propertyName">The name of the property to search against.</param>
    /// <param name="keyword">The keyword to check for in the property's value.</param>
    /// <returns>An expression that can be used in LINQ queries, such as x => x.Property != null && EF.Functions.Like(x.Property, '%keyword%').</returns>
    public static Expression<Func<T, bool>> CreateContainsKeywordExpression<T>(string propertyName, string keyword)
    {
        // Parameter for the lambda expression, e.g., 'x' in 'x => ...'.
        var parameter = Expression.Parameter(typeof(T), "x");

        // Access the specified property on the parameter, e.g., 'x.Property'.
        var property = Expression.Property(parameter, propertyName);

        // Ensure the property's value is not null, e.g., 'x.Property != null'.
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

        // Get the EF.Functions.Like method information.
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
            new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        // Create a constant for EF.Functions, required as the first argument to EF.Functions.Like.
        var efFunctions = Expression.Constant(EF.Functions);

        // Create a constant for the keyword pattern, e.g., '%keyword%'.
        var keywordPattern = Expression.Constant($"%{keyword}%", typeof(string));

        // Call the EF.Functions.Like method, e.g., EF.Functions.Like(x.Property, '%keyword%').
        var likeCall = Expression.Call(likeMethod, efFunctions, property, keywordPattern);

        // Combine the null check and the Like call using an AND condition.
        var finalCondition = Expression.AndAlso(notNullCheck, likeCall);

        // Return the final expression, e.g., x => x.Property != null && EF.Functions.Like(x.Property, '%keyword%').
        return Expression.Lambda<Func<T, bool>>(finalCondition, parameter);
    }
}
