// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Infrastructure.DataAccessManagers.EFCores.Contexts;

namespace Infrastructure.DataAccessManagers.EFCores.Repositories;

public class UnitOfWork(CommandContext context) : IUnitOfWork
{
    private readonly CommandContext _context = context;

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
