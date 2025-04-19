using CdcDataSyncPrototype.Core.Models;
using CdcDataSyncPrototype.Web.Models;
using Microsoft.EntityFrameworkCore;

public class ReceiverDbContext : DbContext
{
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<SyncInboxEntry> SyncInbox => Set<SyncInboxEntry>();
    public DbSet<PublicationStagingEntry> Publications_Staging => Set<PublicationStagingEntry>();



    public ReceiverDbContext(DbContextOptions<ReceiverDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Publication>()
            .Property(p => p.Id)
            .ValueGeneratedNever();


        modelBuilder.Entity<SyncInboxEntry>().ToTable("SyncInbox");
        modelBuilder.Entity<PublicationStagingEntry>(entity =>
        {
            entity.ToTable("Publications_Staging");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);

             entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.Processed)
                .HasDefaultValue(false);

            entity.HasIndex(e => e.Processed);
        });
    }
}
