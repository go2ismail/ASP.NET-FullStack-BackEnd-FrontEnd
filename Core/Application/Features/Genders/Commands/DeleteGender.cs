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




public class DeleteGenderResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteGenderRequest : IRequest<DeleteGenderResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteGenderValidator : AbstractValidator<DeleteGenderRequest>
{
    public DeleteGenderValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteGenderHandler : IRequestHandler<DeleteGenderRequest, DeleteGenderResult>
{
    private readonly IBaseCommandRepository<Gender> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGenderHandler(
        IBaseCommandRepository<Gender> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteGenderResult> Handle(DeleteGenderRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteGenderResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


