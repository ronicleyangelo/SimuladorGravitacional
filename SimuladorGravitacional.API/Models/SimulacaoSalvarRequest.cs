using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Models
{
    public class SimulacaoSalvarRequest
    {
        public string Nome { get; set; } = string.Empty;
        public List<Corpo> Corpos { get; set; } = new();
        public int Iteracoes { get; set; }
        public int Colisoes { get; set; }
        public double Gravidade { get; set; }
    }
}