using System.Text.Json.Serialization;

namespace ProgramacaoAvancada.Models
{
    public class Corpo
    {
        private static readonly Random _rnd = new();
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string Nome { get; private set; } = string.Empty;
        public double Massa { get; private set; }
        public double Densidade { get; private set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }

        [JsonIgnore]
        public double Raio => CalcularRaio();

        public string Cor { get; private set; } = GerarCor();

        private double CalcularRaio()
        {
            if (Densidade <= 0) return 4;
            var volume = Massa / Densidade;
            var r = Math.Cbrt((3.0 * volume) / (4.0 * Math.PI));
            if (double.IsNaN(r) || r <= 0) return 4;
            return Math.Max(4, r * 3.0);
        }

        private static string GerarCor()
        {
            int h = _rnd.Next(0, 360);
            return $"hsl({h}, 80%, 60%)";
        }

        // -----------------------
        // FACTORY
        // -----------------------
        public static Corpo CriarAleatorio(double canvasWidth, double canvasHeight)
        {
            var nomes = new[] {
                "Sol","Terra","Lua","Marte","Júpiter","Saturno","Urano","Netuno",
                "Vênus","Mercúrio","Plutão","Éris","Haumea","Makemake","Ceres"
            };
            var nome = nomes[_rnd.Next(nomes.Length)] + "-" + _rnd.Next(100);
            var massa = _rnd.NextDouble() * 50.0 + 10.0;
            var dens = _rnd.NextDouble() * 5.0 + 1.0;

            return new Corpo
            {
                Nome = nome,
                Massa = massa,
                Densidade = dens,
                PosX = _rnd.NextDouble() * canvasWidth,
                PosY = _rnd.NextDouble() * canvasHeight,
                VelX = (_rnd.NextDouble() - 0.5) * 2.0,
                VelY = (_rnd.NextDouble() - 0.5) * 2.0
            };
        }

        // -----------------------
        // FÍSICA
        // -----------------------

        public void AplicarGravidade(Corpo outro, double gravidade, double G)
        {
            var dx = outro.PosX - PosX;
            var dy = outro.PosY - PosY;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist <= 5.0) return;

            var forca = gravidade * G * Massa * outro.Massa / (dist * dist);
            var fx = forca * dx / dist;
            var fy = forca * dy / dist;

            VelX += fx / Massa;
            VelY += fy / Massa;

            outro.VelX -= fx / outro.Massa;
            outro.VelY -= fy / outro.Massa;
        }

        public void AtualizarPosicao(double velocidade, double canvasWidth, double canvasHeight)
        {
            PosX += VelX * velocidade;
            PosY += VelY * velocidade;

            if (PosX - Raio < 0)
            {
                PosX = Raio;
                VelX *= -0.9;
            }
            else if (PosX + Raio > canvasWidth)
            {
                PosX = canvasWidth - Raio;
                VelX *= -0.9;
            }

            if (PosY - Raio < 0)
            {
                PosY = Raio;
                VelY *= -0.9;
            }
            else if (PosY + Raio > canvasHeight)
            {
                PosY = canvasHeight - Raio;
                VelY *= -0.9;
            }
        }

        public static Corpo Fundir(Corpo a, Corpo b)
        {
            Corpo maior, menor;
            if (a.Massa >= b.Massa) { maior = a; menor = b; }
            else { maior = b; menor = a; }

            maior.VelX = (maior.Massa * maior.VelX + menor.Massa * menor.VelX) / (maior.Massa + menor.Massa);
            maior.VelY = (maior.Massa * maior.VelY + menor.Massa * menor.VelY) / (maior.Massa + menor.Massa);

            maior.Massa += menor.Massa;

            var volumeMaior = maior.Massa / maior.Densidade;
            var volumeMenor = menor.Massa / menor.Densidade;
            var volumeTotal = volumeMaior + volumeMenor;
            if (volumeTotal > 0)
                maior.Densidade = maior.Massa / volumeTotal;

            maior.PosX = (maior.PosX * (maior.Massa - menor.Massa) + menor.PosX * menor.Massa) / maior.Massa;
            maior.PosY = (maior.PosY * (maior.Massa - menor.Massa) + menor.PosY * menor.Massa) / maior.Massa;

            return maior;
        }
    }
}
