// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.CustomerGroups.Commands;


public class UpdateCustomerGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateCustomerGroupRequest : IRequest<UpdateCustomerGroupResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class UpdateCustomerGroupValidator : AbstractValidator<UpdateCustomerGroupRequest>
{
    public UpdateCustomerGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class UpdateCustomerGroupHandler : IRequestHandler<UpdateCustomerGroupRequest, UpdateCustomerGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerGroupResult> Handle(UpdateCustomerGroupRequest request, CancellationToken cancellationToken)
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

        return new UpdateCustomerGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


