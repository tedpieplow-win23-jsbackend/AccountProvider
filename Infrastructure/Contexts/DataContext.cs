using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<UserAddressEntity> UserAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAddressEntity>()
            .HasKey(ua => new { ua.UserId, ua.AddressId } );

        modelBuilder.Entity<UserAddressEntity>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.UserAddresses)
            .HasForeignKey(ua => ua.UserId);

        modelBuilder.Entity<UserAddressEntity>()
            .HasOne(ua => ua.Address)
            .WithMany(a => a.UserAddresses)
            .HasForeignKey(a => a.AddressId);

        base.OnModelCreating(modelBuilder);
    }
}
