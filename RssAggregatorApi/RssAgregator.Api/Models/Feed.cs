using System;

namespace RssAgregator.Api.Models;

public class Feed
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;     
    public string Category { get; set; } = "General";   // Pending: debo poner lista de categorias o algo asi

    public DateTime? LastFetchedAt { get; set; }

    // Pending:
    public List<Article> Articles { get; set; } = new();
}