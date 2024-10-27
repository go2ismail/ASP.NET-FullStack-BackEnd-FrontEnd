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



public class UpdateCustomerContactResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class UpdateCustomerContactRequest : IRequest<UpdateCustomerContactResult>
{
    public string? UserId { get; init; }
    public string Id { get; init; } = null!;
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

public class UpdateCustomerContactValidator : AbstractValidator<UpdateCustomerContactRequest>
{
    public UpdateCustomerContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
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


public class UpdateCustomerContactHandler : IRequestHandler<UpdateCustomerContactRequest, UpdateCustomerContactResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerContactHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerContactResult> Handle(UpdateCustomerContactRequest request, CancellationToken cancellationToken = default)
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

        entity.UpdateContact(
            request.UserId,
            request.Id,
            request.FirstName,
            request.LastName,
            request.JobTitle,
            request.GenderId,
            request.Email,
            request.Description,
            request.MobilePhone,
            request.SocialMedia,
            request.Address,
            request.City,
            request.StateOrProvince,
            request.ZipCode,
            request.Country,
            request.Phone,
            request.Website);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateCustomerContactResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


