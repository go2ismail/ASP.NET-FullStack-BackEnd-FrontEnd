// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Currencies.Commands;




public class DeleteCurrencyResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteCurrencyRequest : IRequest<DeleteCurrencyResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteCurrencyValidator : AbstractValidator<DeleteCurrencyRequest>
{
    public DeleteCurrencyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteCurrencyHandler : IRequestHandler<DeleteCurrencyRequest, DeleteCurrencyResult>
{
    private readonly IBaseCommandRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCurrencyHandler(
        IBaseCommandRepository<Currency> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCurrencyResult> Handle(DeleteCurrencyRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteCurrencyResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


