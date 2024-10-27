// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.CustomerContacts.Queries;
using Application.Features.VendorContacts.Queries;
using Application.Services.CQS.Queries;

using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboards.Queries;


public class GetDashboardMainDto
{
    public List<GetCustomerContactDto> CustomerContacts { get; set; } = new List<GetCustomerContactDto>();
    public List<GetVendorContactDto> VendorContacts { get; set; } = new List<GetVendorContactDto>();
}


public class GetDashboardMainProfile : Profile
{
    public GetDashboardMainProfile()
    {
    }
}

public class GetDashboardMainResult
{
    public GetDashboardMainDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetDashboardMainRequest : IRequest<GetDashboardMainResult>
{
}

public class GetDashboardMainValidator : AbstractValidator<GetDashboardMainRequest>
{
    public GetDashboardMainValidator()
    {
    }
}


public class GetDashboardMainHandler : IRequestHandler<GetDashboardMainRequest, GetDashboardMainResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetDashboardMainHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetDashboardMainResult> Handle(GetDashboardMainRequest request, CancellationToken cancellationToken)
    {
        var customerContactEntities = await (
            from customerContact in _context.CustomerContact.AsNoTracking().ApplyIsDeletedFilter()
            join customer in _context.Customer.AsNoTracking()
                on customerContact.CustomerId equals customer.Id into customerLookup
            from customer in customerLookup.DefaultIfEmpty()
            join gender in _context.Gender.AsNoTracking()
                on customerContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            select new
            {
                customerContact,
                CustomerName = customer != null ? customer.Name : null,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToListAsync(cancellationToken);

        var customerContactDtos = customerContactEntities.Select(entity =>
        {
            var dto = _mapper.Map<GetCustomerContactDto>(entity.customerContact);
            return dto with
            {
                CustomerName = entity.CustomerName,
                GenderName = entity.GenderName
            };
        }).ToList();


        var vendorContactEntities = await (
            from vendorContact in _context.VendorContact.AsNoTracking().ApplyIsDeletedFilter()
            join vendor in _context.Vendor.AsNoTracking()
                on vendorContact.VendorId equals vendor.Id into vendorLookup
            from vendor in vendorLookup.DefaultIfEmpty()
            join gender in _context.Gender.AsNoTracking()
                on vendorContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            select new
            {
                vendorContact,
                VendorName = vendor != null ? vendor.Name : null,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToListAsync(cancellationToken);

        var vendorContactDtos = vendorContactEntities.Select(entity =>
        {
            var dto = _mapper.Map<GetVendorContactDto>(entity.vendorContact);
            return dto with
            {
                VendorName = entity.VendorName,
                GenderName = entity.GenderName
            };
        }).ToList();

        return new GetDashboardMainResult
        {
            Data = new GetDashboardMainDto
            {
                CustomerContacts = customerContactDtos,
                VendorContacts = vendorContactDtos
            },
            Message = "Success"
        };
    }
}




