using MinhaVidaDashboard.Models;

namespace MinhaVidaDashboard.Services
{
    /// <summary>
    /// Cache em memória para os dados do dashboard.
    /// Evita rebuscar da API a cada navegação entre páginas.
    /// </summary>
    public class CacheService
    {
        private DashboardCache? _cache;
        private DateTime _ultimaAtualizacao = DateTime.MinValue;
        private readonly TimeSpan _expiracao = TimeSpan.FromMinutes(2);

        public bool TemCacheValido =>
            _cache != null && DateTime.Now - _ultimaAtualizacao < _expiracao;

        public DashboardCache? ObterCache() => TemCacheValido ? _cache : null;

        public void SalvarCache(DashboardCache dados)
        {
            _cache = dados;
            _ultimaAtualizacao = DateTime.Now;
        }

        public void Invalidar()
        {
            _cache = null;
            _ultimaAtualizacao = DateTime.MinValue;
        }

        public TimeSpan TempoRestante =>
            TemCacheValido ? _expiracao - (DateTime.Now - _ultimaAtualizacao) : TimeSpan.Zero;
    }

    public class DashboardCache
    {
        public List<Transacao> TransacoesEu { get; set; } = new();
        public List<Transacao> TransacoesDela { get; set; } = new();
        public List<Meta> Metas { get; set; } = new();
        public List<Desejo> Desejos { get; set; } = new();
        public DateTime BuscadoEm { get; set; } = DateTime.Now;
    }
}