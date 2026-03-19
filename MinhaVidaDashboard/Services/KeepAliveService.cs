namespace MinhaVidaDashboard.Services
{
    /// <summary>
    /// Serviço que mantém a API "acordada" fazendo um ping a cada 4 minutos.
    /// Evita o cold start do .NET que causa lentidão na primeira requisição.
    /// </summary>
    public class KeepAliveService : IDisposable
    {
        private readonly IHttpClientFactory _clientFactory;
        private Timer? _timer;
        private bool _iniciado = false;

        public KeepAliveService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void Iniciar()
        {
            if (_iniciado) return;
            _iniciado = true;

            // Primeiro ping após 30 segundos, depois a cada 4 minutos
            _timer = new Timer(async _ => await PingAsync(), null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(4));
        }

        private async Task PingAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ClientesAPI");
                // Usa o endpoint de resumo que já existe — leve e rápido
                await client.GetAsync("api/dashboard/resumo");
            }
            catch
            {
                // Silencioso — se a API estiver fora não precisa travar nada
            }
        }

        public void Dispose() => _timer?.Dispose();
    }
}