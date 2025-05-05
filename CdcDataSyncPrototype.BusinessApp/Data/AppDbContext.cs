using CdcDataSyncPrototype.BusinessApp.Models;
using CdcDataSyncPrototype.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CdcDataSyncPrototype.BusinessApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<CdcSyncCheckpoint> CdcSyncCheckpoints => Set<CdcSyncCheckpoint>();
    public DbSet<PublicationAuditLog> PublicationAuditLogs => Set<PublicationAuditLog>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CdcSyncCheckpoint>(builder =>
        {
            builder.ToTable("CdcSyncCheckpoints");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.LastUpdatedUtc)
                .HasDefaultValueSql("GETUTCDATE()");
        });


        modelBuilder.Entity<Publication>(builder =>
        {
            builder.ToTable("Publications");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.LastModified)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<PublicationAuditLog>(entity =>
        {
            entity.ToTable("PublicationAuditLog");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.RuleOutcome).HasMaxLength(255);
            entity.Property(e => e.MetadataJson).HasColumnType("nvarchar(max)");
        });

    }
}