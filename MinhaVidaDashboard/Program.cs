using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MinhaVidaDashboard;
using MinhaVidaDashboard.Services;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Habilita suporte a injeção de HttpClientFactory (Resolve erro do KeepAlive)
builder.Services.AddHttpClient();

// 2. Configuração de URL Robusta (Ajustada para as portas do seu launchSettings)
string baseApiUrl = builder.HostEnvironment.IsProduction()
    ? "https://always-together-api.onrender.com" // Produção no Render
    : "http://localhost:5163";                  // Local (Porta do seu launchSettings)

// Limpeza da URL para evitar erros de boot
string finalUrl = baseApiUrl.Trim().TrimEnd('/') + "/";

// 3. Registro do HttpClient Global
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(finalUrl),
    Timeout = TimeSpan.FromSeconds(60)
});

// Registro extra por nome para compatibilidade
builder.Services.AddHttpClient("ClientesAPI", client => {
    client.BaseAddress = new Uri(finalUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

// 4. Serviços e MudBlazor
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<KeepAliveService>();
builder.Services.AddMudServices();

// 5. Cultura Brasil
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

// 6. Inicialização de Background
try
{
    var keepAlive = app.Services.GetRequiredService<KeepAliveService>();
    keepAlive.Iniciar();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro KeepAlive: {ex.Message}");
}

await app.RunAsync();