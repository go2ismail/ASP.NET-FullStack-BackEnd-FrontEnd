// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Configs.Commands;



public class CreateConfigResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateConfigRequest : IRequest<CreateConfigResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CurrencyId { get; init; } = null!;
    public string SmtpHost { get; init; } = null!;
    public int SmtpPort { get; init; }
    public string SmtpUserName { get; init; } = null!;
    public string SmtpPassword { get; init; } = null!;
    public bool SmtpUseSSL { get; init; }
    public bool Active { get; init; }
}

public class CreateConfigValidator : AbstractValidator<CreateConfigRequest>
{
    public CreateConfigValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.CurrencyId)
            .NotEmpty();

        RuleFor(x => x.SmtpHost)
            .NotEmpty();

        RuleFor(x => x.SmtpPort)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SMTP Port must be greater than or equal to 0");

        RuleFor(x => x.SmtpUserName)
            .NotEmpty();

        RuleFor(x => x.SmtpPassword)
            .NotEmpty();
    }
}


public class CreateConfigHandler(
    IBaseCommandRepository<Config> repository,
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService
        ) : IRequestHandler<CreateConfigRequest, CreateConfigResult>
{
    private readonly IBaseCommandRepository<Config> _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<CreateConfigResult> Handle(CreateConfigRequest request, CancellationToken cancellationToken = default)
    {
        //ensure have only one: active
        if (request.Active == true)
        {
            var configs = _repository
                .GetQuery()
                .ApplyIsDeletedFilter()
                .ToList();

            foreach (var item in configs)
            {
                item.Active = false;
                item.Update(
                    null,
                    item.Name,
                    item.Description,
                    item.CurrencyId,
                    item.SmtpHost,
                    item.SmtpPort,
                    item.SmtpUserName,
                    null,
                    item.SmtpUseSSL,
                    item.Active
                );
                _repository.Update(item);
            }
        }


        var entity = new Config(
                request.UserId,
                request.Name,
                request.Description,
                request.CurrencyId,
                request.SmtpHost,
                request.SmtpPort,
                request.SmtpUserName,
                _encryptionService.Encrypt(request.SmtpPassword),
                request.SmtpUseSSL,
                request.Active
                );

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateConfigResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


