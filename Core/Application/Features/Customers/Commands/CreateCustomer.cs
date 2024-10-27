// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Customers.Commands;



public class CreateCustomerResult
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateCustomerRequest : IRequest<CreateCustomerResult>
{
    public string? UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CustomerGroupId { get; init; } = null!;
    public string? CustomerSubGroupId { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
}

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.CustomerGroupId)
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


public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CreateCustomerResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INumberSequenceService _numberSequenceService;

    public CreateCustomerHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberSequenceService = numberSequenceService;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var code = _numberSequenceService.GenerateNumberSequence(
            request.UserId,
            nameof(Customer),
            null,
            "CST");

        var entity = new Customer(
                request.UserId,
                code,
                request.Name,
                request.Description,
                request.CustomerGroupId,
                request.CustomerSubGroupId,
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

        return new CreateCustomerResult
        {
            Id = entity.Id,
            Code = entity.Code,
            Message = "Success"
        };
    }
}


