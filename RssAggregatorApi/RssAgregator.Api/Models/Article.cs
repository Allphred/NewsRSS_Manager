using System;

namespace RssAgregator.Api.Models;

public class Article
{
    public int Id { get; set; }                          // PK de la BD
    public string RssGuid { get; set; } = string.Empty;  // <guid> del RSS -> deduplicación
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;     
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; } // Pending: Por que no lleva asignacion de valor por defecto?

    public bool IsRead { get; set; }
    public bool IsSaved { get; set; }
    public bool IsDiscarded { get; set; }

    // FK: todo artículo pertenece a un feed
    public int FeedId { get; set; }
    public Feed Feed { get; set; } = null!;
}

