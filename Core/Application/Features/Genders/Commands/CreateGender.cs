// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Genders.Commands;



public class CreateGenderResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateGenderRequest : IRequest<CreateGenderResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class CreateGenderValidator : AbstractValidator<CreateGenderRequest>
{
    public CreateGenderValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class CreateGenderHandler : IRequestHandler<CreateGenderRequest, CreateGenderResult>
{
    private readonly IBaseCommandRepository<Gender> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenderHandler(
        IBaseCommandRepository<Gender> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateGenderResult> Handle(CreateGenderRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Gender(
                request.UserId,
                request.Name,
                request.Description
                );

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateGenderResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


