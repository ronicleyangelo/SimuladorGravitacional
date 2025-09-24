namespace ProgramacaoAvancada.Models
{
    public class Corpo
    {
        private static int _contador = 0;
        private static readonly Random _rnd = new();

        public string Nome { get; private set; }
        public double Massa { get; private set; }
        public double Raio { get; private set; }
        public string Cor { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }

        public double VelX { get; set; } = 0; // velocidade inicial = 0
        public double VelY { get; set; } = 0;

        public Corpo(string nome, double massa, double raio, double posX, double posY, string cor)
        {
            Nome = nome;
            Massa = massa;
            Raio = raio;
            PosX = posX;
            PosY = posY;
            Cor = cor;
        }

        //public static Corpo CriarAleatorio(double canvasWidth, double canvasHeight)
        //{
        //    var massa = _rnd.Next(5, 20); // massa aleatória
        //    var raio = massa;       // raio proporcional à massa
        //    var posX = _rnd.NextDouble() * canvasWidth;
        //    var posY = _rnd.NextDouble() * canvasHeight;

        //    var cor = $"rgb({_rnd.Next(256)}, {_rnd.Next(256)}, {_rnd.Next(256)})";

        //    return new Corpo($"Corpo{++_contador}", massa, raio, posX, posY, cor);
        //}

        public static Corpo CriarAleatorio(double canvasWidth, double canvasHeight)
        {
            var massa = _rnd.Next(5, 20);
            var raio = massa;
            var posX = _rnd.NextDouble() * canvasWidth;
            var posY = _rnd.NextDouble() * canvasHeight;

            var cor = $"rgb({_rnd.Next(256)}, {_rnd.Next(256)}, {_rnd.Next(256)})";

            return new Corpo($"Corpo{++_contador}", massa, raio, posX, posY, cor)
            {
                VelX = (_rnd.NextDouble() - 0.5) * 0.1, // velocidade inicial [-1,1]
                VelY = (_rnd.NextDouble() - 0.5) * 0.1
            };
        }

        // Aplica gravidade entre este corpo e outro
        public void AplicarGravidade(Corpo outro, double fatorSimulacao, double deltaTime)
        {
            const double G = 6.674e-11;

            var dx = outro.PosX - PosX;
            var dy = outro.PosY - PosY;
            var dist2 = dx * dx + dy * dy;
            if (dist2 < 1e-6) return;

            var dist = Math.Sqrt(dist2);
            var forca = G * (Massa * outro.Massa) / dist2 * fatorSimulacao;

            var ax = forca / Massa * (dx / dist);
            var ay = forca / Massa * (dy / dist);
            var bx = forca / outro.Massa * (-dx / dist);
            var by = forca / outro.Massa * (-dy / dist);

            // acumula velocidade multiplicando pelo deltaTime
            VelX += ax * deltaTime;
            VelY += ay * deltaTime;
            outro.VelX += bx * deltaTime;
            outro.VelY += by * deltaTime;
        }



        // Atualiza posição baseado na velocidade
        public void AtualizarPosicao(double canvasWidth, double canvasHeight)
        {
            PosX += VelX;
            PosY += VelY;

            // Rebatida nas bordas
            if (PosX - Raio < 0)
            {
                PosX = Raio;
                VelX *= -1;
            }
            else if (PosX + Raio > canvasWidth)
            {
                PosX = canvasWidth - Raio;
                VelX *= -1;
            }

            if (PosY - Raio < 0)
            {
                PosY = Raio;
                VelY *= -1;
            }
            else if (PosY + Raio > canvasHeight)
            {
                PosY = canvasHeight - Raio;
                VelY *= -1;
            }
        }

        public static Corpo Fundir(Corpo a, Corpo b)
        {
            var massa = a.Massa + b.Massa;
            var raio = Math.Sqrt(a.Raio * a.Raio + b.Raio * b.Raio); // continua área proporcional

            // posição média ponderada
            var x = (a.PosX * a.Massa + b.PosX * b.Massa) / massa;
            var y = (a.PosY * a.Massa + b.PosY * b.Massa) / massa;

            // velocidade resultante
            var velX = (a.VelX * a.Massa + b.VelX * b.Massa) / massa;
            var velY = (a.VelY * a.Massa + b.VelY * b.Massa) / massa;

            string cor = MixColors(a.Cor, b.Cor, a.Massa, b.Massa);

            return new Corpo($"Corpo{++_contador}", massa, raio, x, y, cor)
            {
                VelX = velX,
                VelY = velY
            };
        }


        /// <summary>
        /// Faz mistura de duas cores RGB no formato "rgb(r,g,b)" ponderada pelas massas.
        /// </summary>
        private static string MixColors(string corA, string corB, double massaA, double massaB)
        {
            try
            {
                int[] ParseRgb(string cor)
                {
                    var nums = cor.Replace("rgb(", "").Replace(")", "").Split(',');
                    return nums.Select(n => int.Parse(n.Trim())).ToArray();
                }

                var rgbA = ParseRgb(corA);
                var rgbB = ParseRgb(corB);

                double total = massaA + massaB;
                int r = (int)((rgbA[0] * massaA + rgbB[0] * massaB) / total);
                int g = (int)((rgbA[1] * massaA + rgbB[1] * massaB) / total);
                int b = (int)((rgbA[2] * massaA + rgbB[2] * massaB) / total);

                return $"rgb({r},{g},{b})";
            }
            catch
            {
                // fallback: se não conseguir parsear, escolhe a do mais massivo
                return massaA >= massaB ? corA : corB;
            }
        }

    }
}
