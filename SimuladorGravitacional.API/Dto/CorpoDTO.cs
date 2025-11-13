namespace ProgramacaoAvancada.DTOs
{
    public class CorpoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Massa { get; set; }
        public double Densidade { get; set; }
        public double Raio { get; set; }
        public string Cor { get; set; } = "rgb(255,255,255)";
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public double VelocidadeX { get; set; }
        public double VelocidadeY { get; set; }
        public int UniversoId { get; set; }
    }

    public class CreateCorpoDto
    {
        public string Nome { get; set; } = string.Empty;
        public double Massa { get; set; }
        public double Densidade { get; set; }
        public double Raio { get; set; }
        public string Cor { get; set; } = "rgb(255,255,255)";
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public double VelocidadeX { get; set; } = 0;
        public double VelY { get; set; } = 0;
        public int VelocidadeY { get; set; }
    }
}