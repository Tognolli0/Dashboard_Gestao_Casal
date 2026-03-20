using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Services;
using MinhaVidaAPI.Workers;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

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

builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<OCRService>();
builder.Services.AddHostedService<ResumoWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Livre", policy =>
        policy.WithOrigins(
            "https://localhost:7065",
            "http://localhost:5000",
            "https://always-together.netlify.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
            if (retries == 0)
            {
                Console.WriteLine("Não foi possível conectar ao banco após todas as tentativas. Encerrando.");
                throw;
            }
            await Task.Delay(4000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseCors("Livre");
app.UseAuthorization();
app.MapControllers();

app.Run();