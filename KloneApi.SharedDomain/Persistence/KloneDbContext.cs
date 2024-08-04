using System.Diagnostics;
using KloneApi.SharedDomain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KloneApi.SharedDomain.Persistence;

public class KloneDbContext : DbContext
{
    public KloneDbContext(DbContextOptions<KloneDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            var connectionString = configuration["DatabaseConfig:KloneConnectionString"];
            optionsBuilder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
    }

}

public class KloneDbContextFactory : IDesignTimeDbContextFactory<KloneDbContext>
{
    public KloneDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KloneDbContext>();
        Console.WriteLine(Directory.GetCurrentDirectory().Replace("KloneApi.SharedDomain",""));
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory().Replace("KloneApi.SharedDomain",""))
            .AddJsonFile("KloneApi.Web/appsettings.json")
            .AddJsonFile($"KloneApi.Web/appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            var connectionString = configuration["DatabaseConfig:KloneConnectionString"];
            
            optionsBuilder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });

        return new KloneDbContext(optionsBuilder.Options);
    }
}

