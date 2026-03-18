using MudBlazor;
using System.Runtime.CompilerServices;

namespace MinhaVidaDashboard.Models
{
    public class Desejo
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime DataAlvo { get; set; } = DateTime.Now.AddMonths(1);
        public string Icone { get; set; } = Icons.Material.Filled.Favorite;
        public bool Concluido { get; set; }
    }
}
