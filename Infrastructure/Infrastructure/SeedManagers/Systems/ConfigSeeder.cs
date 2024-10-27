// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManagers.Systems;

public class ConfigSeeder
{
    private readonly IBaseCommandRepository<Config> _config;
    private readonly IBaseCommandRepository<Currency> _currency;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;

    public ConfigSeeder(
        IBaseCommandRepository<Config> config,
        IBaseCommandRepository<Currency> currency,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService)
    {
        _config = config;
        _currency = currency;
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
    }

    public async Task GenerateDataAsync()
    {
        var usDollar = await _currency
            .GetQuery()
            .ApplyIsDeletedFilter()
            .Where(x => x.Name == "US Dollar")
            .SingleOrDefaultAsync();

        var environments = new List<string>
        {
            "Production",
            "Staging",
            "Development"
        };

        var index = 1;
        foreach (var environment in environments)
        {

            if (usDollar != null)
            {
                var entity = new Config(
                    null,
                    environment,
                    null,
                    usDollar.Id,
                    "smtp.gmail.com",
                    465,
                    "dummy_username",
                    _encryptionService.Encrypt("dummy_password"),
                    false,
                    index == 1 ? true : false
                    );

                await _config.CreateAsync(entity);
                index++;
            }

        }


        await _unitOfWork.SaveAsync();

    }
}
