using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MinhaVidaDashboard;
using MinhaVidaDashboard.Services;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Configuração da URL da API (Verifique se o link do Render está EXATAMENTE assim)
var apiUrl = builder.HostEnvironment.IsProduction()
    ? "https://always-together-api.onrender.com/"
    : "http://localhost:5163/";

// 2. Configuração do HttpClient Nomeado "ClientesAPI"
builder.Services.AddHttpClient("ClientesAPI", client =>
{
    client.BaseAddress = new Uri(apiUrl);
    // Aumentado para 60s para dar tempo do Render acordar (Cold Start)
    client.Timeout = TimeSpan.FromSeconds(60);
});

// 3. ESSA LINHA RESOLVE O ERRO DE PÁGINA EM BRANCO:
// Registra o HttpClient padrão que as páginas e serviços injetam, 
// fazendo-o usar a configuração da "ClientesAPI" (URL do Render).
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ClientesAPI"));

// 4. Cache e Keep-alive (Singleton para persistir no App)
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<KeepAliveService>();

// 5. Configuração do MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
});

// 6. Configuração de Cultura (Brasil)
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

// 7. Inicia o Keep-alive para tentar manter a API do Render acordada
try
{
    var keepAlive = app.Services.GetRequiredService<KeepAliveService>();
    keepAlive.Iniciar();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao iniciar KeepAlive: {ex.Message}");
}

await app.RunAsync();