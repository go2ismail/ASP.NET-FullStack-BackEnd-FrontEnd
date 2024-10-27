// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Vendors.Commands;


public class UpdateVendorResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateVendorRequest : IRequest<UpdateVendorResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string VendorGroupId { get; init; } = null!;
    public string? VendorSubGroupId { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
}

public class UpdateVendorValidator : AbstractValidator<UpdateVendorRequest>
{
    public UpdateVendorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.VendorGroupId)
            .NotEmpty();
        RuleFor(x => x.Street)
            .NotEmpty();
        RuleFor(x => x.City)
            .NotEmpty();
        RuleFor(x => x.StateOrProvince)
            .NotEmpty();
        RuleFor(x => x.ZipCode)
            .NotEmpty();
        RuleFor(x => x.Phone)
            .NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty();
    }
}


public class UpdateVendorHandler : IRequestHandler<UpdateVendorRequest, UpdateVendorResult>
{
    private readonly IBaseCommandRepository<Vendor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorHandler(
        IBaseCommandRepository<Vendor> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateVendorResult> Handle(UpdateVendorRequest request, CancellationToken cancellationToken)
    {

        var entity = await _repository.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Update(
                request.UserId,
                request.Name,
                request.Description,
                request.VendorGroupId,
                request.VendorSubGroupId,
                request.Street,
                request.City,
                request.StateOrProvince,
                request.ZipCode,
                request.Country,
                request.Phone,
                request.Email,
                request.Website
            );

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateVendorResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}



