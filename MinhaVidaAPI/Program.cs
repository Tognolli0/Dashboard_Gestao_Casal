using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Services;
using MinhaVidaAPI.Workers;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// 2. Compressão de Resposta (Performance)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);

// 3. Injeção de Dependência dos seus Serviços
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<OCRService>();
builder.Services.AddHostedService<ResumoWorker>();

// 4. Configuração do CORS (Ajustado para o Netlify)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Livre", policy =>
        policy.WithOrigins(
            "https://localhost:7065",
            "http://localhost:5000",
            "https://always-together.netlify.app",    // Sem barra
            "https://always-together.netlify.app/"    // Com barra final
        )
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 5. Configuração do Swagger (Mesmo em Produção)
// No Program.cs da API (Linha 55)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MinhaVida API", Version = "v1" });
});

var app = builder.Build();

// 6. Migração/Criação Automática do Banco ao Iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 5;
    while (retries > 0)
    {
        try
        {
            Console.WriteLine("Tentando conectar ao banco de dados...");
            db.Database.EnsureCreated();
            Console.WriteLine("Banco de dados pronto.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Falha na conexão com o banco. Tentativas restantes: {retries}. Erro: {ex.Message}");
            if (retries == 0) throw;
            await Task.Delay(4000);
        }
    }
}

// 7. Configuração do Pipeline (Middleware)
// Ativamos o Swagger fora do IF para ele aparecer no Render
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinhaVida API v1");
    c.RoutePrefix = string.Empty; // Isso faz o Swagger ser a página inicial da API
});

app.UseResponseCompression();

// A ordem aqui é CRÍTICA: CORS deve vir antes de Authorization e Controllers
app.UseCors("Livre");

app.UseAuthorization();
app.MapControllers();

app.Run();