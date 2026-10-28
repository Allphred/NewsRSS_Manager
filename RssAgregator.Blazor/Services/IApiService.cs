using RssAgregator.Blazor.Models;

namespace RssAgregator.Blazor.Services;

// Interfaz de TODA la comunicación con el Web API.
public interface IApiService
{
    // Último mensaje de error si hay de la última llamada al API.
    string? LastError { get; }

    Task<List<Feed>?> GetFeedsAsync();              // GET    api/feeds
    Task<Feed?> CreateFeedAsync(Feed feed);         // POST   api/feeds
    Task<bool> DeleteFeedAsync(int id);             // DELETE api/feeds/{id}
    Task<int> RefreshFeedAsync(int id);             // POST   api/feeds/{id}/refresh
    Task<List<Article>?> GetArticlesAsync(          // GET    api/articles?status=&category=
        string status, string? category);
    Task<bool> UpdateArticleFlagsAsync(             // PATCH  api/articles/{id}
        int id, bool? isRead, bool? isSaved, bool? isDiscarded);
}
