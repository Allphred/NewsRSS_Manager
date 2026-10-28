namespace RssAgregator.Blazor.Models;

// Espejo del modelo Article del Web API.
public class Article
{
    public int Id { get; set; }                          
    public string RssGuid { get; set; } = string.Empty; 
    public string Title { get; set; } = string.Empty;    
    public string Link { get; set; } = string.Empty;     
    public string Description { get; set; } = string.Empty; 
    public DateTime PublishedDate { get; set; }          
    public bool IsRead { get; set; }                     
    public bool IsSaved { get; set; }                    
    public bool IsDiscarded { get; set; }                
    public int FeedId { get; set; }                      // FK del feed
    public Feed? Feed { get; set; }                      
                                                         
}