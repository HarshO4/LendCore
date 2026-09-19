using LendingPlatform.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LendingPlatform.Tests.Api;

/// <summary>
/// Custom test web application factory that:
/// - Replaces the SQLite database with an in-memory database (unique per instance).
/// - Sets the environment to "Testing" to skip the Migrate() call in Program.cs.
/// </summary>
public class LendingApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use "Testing" environment so the Migrate() block does not run.
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the registered DbContext options (which point to SQLite).
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Also remove any EF Core internal service provider registrations
            // to avoid the "multiple providers" error.
            var efServiceDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true
                         && d.ServiceType.FullName.Contains("IDbContextOptions"))
                .ToList();
            foreach (var d in efServiceDescriptors)
                services.Remove(d);

            // Register fresh in-memory DbContext with an isolated database name.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}
