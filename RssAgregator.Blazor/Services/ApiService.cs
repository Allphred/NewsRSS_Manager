using System.Net.Http.Json;   // extensiones: GetFromJsonAsync, PostAsJsonAsync, PatchAsJsonAsync
using System.Text.Json;       // JsonSerializerOptions y JsonDocument
using RssAgregator.Blazor.Models;

namespace RssAgregator.Blazor.Services;

// Implementación de IApiService, la unica clase que habla con el Web API.
// Se inyectan por constructor: HttpClient, ILogger y IConfiguration 
public class ApiService : IApiService
{
    private readonly HttpClient _http;               
    private readonly ILogger<ApiService> _logger;      
    private readonly IConfiguration _config;           

    // Mensaje del último error ocurrido
    public string? LastError { get; private set; }

    // Poner bien la deserialización de JSON: PropertyNameCaseInsensitive=true y camelCase
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // CONSTRUCTOR: el contenedor de DI entrega las tres dependencias automáticamente.
    // (Registrado en Program.cs con AddHttpClient<IApiService, ApiService>).
    public ApiService(HttpClient http, ILogger<ApiService> logger, IConfiguration config)
    {
        _http = http;
        _logger = logger;
        _config = config;

        _logger.LogInformation("ApiService iniciado. URL base del API: {Url}",
            _config["ApiSettings:BaseUrl"]);
    }

    // ---------- FEEDS ----------

    public async Task<List<Feed>?> GetFeedsAsync()
    {
        LastError = null; // limpieza de error anterior
        try
        {
            _logger.LogInformation("API GET feeds");                       // log antes de llamar
            // GET a "feeds" (relativa a BaseAddress) y deserializa la lista
            return await _http.GetFromJsonAsync<List<Feed>>("feeds", _json);
        }
        catch (Exception ex) // API apagado, red caída, etc.
        {
            _logger.LogError(ex, "Error obteniendo feeds");                // log del error
            LastError = "No se pudo contactar al Web API.";
            return null; // null = "falló"; las páginas lo detectan
        }
    }

    public async Task<Feed?> CreateFeedAsync(Feed feed)
    {
        LastError = null;
        try
        {
            _logger.LogInformation("API POST feeds ({Name})", feed.Name);
            // POST con el feed serializado a JSON en el body
            var respuesta = await _http.PostAsJsonAsync("feeds", feed, _json);

            if (respuesta.IsSuccessStatusCode) // 201 Created
            {
                // leemos el feed creado (viene con el Id asignado por la BD)
                var nuevo = await respuesta.Content.ReadFromJsonAsync<Feed>(_json);
                _logger.LogInformation("Feed creado con id {Id}", nuevo?.Id);
                return nuevo;
            }

            // 400 (categoría inválida) o 409 (URL duplicada)
            LastError = await LeerErrorAsync(respuesta);
            _logger.LogWarning("El API rechazó el feed: {Error}", LastError);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando el feed {Name}", feed.Name);
            LastError = "No se pudo contactar al Web API.";
            return null;
        }
    }

    public async Task<bool> DeleteFeedAsync(int id)
    {
        LastError = null;
        try
        {
            _logger.LogInformation("API DELETE feeds/{Id}", id);
            var respuesta = await _http.DeleteAsync($"feeds/{id}");
            if (respuesta.IsSuccessStatusCode) return true; // 204 No Content

            LastError = await LeerErrorAsync(respuesta);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando el feed {Id}", id);
            LastError = "No se pudo contactar al Web API.";
            return false;
        }
    }

    public async Task<int> RefreshFeedAsync(int id)
    {
        LastError = null;
        try
        {
            _logger.LogInformation("API POST feeds/{Id}/refresh", id);
            // POST sin body (el endpoint no necesita datos)
            var respuesta = await _http.PostAsync($"feeds/{id}/refresh", null);

            if (respuesta.IsSuccessStatusCode) // 200 OK
            {
                // deserializamos { feedId, articulosNuevos }
                var resultado = await respuesta.Content.ReadFromJsonAsync<RefreshResult>(_json);
                _logger.LogInformation("Feed {Id}: {N} artículos nuevos", id, resultado?.ArticulosNuevos);
                return resultado?.ArticulosNuevos ?? 0;
            }

            LastError = await LeerErrorAsync(respuesta); // ej: URL del RSS rota
            _logger.LogWarning("Refresh rechazado: {Error}", LastError);
            return -1; // -1 = falló
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refrescando el feed {Id}", id);
            LastError = "No se pudo descargar el RSS. Revisa que la URL del feed sea válida.";
            return -1;
        }
    }

    // ---------- ARTÍCULOS ----------

    public async Task<List<Article>?> GetArticlesAsync(string status, string? category)
    {
        LastError = null;
        try
        {
            _logger.LogInformation("API GET articles (status={Status}, category={Category})",
                status, category ?? "todas");

            //  query string: articles?status=new&category=Tech
            var url = $"articles?status={status}";
            if (!string.IsNullOrWhiteSpace(category))
                url += $"&category={Uri.EscapeDataString(category)}"; // escapa caracteres raros

            return await _http.GetFromJsonAsync<List<Article>>(url, _json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo artículos");
            LastError = "No se pudo contactar al Web API.";
            return null;
        }
    }

    public async Task<bool> UpdateArticleFlagsAsync(int id, bool? isRead, bool? isSaved, bool? isDiscarded)
    {
        LastError = null;
        try
        {
            _logger.LogInformation("API PATCH articles/{Id}", id);
            // Body con SO los campos a cambiar
            var body = new { isRead, isSaved, isDiscarded };
            var respuesta = await _http.PatchAsJsonAsync($"articles/{id}", body, _json);

            if (respuesta.IsSuccessStatusCode) return true; // 204

            LastError = await LeerErrorAsync(respuesta);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando el artículo {Id}", id);
            LastError = "No se pudo contactar al Web API.";
            return false;
        }
    }

    // Helper privado: extrae el mensaje de error del body del API.
    // El API contesta 400 con {"error":"..."} o 409 con "texto plano JSON".
    private async Task<string> LeerErrorAsync(HttpResponseMessage respuesta)
    {
        var body = await respuesta.Content.ReadAsStringAsync();
        try
        {
            // Intento 1: es un objeto JSON con propiedad "error" (BadRequest)
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var error))
                return error.GetString() ?? "Error desconocido";
        }
        catch { /* no era un objeto JSON */ }

        // Intento 2:  un string JSON (Conflict)
        return string.IsNullOrWhiteSpace(body)
            ? $"Error HTTP {(int)respuesta.StatusCode}"
            : body.Trim('"');
    }
}