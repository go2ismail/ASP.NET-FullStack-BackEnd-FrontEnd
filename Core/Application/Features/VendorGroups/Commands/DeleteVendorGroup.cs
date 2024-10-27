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

namespace Application.Features.VendorGroups.Commands;



public class DeleteVendorGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteVendorGroupRequest : IRequest<DeleteVendorGroupResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteVendorGroupValidator : AbstractValidator<DeleteVendorGroupRequest>
{
    public DeleteVendorGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteVendorGroupHandler : IRequestHandler<DeleteVendorGroupRequest, DeleteVendorGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVendorGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteVendorGroupResult> Handle(DeleteVendorGroupRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository
            .GetQuery()
            .ApplyIsDeletedFilter()
            .Include(x => x.VendorSubGroups)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteVendorGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}



