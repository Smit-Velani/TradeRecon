using Microsoft.EntityFrameworkCore;
using TradeRecon.Core.Models;

namespace TradeRecon.Infrastructure.Data;

public class TradeReconDbContext : DbContext
{
    public TradeReconDbContext(
        DbContextOptions<TradeReconDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<ReconciliationResult> ReconciliationResults
        => Set<ReconciliationResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.TradeId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(t => t.Price)
                .HasPrecision(18, 4);

            entity.Property(t => t.Quantity)
                .HasPrecision(18, 4);

            entity.HasIndex(t => new
            {
                t.TradeId,
                t.SourceSystem
            });
        });

        modelBuilder.Entity<ReconciliationResult>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.TradeId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(r => r.Message)
                .HasMaxLength(500);
        });
    }
}

