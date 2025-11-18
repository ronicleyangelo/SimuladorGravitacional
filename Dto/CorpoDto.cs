

namespace ProgramacaoAvancada.DTOs
{
    public class CorpoDto
    {
        public string Nome { get; set; } = string.Empty;
        public double Massa { get; set; }
        public double Raio { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }
        
        // ✅ PROPRIEDADES DO SISTEMA REALISTA
        public string Cor { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Densidade { get; set; }
        public double Temperatura { get; set; }
        public double VelocidadeRotacao { get; set; }
        public double AnguloRotacao { get; set; }
        public double Brilho { get; set; } = 1.0;
        public bool EhLuminoso { get; set; }
    }
}