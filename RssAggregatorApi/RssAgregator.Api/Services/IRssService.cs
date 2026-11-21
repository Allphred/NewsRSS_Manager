using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using RssAgregator.Api.Data;
using RssAgregator.Api.Models;

namespace RssAgregator.Api.Services;

public interface IRssService
{
    // Descarga un feed y guarda sus artículos nuevos. Devuelve -1 si no existe.
    Task<int> FetchArticlesAsync(int feedId);
}

// Descarga feeds RSS y almacena sus artículos nuevos.
public class RssService : IRssService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly ILogger<RssService> _logger;

    public RssService(AppDbContext db, HttpClient http, ILogger<RssService> logger)
    {
        _db = db;
        _http = http;
        _logger = logger;

        _http.DefaultRequestHeaders.UserAgent.ParseAdd("RssAgregator/1.0");
        _http.DefaultRequestHeaders.Accept.ParseAdd("application/rss+xml");
        _http.DefaultRequestHeaders.Accept.ParseAdd("application/xml");
    }

    public async Task<int> FetchArticlesAsync(int feedId)
    {
        // Busca el feed solicitado.
        var feed = await _db.Feeds.FindAsync(feedId);
        if (feed is null)
        {
            _logger.LogWarning("Fetch: no existe el feed {FeedId}", feedId);
            return -1;
        }

        // Descarga el RSS con un User-Agent identificable y lee el XML de la respuesta.
        using var response = await _http.GetAsync(feed.Url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = XmlReader.Create(stream);
        var rss = SyndicationFeed.Load(reader)
              ?? throw new InvalidDataException("La respuesta no contiene un feed RSS válido.");

        var nuevos = 0;
        foreach (var item in rss.Items)
        {
            // Obtiene el enlace y usa el GUID o el enlace como identificador.
            var link = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty;
            var guid = string.IsNullOrWhiteSpace(item.Id) ? link : item.Id;
            if (string.IsNullOrWhiteSpace(guid)) continue;

            // Omite artículos que ya están guardados.
            if (await _db.Articles.AnyAsync(a => a.FeedId == feed.Id && a.RssGuid == guid))
                continue;

            // Crea el artículo a partir de los datos del RSS.
            _db.Articles.Add(new Article
            {
                RssGuid = guid,
                Title = item.Title?.Text ?? "(sin título)",
                Link = link,
                Description = item.Summary?.Text ?? string.Empty,
                PublishedDate = item.PublishDate.UtcDateTime,
                FeedId = feed.Id
            });
            nuevos++;
        }

        // Actualiza la fecha de consulta y guarda los cambios.
        feed.LastFetchedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Registra y devuelve la cantidad de artículos agregados.
        _logger.LogInformation("Feed '{Name}': {N} artículos nuevos", feed.Name, nuevos);
        return nuevos;
    }
}