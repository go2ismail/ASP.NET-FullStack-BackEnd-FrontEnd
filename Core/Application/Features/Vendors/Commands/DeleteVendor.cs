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

namespace Application.Features.Vendors.Commands;



public class DeleteVendorResult
{
    public string Id { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class DeleteVendorRequest : IRequest<DeleteVendorResult>
{
    public string Id { get; init; } = null!;
    public string? UserId { get; init; }
}

public class DeleteVendorValidator : AbstractValidator<DeleteVendorRequest>
{
    public DeleteVendorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class DeleteVendorHandler : IRequestHandler<DeleteVendorRequest, DeleteVendorResult>
{
    private readonly IBaseCommandRepository<Vendor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVendorHandler(
        IBaseCommandRepository<Vendor> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteVendorResult> Handle(DeleteVendorRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository
            .GetQuery()
            .ApplyIsDeletedFilter()
            .Include(x => x.VendorContacts)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        entity.Delete(request.UserId);
        _repository.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteVendorResult
        {
            Id = entity.Id,
            Message = "Success"
        };
    }
}



