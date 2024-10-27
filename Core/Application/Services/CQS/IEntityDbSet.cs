// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Application.Services.CQS;

public interface IEntityDbSet
{
    public DbSet<Gender> Gender { get; set; }
    public DbSet<NumberSequence> NumberSequence { get; set; }
    public DbSet<Config> Config { get; set; }
    public DbSet<Token> Token { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<CustomerContact> CustomerContact { get; set; }
    public DbSet<CustomerGroup> CustomerGroup { get; set; }
    public DbSet<CustomerSubGroup> CustomerSubGroup { get; set; }
    public DbSet<FileDoc> FileDoc { get; set; }
    public DbSet<FileImage> FileImage { get; set; }
    public DbSet<Vendor> Vendor { get; set; }
    public DbSet<VendorContact> VendorContact { get; set; }
    public DbSet<VendorGroup> VendorGroup { get; set; }
    public DbSet<VendorSubGroup> VendorSubGroup { get; set; }
}
