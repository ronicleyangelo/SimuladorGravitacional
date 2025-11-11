using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramacaoAvancada.Models
{
    public class Corpo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        private static int _contador = 0;
        private static readonly Random _rnd = new();

        public string Nome { get; private set; }
        public double Massa { get; private set; }
        public double Densidade { get; private set; }
        public double Raio { get; private set; }
        public string Cor { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; } = 0;
        public double VelY { get; set; } = 0;

        public Corpo(string nome, double massa, double densidade, double posX, double posY, string cor)
        {
            Nome = nome;
            Massa = massa;
            Densidade = densidade;
            PosX = posX;
            PosY = posY;
            Cor = cor;

            // Cálculo do raio a partir da massa e densidade
            double volume = massa / densidade;
            Raio = Math.Pow((3.0 * volume) / (4.0 * Math.PI), 1.0 / 3.0);

            // ✅ VALIDA VALORES IMEDIATAMENTE
            ValidarValores();
        }

        // ✅ MÉTODO PARA VALIDAR E CORRIGIR VALORES NUMÉRICOS
        public void ValidarValores()
        {
            // Garante que não há valores infinitos ou NaN
            if (double.IsInfinity(PosX) || double.IsNaN(PosX)) PosX = 0;
            if (double.IsInfinity(PosY) || double.IsNaN(PosY)) PosY = 0;
            if (double.IsInfinity(VelX) || double.IsNaN(VelX)) VelX = 0;
            if (double.IsInfinity(VelY) || double.IsNaN(VelY)) VelY = 0;
            if (double.IsInfinity(Massa) || double.IsNaN(Massa)) Massa = 1;
            if (double.IsInfinity(Raio) || double.IsNaN(Raio)) Raio = 5;
            if (double.IsInfinity(Densidade) || double.IsNaN(Densidade)) Densidade = 1;

            // Limita valores muito extremos
            Massa = Math.Max(0.1, Math.Min(1000, Massa));
            Raio = Math.Max(1, Math.Min(100, Raio));
            Densidade = Math.Max(0.1, Math.Min(20, Densidade));
        }

        // ✅ MÉTODO PARA SER CHAMADO ANTES DA SERIALIZAÇÃO JSON
        public void PrepararParaSerializacao()
        {
            ValidarValores();
        }

        // ... (mantenha todos os outros métodos existentes: CriarAleatorio, CriarDistribuido, AplicarGravidade, etc.)

        public static Corpo Fundir(Corpo a, Corpo b)
        {
            // ✅ MASSA: Soma das massas
            var massa = a.Massa + b.Massa;

            // ✅ DENSIDADE: Média ponderada pelas massas
            var densidade = (a.Densidade * a.Massa + b.Densidade * b.Massa) / massa;

            // ✅ POSIÇÃO: Centro de massa
            var x = (a.PosX * a.Massa + b.PosX * b.Massa) / massa;
            var y = (a.PosY * a.Massa + b.PosY * b.Massa) / massa;

            // ✅ VELOCIDADE: Conservação da quantidade de movimento
            var qtdMovimentoX = (a.Massa * a.VelX) + (b.Massa * b.VelX);
            var qtdMovimentoY = (a.Massa * a.VelY) + (b.Massa * b.VelY);

            var velX = qtdMovimentoX / massa;
            var velY = qtdMovimentoY / massa;

            string cor = MixColors(a.Cor, b.Cor, a.Massa, b.Massa);

            var corpoFundido = new Corpo($"Corpo{++_contador}", massa, densidade, x, y, cor)
            {
                VelX = velX,
                VelY = velY
            };

            // ✅ VALIDA O CORPO FUNDIDO
            corpoFundido.ValidarValores();

            return corpoFundido;
        }

        // ✅ ADICIONE ESTES MÉTODOS À CLASSE CORPO:

        // Método para aplicar gravidade (que está sendo chamado no Universo)
        public void AplicarGravidade(Corpo outro, double constanteGravitacional, double deltaTime)
        {
            const double G = 6.674e-11;

            var dx = outro.PosX - PosX;
            var dy = outro.PosY - PosY;
            var dist2 = dx * dx + dy * dy;

            // Distância mínima para evitar divisão por zero
            if (dist2 < 1e-6) return;

            var dist = Math.Sqrt(dist2);
            var forca = G * (Massa * outro.Massa) / dist2 * constanteGravitacional;

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

        // ✅ TORNE ESTE MÉTODO PÚBLICO (adicione 'public' antes de 'static')
        public static Corpo CriarDistribuido(double canvasWidth, double canvasHeight, int index, int totalCorpos)
        {
            var massa = _rnd.Next(5, 20);
            var densidade = _rnd.NextDouble() * 4 + 1;

            // Distribuição em grid para evitar aglomeração
            var colunas = (int)Math.Ceiling(Math.Sqrt(totalCorpos));
            var linhas = (int)Math.Ceiling((double)totalCorpos / colunas);

            var coluna = index % colunas;
            var linha = index / colunas;

            var posX = (coluna + 0.5 + (_rnd.NextDouble() - 0.5) * 0.3) * (canvasWidth / colunas);
            var posY = (linha + 0.5 + (_rnd.NextDouble() - 0.5) * 0.3) * (canvasHeight / linhas);

            // Garantir que fique dentro dos limites
            posX = Math.Max(10, Math.Min(canvasWidth - 10, posX));
            posY = Math.Max(10, Math.Min(canvasHeight - 10, posY));

            var cor = $"rgb({_rnd.Next(150, 256)}, {_rnd.Next(150, 256)}, {_rnd.Next(150, 256)})";

            // ✅ Velocidade inicial SEMPRE ZERO
            return new Corpo($"Corpo{++_contador}", massa, densidade, posX, posY, cor)
            {
                VelX = 0, // ✅ Velocidade inicial zero
                VelY = 0  // ✅ Velocidade inicial zero
            };
        }

        // Método para atualizar posição (que está sendo chamado no Universo)
        public void AtualizarPosicao(double canvasWidth, double canvasHeight)
        {
            PosX += VelX;
            PosY += VelY;

            // Rebatida nas bordas com amortecimento
            if (PosX - Raio < 0)
            {
                PosX = Raio;
                VelX *= -0.8; // Amortecimento
            }
            else if (PosX + Raio > canvasWidth)
            {
                PosX = canvasWidth - Raio;
                VelX *= -0.8;
            }

            if (PosY - Raio < 0)
            {
                PosY = Raio;
                VelY *= -0.8;
            }
            else if (PosY + Raio > canvasHeight)
            {
                PosY = canvasHeight - Raio;
                VelY *= -0.8;
            }
        }

        // ✅ MÉTODO MixColors (que está faltando)
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