using Microsoft.EntityFrameworkCore;
using RssAgregator.Api.Models;

namespace RssAgregator.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // No permitir dos fuentes con la misma URL
        mb.Entity<Feed>().HasIndex(f => f.Url).IsUnique();

        // Un artículo no se duplica dentro del mismo feed
        mb.Entity<Article>().HasIndex(a => new { a.FeedId, a.RssGuid }).IsUnique();

        // Filtros 
        mb.Entity<Article>().HasIndex(a => a.PublishedDate);
        mb.Entity<Feed>().HasIndex(f => f.Category);
    }
}