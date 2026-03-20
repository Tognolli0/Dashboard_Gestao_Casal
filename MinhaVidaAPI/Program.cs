using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Services;
using MinhaVidaAPI.Workers;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DE CONEXÃO CORRIGIDA ---
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (string.IsNullOrEmpty(databaseUrl))
{
    [cite_start]// Local: Usa a string do appsettings.json [cite: 361]
    // Certifique-se de remover "No IP version preference" do seu appsettings.json manual
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
else
{
    // Render/Produção: Converte a URL postgres:// para o formato Npgsql
    // Isso evita o erro de "KeyNotFoundException" e "ArgumentException"
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    connectionString = $"Host={databaseUri.Host};" +
                       $"Port={databaseUri.Port};" +
                       $"Database={databaseUri.PathAndQuery.TrimStart('/')};" +
                       $"Username={userInfo[0]};" +
                       $"Password={userInfo[1]};" +
                       $"SSL Mode=Require;Trust Server Certificate=true;";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
// --- FIM DA CONFIGURAÇÃO DE CONEXÃO ---

// 2. Compressão de resposta
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

// 5. Migrações automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // No PostgreSQL em produção, EnsureCreated cria as tabelas se não existirem
    db.Database.EnsureCreated();
}

// 6. Pipeline
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