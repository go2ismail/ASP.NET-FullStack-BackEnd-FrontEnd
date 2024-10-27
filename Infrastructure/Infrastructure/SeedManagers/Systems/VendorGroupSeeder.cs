// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManagers.Systems;

public class VendorGroupSeeder
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public VendorGroupSeeder(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var currencies = new List<(string Name, string Description)>
            {
                ("Default", ""),
                ("Factory", ""),
                ("Distributor", ""),
                ("Wholesaler", ""),
                ("Retailer", "")
            };

        foreach (var (name, description) in currencies)
        {
            var existingVendorGroup = await _repository
                .GetQuery()
                .ApplyIsDeletedFilter()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync();

            if (existingVendorGroup == null)
            {
                var newVendorGroup = new VendorGroup(
                    null,
                    name,
                    description);

                await _repository.CreateAsync(newVendorGroup);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}