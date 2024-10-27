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

namespace Application.Features.VendorContacts.Commands;



public class DeleteVendorContactResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteVendorContactRequest : IRequest<DeleteVendorContactResult>
{
    public string? UserId { get; init; }
    public string VendorContactId { get; init; } = null!;
    public string VendorId { get; init; } = null!;
}

public class DeleteVendorContactValidator : AbstractValidator<DeleteVendorContactRequest>
{
    public DeleteVendorContactValidator()
    {
        RuleFor(x => x.VendorContactId)
            .NotEmpty();
    }
}


public class DeleteVendorContactHandler : IRequestHandler<DeleteVendorContactRequest, DeleteVendorContactResult>
{
    private readonly IBaseCommandRepository<Vendor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVendorContactHandler(
        IBaseCommandRepository<Vendor> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteVendorContactResult> Handle(DeleteVendorContactRequest request, CancellationToken cancellationToken = default)
    {
        var query = _repository.GetQuery();

        query = query
            .ApplyIsDeletedFilter()
            .Include(x => x.VendorContacts)
            .Where(x => x.Id == request.VendorId);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.VendorId}");
        }

        var result = entity.DeleteContact(
            request.UserId,
            request.VendorContactId);

        _repository.Update(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteVendorContactResult
        {
            Id = result.Id,
            Message = "Success"
        };
    }
}

