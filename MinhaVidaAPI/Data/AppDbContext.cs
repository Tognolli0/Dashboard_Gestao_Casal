using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Models; // Já vamos criar essa pasta e modelo

namespace MinhaVidaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Esta é a tabela que guardará os gastos seus e do Secret Studio
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<Desejo> Desejos { get; set; }
    }
}