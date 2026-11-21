namespace RssAgregator.Blazor.Models;

public static class FeedCategories
{
    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        "Tech", "News", "Deportes", "Gaming", "General",
        "Educación", "Espacio", "Océano", "Filosofía", "Naturaleza",
        "Psicología", "Exploración", "Humor"
    });
}
