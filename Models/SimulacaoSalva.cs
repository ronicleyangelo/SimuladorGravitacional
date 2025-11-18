using System;
using ProgramacaoAvancada.DTOs;
namespace ProgramacaoAvancada.Models
{
    public class SimulacaoSalva
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int NumeroCorpos { get; set; }
        public int Iteracoes { get; set; }
        public int Colisoes { get; set; }
        public double Gravidade { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<CorpoDto> Corpos { get; set; } = new List<CorpoDto>();
    }
}