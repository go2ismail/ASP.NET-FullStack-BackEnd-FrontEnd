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



public class UpdateCurrencyResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateCurrencyRequest : IRequest<UpdateCurrencyResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string Symbol { get; init; } = null!;
    public string? Description { get; init; }
}

public class UpdateCurrencyValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.Symbol)
            .NotEmpty();
    }
}


public class UpdateCurrencyHandler : IRequestHandler<UpdateCurrencyRequest, UpdateCurrencyResult>
{
    private readonly IBaseCommandRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCurrencyHandler(
        IBaseCommandRepository<Currency> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCurrencyResult> Handle(UpdateCurrencyRequest request, CancellationToken cancellationToken)
    {

        var entity = await _repository.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Update(
                request.UserId,
                request.Name,
                request.Symbol,
                request.Description
            );

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateCurrencyResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


