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



public class CreateVendorSubGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateVendorSubGroupRequest : IRequest<CreateVendorSubGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string VendorGroupId { get; set; } = null!;
    public string? Description { get; init; }
}

public class CreateVendorSubGroupValidator : AbstractValidator<CreateVendorSubGroupRequest>
{
    public CreateVendorSubGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.VendorGroupId)
            .NotEmpty();
    }
}


public class CreateVendorSubGroupHandler : IRequestHandler<CreateVendorSubGroupRequest, CreateVendorSubGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorSubGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateVendorSubGroupResult> Handle(CreateVendorSubGroupRequest request, CancellationToken cancellationToken = default)
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

        var result = entity.CreateSubGroup(
            request.UserId,
            request.Name,
            request.Description);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateVendorSubGroupResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}






