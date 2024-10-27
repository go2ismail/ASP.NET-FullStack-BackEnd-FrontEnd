// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerSubGroups.Commands;



public class CreateCustomerSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateCustomerSubGroupRequest : IRequest<CreateCustomerSubGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string CustomerGroupId { get; set; } = null!;
    public string? Description { get; init; }
}

public class CreateCustomerSubGroupValidator : AbstractValidator<CreateCustomerSubGroupRequest>
{
    public CreateCustomerSubGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.CustomerGroupId)
            .NotEmpty();
    }
}


public class CreateCustomerSubGroupHandler : IRequestHandler<CreateCustomerSubGroupRequest, CreateCustomerSubGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerSubGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerSubGroupResult> Handle(CreateCustomerSubGroupRequest request, CancellationToken cancellationToken = default)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerSubGroups.Where(y => y.IsDeleted == false))
            .Where(x => x.Id == request.CustomerGroupId);


        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.CustomerGroupId}");
        }

        var result = entity.CreateSubGroup(
            request.UserId,
            request.Name,
            request.Description);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateCustomerSubGroupResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}





