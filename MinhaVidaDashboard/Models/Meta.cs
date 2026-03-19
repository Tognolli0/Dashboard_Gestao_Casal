namespace MinhaVidaDashboard.Models
{
    public class Meta
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public double ValorObjetivo { get; set; }
        public double ValorGuardado { get; set; }
        public string Responsavel { get; set; } = "Casal";

        // Propriedade calculada (Não vai para o banco, só ajuda no C#)
        public double Porcentagem => ValorObjetivo > 0
            ? (ValorGuardado / ValorObjetivo) * 100
            : 0;
    }
}