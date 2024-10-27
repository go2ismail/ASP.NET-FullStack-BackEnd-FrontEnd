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



public class DeleteCustomerContactResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteCustomerContactRequest : IRequest<DeleteCustomerContactResult>
{
    public string? UserId { get; init; }
    public string Id { get; init; } = null!;
    public string CustomerId { get; init; } = null!;
}

public class DeleteCustomerContactValidator : AbstractValidator<DeleteCustomerContactRequest>
{
    public DeleteCustomerContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}


public class DeleteCustomerContactHandler : IRequestHandler<DeleteCustomerContactRequest, DeleteCustomerContactResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerContactHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCustomerContactResult> Handle(DeleteCustomerContactRequest request, CancellationToken cancellationToken = default)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerContacts)
            .Where(x => x.Id == request.CustomerId);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.CustomerId}");
        }

        var result = entity.DeleteContact(
            request.UserId,
            request.Id);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteCustomerContactResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}
