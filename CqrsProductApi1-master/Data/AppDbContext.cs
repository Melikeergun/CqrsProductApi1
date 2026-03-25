using CqrsProductApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CqrsProductApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        builder.UseOpenIddict();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(x => x.Price)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.CreatedAt)
                  .IsRequired();
        });
    }
}