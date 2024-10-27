// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Configurations;
using Infrastructure.SecurityManagers.AspNetIdentity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessManagers.EFCores.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<ApplicationUser>(options), IEntityDbSet
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new GenderConfiguration());
        modelBuilder.ApplyConfiguration(new NumberSequenceConfiguration());
        modelBuilder.ApplyConfiguration(new TokenConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerContactConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerGroupConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerSubGroupConfiguration());
        modelBuilder.ApplyConfiguration(new FileDocConfiguration());
        modelBuilder.ApplyConfiguration(new FileImageConfiguration());
        modelBuilder.ApplyConfiguration(new VendorConfiguration());
        modelBuilder.ApplyConfiguration(new VendorContactConfiguration());
        modelBuilder.ApplyConfiguration(new VendorGroupConfiguration());
        modelBuilder.ApplyConfiguration(new VendorSubGroupConfiguration());

    }

}
