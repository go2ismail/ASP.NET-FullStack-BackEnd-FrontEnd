// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.VendorGroups.Commands;


public class UpdateVendorGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateVendorGroupRequest : IRequest<UpdateVendorGroupResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class UpdateVendorGroupValidator : AbstractValidator<UpdateVendorGroupRequest>
{
    public UpdateVendorGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class UpdateVendorGroupHandler : IRequestHandler<UpdateVendorGroupRequest, UpdateVendorGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateVendorGroupResult> Handle(UpdateVendorGroupRequest request, CancellationToken cancellationToken)
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

        return new UpdateVendorGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}



