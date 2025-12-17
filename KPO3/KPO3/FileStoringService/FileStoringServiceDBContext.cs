using Microsoft.EntityFrameworkCore;
using FileStoringService.Models;

public class FileStoringServiceDBContext : DbContext
{
    public DbSet<Homework> Files { get; set; } = null!;

    public FileStoringServiceDBContext(DbContextOptions<FileStoringServiceDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Homework>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();

            entity.Property(f => f.StudentName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(f => f.Exercise)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(f => f.FilePath)
                .IsRequired();

            entity.Property(f => f.FileName)
                .IsRequired();
            
            entity.Property(f => f.Time)
                .IsRequired();
            
            entity.Property(f => f.FileType)
                .IsRequired()
                .HasMaxLength(100);

            entity.ToTable("homeworks");
        });
    }
}