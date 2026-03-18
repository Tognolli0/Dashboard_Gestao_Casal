namespace MinhaVidaDashboard.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Responsavel { get; set; } = "Eu";
        public string Categoria { get; set; } = "Geral";
        public string Tipo { get; set; } = "Saída";
        public bool EhPessoal { get; set; } = false;
    }
}