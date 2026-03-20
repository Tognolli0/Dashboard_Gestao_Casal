using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MinhaVidaAPI.Services
{
    public class WhatsAppService
    {
        private readonly IConfiguration _config;

        public WhatsAppService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarMensagemParaCasal(string mensagem)
        {
            var sid = _config["Twilio:AccountSid"];
            var token = _config["Twilio:AuthToken"];
            var from = _config["Twilio:FromPhoneNumber"];

            // Inicializa a lista e adiciona apenas se não for nulo
            var numeros = new List<string>();

            var meuNumero = _config["Twilio:MeuNumero"];
            var numeroDela = _config["Twilio:NumeroDela"];

            if (!string.IsNullOrEmpty(meuNumero)) numeros.Add(meuNumero);
            if (!string.IsNullOrEmpty(numeroDela)) numeros.Add(numeroDela);

            // Verifica se as credenciais essenciais do Twilio existem
            if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(from))
            {
                // Log ou retorno silencioso se o serviço não estiver configurado
                return;
            }

            TwilioClient.Init(sid, token);

            foreach (var numero in numeros)
            {
                await MessageResource.CreateAsync(
                    body: mensagem,
                    from: new Twilio.Types.PhoneNumber($"whatsapp:{from}"),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{numero}")
                );
            }
        }
    }
}