using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
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

// 3. Injeção de Dependência dos Serviços
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<OCRService>();
builder.Services.AddHostedService<ResumoWorker>();

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Livre", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 5. Swagger
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

// 7. Pipeline de Middleware — ordem crítica
// CORS deve ser o PRIMEIRO middleware
app.UseCors("Livre");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinhaVida API v1");
    c.RoutePrefix = string.Empty;
});

// Compressão DEPOIS do CORS
app.UseResponseCompression();

app.UseAuthorization();
app.MapControllers();

app.Run();