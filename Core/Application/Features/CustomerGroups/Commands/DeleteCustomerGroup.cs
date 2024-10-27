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

namespace Application.Features.CustomerGroups.Commands;



public class DeleteCustomerGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteCustomerGroupRequest : IRequest<DeleteCustomerGroupResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteCustomerGroupValidator : AbstractValidator<DeleteCustomerGroupRequest>
{
    public DeleteCustomerGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteCustomerGroupHandler : IRequestHandler<DeleteCustomerGroupRequest, DeleteCustomerGroupResult>
{
    private readonly IBaseCommandRepository<CustomerGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerGroupHandler(
        IBaseCommandRepository<CustomerGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCustomerGroupResult> Handle(DeleteCustomerGroupRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository
            .GetQuery()
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerSubGroups)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteCustomerGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


