using System;

namespace ProgramacaoAvancada.Models
{
    public class SimulacaoData
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string ConteudoJson { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public int NumeroIteracoes { get; set; }
        public int NumeroColisoes { get; set; }
        public int NumeroCorpos { get; set; }
    }

}