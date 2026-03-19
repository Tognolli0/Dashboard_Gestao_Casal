using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Services;
using MinhaVidaAPI.Workers;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// 1. SQLite otimizado com WAL mode (Write-Ahead Logging = leituras mais rápidas)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=minhavida.db;Cache=Shared")
           .EnableSensitiveDataLogging(false)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)); // NoTracking por padrão

// 2. Compressão de resposta (reduz tamanho do JSON em ~70%)
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

// 3. Serviços
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<OCRService>();
builder.Services.AddHostedService<ResumoWorker>();

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Livre", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Banco + índices
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Ativa pragmas de performance no SQLite
    // Cada PRAGMA em conexão separada para evitar conflito com transações
    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();
    using (var cmd = conn.CreateCommand())
    {
        // journal_mode e synchronous DEVEM ser executados fora de transação
        cmd.CommandText = "PRAGMA journal_mode=WAL;";
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = "PRAGMA synchronous=NORMAL;";
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = "PRAGMA cache_size=-64000;";
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = "PRAGMA temp_store=MEMORY;";
        await cmd.ExecuteNonQueryAsync();
    }
    await conn.CloseAsync();
}

// 6. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression(); // deve vir antes do UseCors
app.UseCors("Livre");
app.UseAuthorization();
app.MapControllers();

app.Run();