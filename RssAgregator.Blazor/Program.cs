using RssAgregator.Blazor.Components;
using RssAgregator.Blazor.Services;     // IApiService y ApiService

var builder = WebApplication.CreateBuilder(args);

// Registra Blazor en modo interactivo Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra HttpClient TIPADO para servicios
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    var url = builder.Configuration["ApiSettings:BaseUrl"]; 
    client.BaseAddress = new Uri(url!);   
    client.Timeout = TimeSpan.FromSeconds(30); // evita frizarse si el RSS tarda mucho
});

var app = builder.Build();


app.UseHttpsRedirection(); 

app.UseStaticFiles();     

app.UseAntiforgery();      // protege los formularios contra CSRF

// Conecta las rutas de los .razor y activa el modo Server
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();