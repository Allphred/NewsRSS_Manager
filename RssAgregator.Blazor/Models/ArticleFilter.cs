namespace RssAgregator.Blazor.Models;

// Objeto que viaja del FilterBar (componente hijo) a la página Articles (padre)
// cuando el usuario cambia el estado o la categoría. 
public class ArticleFilter
{
    public string Status { get; set; } = "new";  // new | saved | read | discarded | all
    public string? Category { get; set; }        // Tech, News... o null para "Todas"
}