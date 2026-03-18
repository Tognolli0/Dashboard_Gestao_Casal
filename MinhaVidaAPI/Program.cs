using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Models;
using MinhaVidaAPI.Services; // Adicionado
using MinhaVidaAPI.Workers;  // Adicionado

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=minhavida.db"));

// --- NOVOS SERVIÇOS ADICIONADOS ---
// Registra o serviço que envia as mensagens
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<OCRService>();
// Registra o robô que roda em segundo plano para o resumo semanal
builder.Services.AddHostedService<ResumoWorker>();
// ----------------------------------

// 2. Configuração de CORS para o Blazor conseguir acessar
builder.Services.AddCors(options => {
    options.AddPolicy("Livre", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. CRIAÇÃO AUTOMÁTICA DO BANCO
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 4. Configuração do Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Livre");
app.UseAuthorization();
app.MapControllers();

app.Run();