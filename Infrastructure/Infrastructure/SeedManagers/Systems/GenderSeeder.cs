// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManagers.Systems;


public class GenderSeeder
{
    private readonly IBaseCommandRepository<Gender> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GenderSeeder(
        IBaseCommandRepository<Gender> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var data = new List<(string Name, string Description)>
            {
                ("Male", ""),
                ("Female", "")
            };

        foreach (var (name, description) in data)
        {
            var existingGender = await _repository
                .GetQuery()
                .ApplyIsDeletedFilter()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync();

            if (existingGender == null)
            {
                var newGender = new Gender(
                    null,
                    name,
                    description);

                await _repository.CreateAsync(newGender);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
