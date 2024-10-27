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

namespace Application.Features.VendorSubGroups.Commands;



public class DeleteVendorSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteVendorSubGroupRequest : IRequest<DeleteVendorSubGroupResult>
{
    public string? UserId { get; init; }
    public string Id { get; init; } = null!;
    public string VendorGroupId { get; init; } = null!;
}

public class DeleteVendorSubGroupValidator : AbstractValidator<DeleteVendorSubGroupRequest>
{
    public DeleteVendorSubGroupValidator()
    {
        RuleFor(x => x.VendorGroupId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteVendorSubGroupHandler : IRequestHandler<DeleteVendorSubGroupRequest, DeleteVendorSubGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVendorSubGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteVendorSubGroupResult> Handle(DeleteVendorSubGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.VendorSubGroups)
            .Where(x => x.Id == request.VendorGroupId);


        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.VendorGroupId}");
        }

        entity.DeleteSubGroup(
            request.UserId,
            request.Id);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteVendorSubGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}




