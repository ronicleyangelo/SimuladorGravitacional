using System.Text.Json.Serialization;

namespace ProgramacaoAvancada.Models
{
    public class SimulacaoSnapshot
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeCorpos { get; set; }
        public int NumeroIteracoes { get; set; }
        public int NumeroColisoes { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Lista de corpos desta simulação
        public List<Corpo> Corpos { get; set; } = new();

        // Para exibição na UI
        [JsonIgnore]
        public string DataFormatada => DataCriacao.ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string Resumo => $"{QuantidadeCorpos} corpos, {NumeroIteracoes} iterações, {NumeroColisoes} colisões";
    }
    
        public class SimulacaoSalvarRequest
    {
        public string Nome { get; set; } = string.Empty;
        public List<Corpo> Corpos { get; set; } = new();
        public int Iteracoes { get; set; }
        public int Colisoes { get; set; }
        public double Gravidade { get; set; }
        
        [JsonIgnore]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}