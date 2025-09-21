namespace ProgramacaoAvancada.Models
{
    public class Corpo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; } = string.Empty;
        public double Massa { get; set; }
        public double Raio { get; set; }
        public double Densidade { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }
        public double Velz { get; set; }

    }

    public class GerenciadorGravitacional
    {
        public double ConstanteGravitacional { get; set; } = 6.67430e-11;
        public List<Corpo> Corpos { get; set; } = new List<Corpo>();

        // Atualiza posições e velocidades
        public void Atualizar(double deltaTime)
        {
            // Para cada corpo, calcular a força resultante
            foreach (var corpoA in Corpos)
            {
                double forcaX = 0, forcaY = 0, forcaZ = 0;

                foreach (var corpoB in Corpos)
                {
                    if (corpoA == corpoB) continue;

                    // Vetor distância
                    double dx = corpoB.PosX - corpoA.PosX;
                    double dy = corpoB.PosY - corpoA.PosY;
                    double dz = corpoB.PosZ - corpoA.PosZ;

                    double distancia = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    if (distancia == 0) continue;

                    // Força gravitacional
                    double forca = ConstanteGravitacional * (corpoA.Massa * corpoB.Massa) / (distancia * distancia);

                    // Normaliza vetor distância e aplica força
                    forcaX += forca * (dx / distancia);
                    forcaY += forca * (dy / distancia);
                    forcaZ += forca * (dz / distancia);
                }

                // Aceleração = Força / Massa
                double acelX = forcaX / corpoA.Massa;
                double acelY = forcaY / corpoA.Massa;
                double acelZ = forcaZ / corpoA.Massa;

                // Atualiza velocidade
                corpoA.VelX += acelX * deltaTime;
                corpoA.VelY += acelY * deltaTime;
                corpoA.VelZ += acelZ * deltaTime;

                // Atualiza posição
                corpoA.PosX += corpoA.VelX * deltaTime;
                corpoA.PosY += corpoA.VelY * deltaTime;
                corpoA.PosZ += corpoA.VelZ * deltaTime;
            }
        }
    }

}


