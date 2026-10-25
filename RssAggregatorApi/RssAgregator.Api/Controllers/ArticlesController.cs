using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssAgregator.Api.Data;
using RssAgregator.Api.Models;

namespace RssAgregator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Gestiona la consulta y administración de artículos RSS.
public class ArticlesController : ControllerBase
{
    // Acceso a la base de datos.
    private readonly AppDbContext _db;
    // Registro de eventos.
    private readonly ILogger<ArticlesController> _logger;
    // Configuración, incluida la retención de artículos.
    private readonly IConfiguration _config;

    public ArticlesController(AppDbContext db, ILogger<ArticlesController> logger,
                              IConfiguration config)
    {
        _db = db;
        _logger = logger;
        _config = config;
    }

    // Lista artículos por estado y categoría. Por defecto, muestra los no leídos ni descartados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetArticles(
        string status = "new", string? category = null)
    {
        var query = _db.Articles.Include(a => a.Feed).AsQueryable();

        query = status switch
        {
            "saved"     => query.Where(a => a.IsSaved),
            "read"      => query.Where(a => a.IsRead),
            "discarded" => query.Where(a => a.IsDiscarded),
            "all"       => query,
            _           => query.Where(a => !a.IsRead && !a.IsDiscarded)
        };

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Feed!.Category == category);

        return await query.OrderByDescending(a => a.PublishedDate).ToListAsync();
    }

    // Busca un artículo e incluye los datos de su feed.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Article>> GetArticle(int id)
    {
        var article = await _db.Articles.Include(a => a.Feed)
                                        .FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return NotFound();
        return article;
    }

    // Crea un artículo
    [HttpPost]
    public async Task<ActionResult<Article>> CreateArticle(Article article)
    {
        if (!await _db.Feeds.AnyAsync(f => f.Id == article.FeedId))
            return BadRequest($"No existe el feed {article.FeedId}");

        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    // Actualiza los datos editables de un artículo.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateArticle(int id, Article input)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        article.Title = input.Title;
        article.Link = input.Link;
        article.Description = input.Description;
        article.PublishedDate = input.PublishedDate;
        article.IsRead = input.IsRead;
        article.IsSaved = input.IsSaved;
        article.IsDiscarded = input.IsDiscarded;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Artículo {Id} actualizado", id);
        return NoContent();
    }

    // Actualiza solo los campos recibidos, por ejemplo el estado de lectura o guardado.
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchArticle(int id, ArticlePatchDto dto)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        if (dto.Title is not null) article.Title = dto.Title;
        if (dto.IsRead.HasValue) article.IsRead = dto.IsRead.Value;
        if (dto.IsSaved.HasValue) article.IsSaved = dto.IsSaved.Value;
        if (dto.IsDiscarded.HasValue) article.IsDiscarded = dto.IsDiscarded.Value;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Artículo {Id}: read={R} saved={S} discarded={D}",
            id, article.IsRead, article.IsSaved, article.IsDiscarded);
        return NoContent();
    }

    // Elimina un artículo.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Elimina artículos antiguos que no están guardados.
    [HttpDelete("purge-old")]
    public async Task<IActionResult> PurgeOld()
    {
        // Lee el período de retención de la configuración
        var dias = _config.GetValue("RssSettings:RetentionDays", 7);
        var limite = DateTime.UtcNow.AddDays(-dias);

        // Solo elimina artículos no guardados cuya fecha sea anterior al límite calculado.
        var query = _db.Articles.Where(a => !a.IsSaved && a.PublishedDate < limite);
        var total = await query.CountAsync();
        await query.ExecuteDeleteAsync();

        _logger.LogInformation("Purge: {Total} artículos de +{Dias} días eliminados", total, dias);
        return Ok(new { eliminados = total, retentionDays = dias });
    }
}

public record ArticlePatchDto(string? Title, bool? IsRead, bool? IsSaved, bool? IsDiscarded);