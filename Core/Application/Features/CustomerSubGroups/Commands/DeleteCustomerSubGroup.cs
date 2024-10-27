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



public class DeleteCustomerSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteCustomerSubGroupRequest : IRequest<DeleteCustomerSubGroupResult>
{
    public string? UserId { get; init; }
    public string Id { get; init; } = null!;
    public string CustomerGroupId { get; init; } = null!;
}

public class DeleteCustomerSubGroupValidator : AbstractValidator<DeleteCustomerSubGroupRequest>
{
    public DeleteCustomerSubGroupValidator()
    {
        RuleFor(x => x.CustomerGroupId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteCustomerSubGroupHandler : IRequestHandler<DeleteCustomerSubGroupRequest, DeleteCustomerSubGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerSubGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCustomerSubGroupResult> Handle(DeleteCustomerSubGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerSubGroups)
            .Where(x => x.Id == request.CustomerGroupId);


        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.CustomerGroupId}");
        }

        entity.DeleteSubGroup(
            request.UserId,
            request.Id);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteCustomerSubGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


