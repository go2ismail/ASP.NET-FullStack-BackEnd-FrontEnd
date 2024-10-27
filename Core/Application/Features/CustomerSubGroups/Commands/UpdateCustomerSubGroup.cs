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





public class UpdateCustomerSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateCustomerSubGroupRequest : IRequest<UpdateCustomerSubGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string CustomerGroupId { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string? Description { get; init; }
}

public class UpdateCustomerSubGroupValidator : AbstractValidator<UpdateCustomerSubGroupRequest>
{
    public UpdateCustomerSubGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.CustomerGroupId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class UpdateCustomerSubGroupHandler : IRequestHandler<UpdateCustomerSubGroupRequest, UpdateCustomerSubGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerSubGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerSubGroupResult> Handle(UpdateCustomerSubGroupRequest request, CancellationToken cancellationToken = default)
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

        var result = entity.UpdateSubGroup(
            request.UserId,
            request.Id,
            request.Name,
            request.Description);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateCustomerSubGroupResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}





