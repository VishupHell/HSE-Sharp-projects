using Microsoft.EntityFrameworkCore;
using FileAnalysisService.Models;

public class FileAnalysisServiceDBContext : DbContext
{
    public DbSet<Report> Files { get; set; } = null!;

    public FileAnalysisServiceDBContext(DbContextOptions<FileAnalysisServiceDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();

            entity.Property(f => f.FileCloudWords)
                .HasMaxLength(100);
            
            entity.Property(f => f.FileType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(f => f.FilePath)
                .IsRequired();
            entity.Property(f => f.Exercise)
                .IsRequired();

            entity.Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(100);

            entity.ToTable("reports");
        });
    }
}