using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RssAgregator.Api.Data;
using RssAgregator.Api.Services;

// Crea el generador de la aplicación y carga la configuración del entorno.
var builder = WebApplication.CreateBuilder(args);

// Registra los controladores y configura la serialización JSON para ignorar
// referencias circulares entre entidades relacionadas.
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Habilita la exploración de endpoints y genera la documentación OpenAPI/Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra el contexto de Entity Framework Core con la conexión SQLite
// definida como "DefaultConnection" en la configuración de la aplicación.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra el servicio RSS con ciclo de vida scoped: una instancia por solicitud.
builder.Services.AddScoped<IRssService, RssService>();

// Construye la aplicación con los servicios y la configuración registrados.
var app = builder.Build();

// Expone Swagger y su interfaz de usuario únicamente durante el desarrollo.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige las solicitudes HTTP a HTTPS cuando se habilite esta configuración.
//app.UseHttpsRedirection();

// Activa el middleware de autorización cuando se configure la autenticación.
//app.UseAuthorization();

// Asocia las rutas de los controladores con el pipeline HTTP.
app.MapControllers();

// Inicia el servidor web y mantiene la aplicación atendiendo solicitudes.
app.Run();


