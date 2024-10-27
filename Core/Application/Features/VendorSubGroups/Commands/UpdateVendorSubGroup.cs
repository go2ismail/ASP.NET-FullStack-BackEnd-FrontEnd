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






public class UpdateVendorSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateVendorSubGroupRequest : IRequest<UpdateVendorSubGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string VendorGroupId { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string? Description { get; init; }
}

public class UpdateVendorSubGroupValidator : AbstractValidator<UpdateVendorSubGroupRequest>
{
    public UpdateVendorSubGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.VendorGroupId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class UpdateVendorSubGroupHandler : IRequestHandler<UpdateVendorSubGroupRequest, UpdateVendorSubGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorSubGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateVendorSubGroupResult> Handle(UpdateVendorSubGroupRequest request, CancellationToken cancellationToken = default)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.VendorSubGroups.Where(y => y.IsDeleted == false))
            .Where(x => x.Id == request.VendorGroupId);


        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.VendorGroupId}");
        }

        var result = entity.UpdateSubGroup(
            request.UserId,
            request.Id,
            request.Name,
            request.Description);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateVendorSubGroupResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}





