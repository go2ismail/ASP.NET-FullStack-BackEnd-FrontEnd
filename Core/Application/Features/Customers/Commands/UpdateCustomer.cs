// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Repositories;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Customers.Commands;


public class UpdateCustomerResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateCustomerRequest : IRequest<UpdateCustomerResult>
{
    public string Id { get; init; } = null!;
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

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
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


public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerResult> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
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

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateCustomerResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


