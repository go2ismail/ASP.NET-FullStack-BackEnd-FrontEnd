// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.CustomerGroups.Commands;



public class CreateCustomerGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateCustomerGroupRequest : IRequest<CreateCustomerGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class CreateCustomerGroupValidator : AbstractValidator<CreateCustomerGroupRequest>
{
    public CreateCustomerGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class CreateCustomerGroupHandler : IRequestHandler<CreateCustomerGroupRequest, CreateCustomerGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerGroupResult> Handle(CreateCustomerGroupRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CustomerGroup(
                request.UserId,
                request.Name,
                request.Description
                );

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateCustomerGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


