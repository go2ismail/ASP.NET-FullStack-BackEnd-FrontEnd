// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Application.Services.CQS.Queries;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyIsDeletedFilter<T>(this IQueryable<T> query, bool isDeleted = false) where T : class
    {
        if (typeof(IHasIsDeleted).IsAssignableFrom(typeof(T)))
        {
            query = query.Where(x => (x as IHasIsDeleted)!.IsDeleted == isDeleted);
        }

        return query;
    }

    public static IQueryable<T> ApplyODataFilterWithPaging<T>(
        this IQueryable<T> query,
        IODataQueryOptions<T> queryOptions,
        out int totalRecords,
        out int skip,
        out int top
    ) where T : class
    {
        totalRecords = queryOptions.GetTotalRecords(query);

        query = queryOptions.ApplyOData(query);

        skip = queryOptions.GetSkip();

        top = queryOptions.GetTop();

        if (top == 0)
        {
            throw new ApplicationException("Top must not be zero.");
        }

        return query;
    }

    public static IQueryable<T> ApplyODataSorting<T>(
        this IQueryable<T> query,
        IODataQueryOptions<T> queryOptions
    ) where T : class
    {

        query = queryOptions.ApplySorting(query);

        return query;
    }

    public static IQueryable<T> ApplyODataFilter<T>(
        this IQueryable<T> query,
        IODataQueryOptions<T> queryOptions
    ) where T : class
    {

        query = queryOptions.ApplyFilter(query);

        return query;
    }

    public static IQueryable<T> ApplyODataPaging<T>(
        this IQueryable<T> query,
        IODataQueryOptions<T> queryOptions,
        out int skip,
        out int top
    ) where T : class
    {

        query = queryOptions.ApplyPaging(query);

        skip = queryOptions.GetSkip();

        top = queryOptions.GetTop();

        if (top == 0)
        {
            throw new ApplicationException("Top must not be zero.");
        }

        return query;
    }

    public static IQueryable<T> ApplyRelationFilter<T, TNavigation>(this IQueryable<T> query, string queryValue, string navigationPropertyName, IQueryContext context)
         where T : class
         where TNavigation : class
    {
        var navigationSet = context.Set<TNavigation>().AsNoTracking();

        // Retrieve IDs based on the navigation entity type
        var ids = navigationSet
            .Where(e => EF.Property<string>(e, "Name").Contains(queryValue))
            .Select(e => EF.Property<string>(e, "Id"))
            .ToList();

        // Build the predicate dynamically
        var parameter = Expression.Parameter(typeof(T), "x");
        var navigationProperty = Expression.Property(parameter, navigationPropertyName);
        var containsMethod = typeof(List<string>).GetMethod("Contains", [typeof(string)]) ?? throw new ApplicationException($"Method 'Contains' not found on List<string>");
        var containsExpression = Expression.Call(Expression.Constant(ids), containsMethod, navigationProperty);
        var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

        return query.Where(lambda);
    }
}
