namespace ProgramacaoAvancada.Models
{
    public class Corpo
    {
        private static int _contador = 0;
        private static readonly Random _rnd = new();

        public string Nome { get; private set; }
        public double Massa { get; private set; }
        public double Densidade { get; private set; }
        public double Raio { get; private set; }
        public string Cor { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; } = 0; // ✅ Velocidade inicial sempre zero
        public double VelY { get; set; } = 0; // ✅ Velocidade inicial sempre zero

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
        }

        public static Corpo CriarAleatorio(double canvasWidth, double canvasHeight)
        {
            // ✅ 5 tipos de corpos celestes diferentes
            var tipoCorpo = _rnd.Next(0, 5); // 0=Asteroides, 1=Luas, 2=Planetas, 3=Gigantes, 4=Estrelas

            double massa, densidade;
            string prefixoNome = "Corpo";

            switch (tipoCorpo)
            {
                case 0: // Asteroides - muito pequenos
                    massa = _rnd.NextDouble() * 3 + 0.5; // 0.5-3.5
                    densidade = _rnd.NextDouble() * 1.5 + 0.5; // 0.5-2.0
                    prefixoNome = "Asteroide";
                    break;

                case 1: // Luas - pequenas
                    massa = _rnd.NextDouble() * 7 + 1; // 1-8
                    densidade = _rnd.NextDouble() * 2 + 1; // 1-3
                    prefixoNome = "Lua";
                    break;

                case 2: // Planetas - médios
                    massa = _rnd.NextDouble() * 20 + 5; // 5-25
                    densidade = _rnd.NextDouble() * 3 + 1; // 1-4
                    prefixoNome = "Planeta";
                    break;

                case 3: // Gigantes gasosos - grandes
                    massa = _rnd.NextDouble() * 50 + 20; // 20-70
                    densidade = _rnd.NextDouble() * 2 + 0.5; // 0.5-2.5 (menos densos)
                    prefixoNome = "Gigante";
                    break;

                case 4: // Estrelas - muito grandes
                    massa = _rnd.NextDouble() * 100 + 50; // 50-150
                    densidade = _rnd.NextDouble() * 4 + 1; // 1-5
                    prefixoNome = "Estrela";
                    break;

                default:
                    massa = _rnd.Next(5, 15);
                    densidade = _rnd.NextDouble() * 3 + 1;
                    break;
            }

            // ✅ Distribuição uniforme
            var posX = _rnd.NextDouble() * canvasWidth;
            var posY = _rnd.NextDouble() * canvasHeight;

            // ✅ Cores temáticas baseadas no tipo
            string cor = tipoCorpo switch
            {
                0 => $"rgb({_rnd.Next(120, 180)}, {_rnd.Next(120, 180)}, {_rnd.Next(140, 200)})", // Cinza azulado
                1 => $"rgb({_rnd.Next(150, 200)}, {_rnd.Next(140, 190)}, {_rnd.Next(120, 170)})", // Marrom acinzentado
                2 => $"rgb({_rnd.Next(180, 240)}, {_rnd.Next(160, 220)}, {_rnd.Next(100, 160)})", // Terroso
                3 => $"rgb({_rnd.Next(220, 256)}, {_rnd.Next(180, 230)}, {_rnd.Next(100, 150)})", // Laranja
                4 => $"rgb({_rnd.Next(240, 256)}, {_rnd.Next(220, 256)}, {_rnd.Next(100, 180)})", // Amarelo brilhante
                _ => $"rgb({_rnd.Next(150, 256)}, {_rnd.Next(150, 256)}, {_rnd.Next(150, 256)})"
            };

            // ✅ Velocidade inicial SEMPRE ZERO (conforme especificação)
            var corpo = new Corpo($"{prefixoNome}{++_contador}", massa, densidade, posX, posY, cor)
            {
                VelX = 0, // ✅ Velocidade inicial zero
                VelY = 0  // ✅ Velocidade inicial zero
            };

            return corpo;
        }

        // ✅ MÉTODO ALTERNATIVO: Para criar corpos com distribuição mais espalhada
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

        // Aplica gravidade entre este corpo e outro
        public void AplicarGravidade(Corpo outro, double fatorSimulacao, double deltaTime)
        {
            const double G = 6.674e-11;

            var dx = outro.PosX - PosX;
            var dy = outro.PosY - PosY;
            var dist2 = dx * dx + dy * dy;

            // Distância mínima para evitar divisão por zero
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
            // Quantidade de movimento total em X = m1*v1x + m2*v2x
            var qtdMovimentoX = (a.Massa * a.VelX) + (b.Massa * b.VelX);
            var qtdMovimentoY = (a.Massa * a.VelY) + (b.Massa * b.VelY);

            // Velocidade do novo corpo = quantidade de movimento total / massa total
            var velX = qtdMovimentoX / massa;
            var velY = qtdMovimentoY / massa;

            string cor = MixColors(a.Cor, b.Cor, a.Massa, b.Massa);

            return new Corpo($"Corpo{++_contador}", massa, densidade, x, y, cor)
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
