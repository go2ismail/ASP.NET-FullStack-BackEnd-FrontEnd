// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessManagers.EFCores.Contexts;

public class QueryContext(DbContextOptions<DataContext> options) : DataContext(options), IQueryContext
{
    public new IQueryable<T> Set<T>() where T : class
    {
        return base.Set<T>();
    }
}
