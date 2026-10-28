namespace RssAgregator.Blazor.Models;

// Espejo del modelo Feed del Web API.
public class Feed
{
    public int Id { get; set; }                       
    public string Name { get; set; } = string.Empty;   
    public string Url { get; set; } = string.Empty;    
    public string Category { get; set; } = "General";  
    public DateTime? LastFetchedAt { get; set; }       
    public List<Article> Articles { get; set; } = new(); 
}