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

namespace Application.Features.CustomerContacts.Commands;



public class CreateCustomerContactResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class CreateCustomerContactRequest : IRequest<CreateCustomerContactResult>
{
    public string? UserId { get; init; }
    public string CustomerId { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string GenderId { get; init; } = null!;
    public string? Description { get; init; }
    public string JobTitle { get; init; } = null!;
    public string? MobilePhone { get; init; }
    public string? SocialMedia { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? StateOrProvince { get; init; }
    public string? ZipCode { get; init; }
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
}

public class CreateCustomerContactValidator : AbstractValidator<CreateCustomerContactRequest>
{
    public CreateCustomerContactValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
        RuleFor(x => x.FirstName)
            .NotEmpty();
        RuleFor(x => x.LastName)
            .NotEmpty();
        RuleFor(x => x.GenderId)
            .NotEmpty();
        RuleFor(x => x.JobTitle)
            .NotEmpty();
        RuleFor(x => x.Phone)
            .NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty();
    }
}


public class CreateCustomerContactHandler : IRequestHandler<CreateCustomerContactRequest, CreateCustomerContactResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerContactHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerContactResult> Handle(CreateCustomerContactRequest request, CancellationToken cancellationToken = default)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerContacts.Where(y => y.IsDeleted == false))
            .Where(x => x.Id == request.CustomerId);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.CustomerId}");
        }

        var result = entity.CreateContact(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.GenderId,
            request.Description,
            request.JobTitle,
            request.MobilePhone,
            request.SocialMedia,
            request.Address,
            request.City,
            request.StateOrProvince,
            request.ZipCode,
            request.Country,
            request.Phone,
            request.Email,
            request.Website);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateCustomerContactResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}



