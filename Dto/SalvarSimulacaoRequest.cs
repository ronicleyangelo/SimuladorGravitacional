using ProgramacaoAvancada.Models;
namespace ProgramacaoAvancada.DTOs
{
        public class SalvarSimulacaoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public int NumeroCorpos { get; set; }
        public int Iteracoes { get; set; }
        public int Colisoes { get; set; }
        public double Gravidade { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public List<CorpoDto> Corpos { get; set; } = new List<CorpoDto>(); // ✅ Corrigido
    }
}

