using LendingPlatform.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Data;

/// <summary>
/// EF Core database context for the lending platform.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanAmount).HasColumnType("TEXT"); // SQLite stores decimal as TEXT
            entity.Property(e => e.AssetValue).HasColumnType("TEXT");
            entity.Property(e => e.Ltv).HasColumnType("TEXT");
            entity.Property(e => e.Decision).HasMaxLength(20).IsRequired();
            entity.Property(e => e.DecisionReason).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.RuleApplied).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
