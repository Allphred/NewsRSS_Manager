namespace RssAgregator.Blazor.Models;

// Respuesta del endpoint POST /api/feeds/{id}/refresh
// (el API contesta: { "feedId": 1, "articulosNuevos": 30 })
public record RefreshResult(int FeedId, int ArticulosNuevos);