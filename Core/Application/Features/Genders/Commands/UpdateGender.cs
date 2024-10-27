// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Genders.Commands;



public class UpdateGenderResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateGenderRequest : IRequest<UpdateGenderResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class UpdateGenderValidator : AbstractValidator<UpdateGenderRequest>
{
    public UpdateGenderValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class UpdateGenderHandler : IRequestHandler<UpdateGenderRequest, UpdateGenderResult>
{
    private readonly IBaseCommandRepository<Gender> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGenderHandler(
        IBaseCommandRepository<Gender> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateGenderResult> Handle(UpdateGenderRequest request, CancellationToken cancellationToken)
    {

        var entity = await _repository.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Update(
                request.UserId,
                request.Name,
                request.Description
            );

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateGenderResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


