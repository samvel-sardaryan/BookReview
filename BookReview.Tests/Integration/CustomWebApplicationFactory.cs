using BookReview.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookReview.Tests.Integration;

// Boots the real API in-memory (TestServer) but swaps PostgreSQL for the
// EF Core in-memory provider and supplies a JWT signing key, so the tests
// need neither Docker nor a database.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"it-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // HMAC-SHA512 needs a key of at least 64 bytes.
                ["AppSettings:Token"] = "integration-test-signing-key-which-is-long-enough-for-hmac-sha512-0123456789",
                ["AppSettings:Issuer"] = "MyApp",
                ["AppSettings:Audience"] = "MyAppUsers",
                ["ConnectionStrings:DefaultConnection"] = "Host=unused;Database=unused;Username=unused;Password=unused"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove the PostgreSQL-backed DataContext registrations...
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<DataContext>) ||
                d.ServiceType == typeof(DataContext) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericTypeDefinition().Name.Contains("IDbContextOptionsConfiguration") &&
                 d.ServiceType.GenericTypeArguments.Length == 1 &&
                 d.ServiceType.GenericTypeArguments[0] == typeof(DataContext)))
                .ToList();
            foreach (var descriptor in toRemove)
                services.Remove(descriptor);

            // ...and replace them with a shared in-memory database for the test run.
            services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(_dbName));
        });
    }
}
