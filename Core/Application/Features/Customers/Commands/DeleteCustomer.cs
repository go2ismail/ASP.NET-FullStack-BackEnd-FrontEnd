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

namespace Application.Features.Customers.Commands;



public class DeleteCustomerResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteCustomerRequest : IRequest<DeleteCustomerResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteCustomerValidator : AbstractValidator<DeleteCustomerRequest>
{
    public DeleteCustomerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerRequest, DeleteCustomerResult>
{
    private readonly IBaseCommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerHandler(
        IBaseCommandRepository<Customer> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCustomerResult> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository
            .GetQuery()
            .ApplyIsDeletedFilter()
            .Include(x => x.CustomerContacts)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteCustomerResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}


