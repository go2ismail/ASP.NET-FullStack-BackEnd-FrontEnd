// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Configs.Commands;


public class DeleteConfigResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteConfigRequest : IRequest<DeleteConfigResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteConfigValidator : AbstractValidator<DeleteConfigRequest>
{
    public DeleteConfigValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteConfigHandler(
    IBaseCommandRepository<Config> repository,
    IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteConfigRequest, DeleteConfigResult>
{
    private readonly IBaseCommandRepository<Config> _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<DeleteConfigResult> Handle(DeleteConfigRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync(request.Id, cancellationToken) ?? throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteConfigResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


