using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;


public class PaymentServiceDBContext : DbContext
{
    public DbSet<BankAccount> Accounts { get; set; } = null!;

    public PaymentServiceDBContext(DbContextOptions<PaymentServiceDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();

            entity.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(f => f.Amount)
                .IsRequired();

            entity.ToTable("bank_accounts");
        });
    }
}