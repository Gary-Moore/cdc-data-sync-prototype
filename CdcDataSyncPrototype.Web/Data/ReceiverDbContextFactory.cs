using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ReceiverDbContextFactory : IDesignTimeDbContextFactory<ReceiverDbContext>
{
    public ReceiverDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ReceiverDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("ReceiverDb"));

        return new ReceiverDbContext(optionsBuilder.Options);
    }
}