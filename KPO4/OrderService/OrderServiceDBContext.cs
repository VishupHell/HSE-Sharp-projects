using Microsoft.EntityFrameworkCore;
using OrderService.Models;
namespace OrderService;

public class OrderServiceDBContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    public OrderServiceDBContext(DbContextOptions<OrderServiceDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(f => f.OrderId);
            entity.Property(f => f.OrderId).ValueGeneratedNever();

            entity.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(f => f.CustomerId)
                .IsRequired();

            entity.Property(f => f.Status)
                .IsRequired();

            entity.ToTable("orders");
        });
    }
}