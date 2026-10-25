using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssAgregator.Api.Data;
using RssAgregator.Api.Models;
using RssAgregator.Api.Services;

namespace RssAgregator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Gestiona los feeds RSS.
public class FeedsController : ControllerBase
{
    // Acceso a la base de datos.
    private readonly AppDbContext _db;
    // Registro de eventos y errores.
    private readonly ILogger<FeedsController> _logger;
    // Configuración, incluidas las categorías permitidas.
    private readonly IConfiguration _config;
    // Servicio para descargar y guardar artículos RSS.
    private readonly IRssService _rssService;

    public FeedsController(AppDbContext db, ILogger<FeedsController> logger,
                           IConfiguration config, IRssService rssService)
    {
        _db = db;
        _logger = logger;
        _config = config;
        _rssService = rssService;
    }

    // Devuelve todos los feeds registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Feed>>> GetFeeds()
    {
        _logger.LogInformation("Listando feeds");
        return await _db.Feeds.ToListAsync();
    }

    // Busca un feed e incluye sus artículos.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Feed>> GetFeed(int id)
    {
        var feed = await _db.Feeds.Include(f => f.Articles)
                                  .FirstOrDefaultAsync(f => f.Id == id);
        if (feed is null) return NotFound();
        return feed;
    }

    // Crea un feed si su categoría es válida y su URL no está registrada.
    [HttpPost]
    public async Task<ActionResult<Feed>> CreateFeed(Feed feed)
    {
        var error = ValidateCategory(feed.Category);
        if (error is not null) return BadRequest(new { error });

        if (await _db.Feeds.AnyAsync(f => f.Url == feed.Url))
            return Conflict($"Ya existe un feed con la URL {feed.Url}");

        _db.Feeds.Add(feed);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Feed creado: {Name} ({Url})", feed.Name, feed.Url);
        return CreatedAtAction(nameof(GetFeed), new { id = feed.Id }, feed);
    }

    // Actualiza el nombre, la URL y la categoría del feed.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateFeed(int id, Feed input)
    {
        var feed = await _db.Feeds.FindAsync(id);
        if (feed is null) return NotFound();

        var error = ValidateCategory(input.Category);
        if (error is not null) return BadRequest(new { error });

        feed.Name = input.Name;
        feed.Url = input.Url;
        feed.Category = input.Category;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Feed {Id} actualizado", id);
        return NoContent();
    }

    // Elimina el feed y sus artículos relacionados en cascada.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFeed(int id)
    {
        var feed = await _db.Feeds.FindAsync(id);
        if (feed is null) return NotFound();

        _db.Feeds.Remove(feed);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Feed {Id} eliminado (artículos en cascada)", id);
        return NoContent();
    }

    // Descarga el RSS y guarda sus artículos nuevos.
    [HttpPost("{id:int}/refresh")]
    public async Task<IActionResult> RefreshFeed(int id)
    {
        try
        {
            var nuevos = await _rssService.FetchArticlesAsync(id);
            if (nuevos < 0) return NotFound();
            return Ok(new { feedId = id, articulosNuevos = nuevos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refrescando el feed {Id}", id);
            return BadRequest(new { error = "No se pudo leer el RSS: " + ex.Message });
        }
    }

    // Comprueba que la categoría esté permitida en la configuración.
    private string? ValidateCategory(string category)
    {
        // Si no se configura una lista, no se restringen las categorías.
        var permitidas = _config.GetSection("RssSettings:AllowedCategories").Get<string[]>()
                         ?? Array.Empty<string>();
        if (permitidas.Length > 0 && !permitidas.Contains(category))
            return $"Categoría '{category}' inválida. Permitidas: {string.Join(", ", permitidas)}";
        return null;
    }
}