// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;

using Infrastructure.DataAccessManagers.EFCores.Exceptions;

using Microsoft.AspNetCore.OData.Query;

namespace Infrastructure.DataAccessManagers.EFCores.ODatas;

public class ODataQueryOptionsAdapter<T>(ODataQueryOptions<T> odataQueryOptions) : IODataQueryOptions<T>
{
    private readonly ODataQueryOptions<T> _odataQueryOptions = odataQueryOptions;

    public IQueryable<T> ApplyOData(IQueryable<T> query)
    {
        if (_odataQueryOptions.Skip == null)
        {
            throw new ODataException($"Skip value must not be null {nameof(_odataQueryOptions.Skip)}.");
        }

        if (_odataQueryOptions.Top == null)
        {
            throw new ODataException($"Top value must not be null {nameof(_odataQueryOptions.Top)}.");
        }

        return (IQueryable<T>)_odataQueryOptions.ApplyTo(query);
    }

    public int GetTotalRecords(IQueryable<T> query)
    {
        if (_odataQueryOptions.Filter != null)
        {
            query = (IQueryable<T>)_odataQueryOptions.Filter.ApplyTo(query, new ODataQuerySettings());
        }

        return query.Count();
    }
    public int GetSkip()
    {
        return _odataQueryOptions.Skip?.Value ?? 0;
    }

    public int GetTop()
    {
        return _odataQueryOptions.Top?.Value ?? 0;
    }

    public string? GetOrderby()
    {
        return _odataQueryOptions.OrderBy?.RawValue;
    }

    public IQueryable<T> ApplySorting(IQueryable<T> query)
    {
        if (_odataQueryOptions.OrderBy != null)
        {

            return _odataQueryOptions.OrderBy.ApplyTo(query, new ODataQuerySettings());
        }

        return query;
    }

    public IQueryable<T> ApplyFilter(IQueryable<T> query)
    {
        if (_odataQueryOptions.Filter != null)
        {

            return (IQueryable<T>)_odataQueryOptions.Filter.ApplyTo(query, new ODataQuerySettings());
        }

        return query;
    }

    public IQueryable<T> ApplyPaging(IQueryable<T> query)
    {
        if (_odataQueryOptions.Skip == null)
        {
            throw new ODataException($"Skip value must not be null {nameof(_odataQueryOptions.Skip)}.");
        }

        query = _odataQueryOptions.Skip.ApplyTo(query, new ODataQuerySettings());

        if (_odataQueryOptions.Top == null)
        {
            throw new ODataException($"Top value must not be null {nameof(_odataQueryOptions.Top)}.");
        }

        query = _odataQueryOptions.Top.ApplyTo(query, new ODataQuerySettings());

        return query;
    }
}
