// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Vendors.Commands;



public class CreateVendorResult
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateVendorRequest : IRequest<CreateVendorResult>
{
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

public class CreateVendorValidator : AbstractValidator<CreateVendorRequest>
{
    public CreateVendorValidator()
    {
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


public class CreateVendorHandler : IRequestHandler<CreateVendorRequest, CreateVendorResult>
{
    private readonly IBaseCommandRepository<Vendor> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INumberSequenceService _numberSequenceService;

    public CreateVendorHandler(
        IBaseCommandRepository<Vendor> repository,
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberSequenceService = numberSequenceService;
    }

    public async Task<CreateVendorResult> Handle(CreateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var code = _numberSequenceService.GenerateNumberSequence(
            request.UserId,
            nameof(Vendor),
            null,
            "VND");

        var entity = new Vendor(
                request.UserId,
                code,
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

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateVendorResult
        {
            Id = entity.Id,
            Code = entity.Code,
            Message = "Success"
        };
    }
}



