using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RssAgregator.Api.Data;

// Usada por "dotnet ef" para crear el DbContext en tiempo de diseño,
// sin necesidad de arrancar Program.cs ni el contenedor de DI.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Lee la misma cadena de conexión de appsettings.json (una sola fuente de verdad)
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(config.GetConnectionString("DefaultConnection"))
            .Options;

        return new AppDbContext(options);
    }
}