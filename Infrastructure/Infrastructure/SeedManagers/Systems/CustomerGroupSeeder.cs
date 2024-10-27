// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManagers.Systems;

public class CustomerGroupSeeder
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerGroupSeeder(
        IBaseCommandRepository<CustomerGroup> repository,
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
                ("B2B", "Business to Business"),
                ("B2C", "Business to Customer"),
                ("B2G", "Business to Government")
            };

        foreach (var (name, description) in currencies)
        {
            var existingCustomerGroup = await _repository
                .GetQuery()
                .ApplyIsDeletedFilter()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync();

            if (existingCustomerGroup == null)
            {
                var newCustomerGroup = new CustomerGroup(
                    null,
                    name,
                    description);

                await _repository.CreateAsync(newCustomerGroup);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}