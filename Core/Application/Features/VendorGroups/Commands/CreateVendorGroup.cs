// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.VendorGroups.Commands;


public class CreateVendorGroupResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateVendorGroupRequest : IRequest<CreateVendorGroupResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class CreateVendorGroupValidator : AbstractValidator<CreateVendorGroupRequest>
{
    public CreateVendorGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}


public class CreateVendorGroupHandler : IRequestHandler<CreateVendorGroupRequest, CreateVendorGroupResult>
{
    private readonly IBaseCommandRepository<VendorGroup> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorGroupHandler(
        IBaseCommandRepository<VendorGroup> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateVendorGroupResult> Handle(CreateVendorGroupRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new VendorGroup(
                request.UserId,
                request.Name,
                request.Description
                );

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateVendorGroupResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}



