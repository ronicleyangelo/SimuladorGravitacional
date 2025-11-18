namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// 🌌 Corpo Celeste com Física Realista - VERSÃO ULTRA OTIMIZADA
    /// </summary>
    public class Corpo
    {
        private static int _contador = 0;
        private static readonly Random _rnd = new();
        private static readonly Dictionary<string, int[]> _cacheCores = new();

        // ========== PROPRIEDADES FÍSICAS ==========
        public string Nome { get; set; }
        public TipoCorpo Tipo { get; set; }
        public double Massa { get; set; }
        public double Densidade { get; set; }
        public double Raio { get; set; }
        public string Cor { get; set; }
        
        // ========== CINEMÁTICA ==========
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }
        
        // ========== ROTAÇÃO E VISUAL ==========
        public double VelocidadeRotacao { get; set; }
        public double AnguloRotacao { get; set; }
        public double Brilho { get; set; }
        public double PulsacaoBrilho { get; set; }
        
        // ========== TEMPERATURA E COR DINÂMICA ==========
        public double Temperatura { get; set; }
        public bool EhLuminoso { get; set; }

        // ========== CACHE PARA CÁLCULOS ==========
        private double _raioQuadrado;
        private double _massaInversa;

        // ========== CONSTANTES OTIMIZADAS ==========
        private const double G_REAL = 6.674e-11;
        private const double COEFICIENTE_RESTITUICAO = 0.75;
        private const double FATOR_AMORTECIMENTO = 0.95;
        private const double LIMITE_ROTACAO = 0.2;

        // ========== CONSTRUTOR OTIMIZADO ==========
        public Corpo(string nome, double massa, double densidade, double posX, double posY, string cor)
        {
            Nome = nome;
            Massa = massa;
            Densidade = densidade;
            PosX = posX;
            PosY = posY;
            Cor = cor;

            // ✅ CÁLCULOS OTIMIZADOS
            CalcularPropriedadesDerivadas();
        }

        // ========== MÉTODO DE CÁLCULOS OTIMIZADO ==========
        private void CalcularPropriedadesDerivadas()
        {
            // Cálculo do raio otimizado
            double volume = Massa / Densidade;
            Raio = Math.Pow(0.238732414 * volume, 0.333333333); // (3/(4π))^(1/3) ≈ 0.238732414
            _raioQuadrado = Raio * Raio;
            _massaInversa = 1.0 / Massa;

            // Tipo inferido por massa com lookup table
            Tipo = Massa switch
            {
                < 4 => TipoCorpo.Asteroide,
                < 12 => TipoCorpo.Lua,
                < 30 => TipoCorpo.PlanetaRochoso,
                < 85 => TipoCorpo.GiganteGasoso,
                _ => TipoCorpo.Estrela
            };

            // Temperatura baseada em lookup table
            Temperatura = Tipo switch
            {
                TipoCorpo.Asteroide => 250,
                TipoCorpo.Cometa => 150,
                TipoCorpo.Lua => 250,
                TipoCorpo.PlanetaRochoso => 350,
                TipoCorpo.GiganteGasoso => 150,
                TipoCorpo.GiganteGelo => 80,
                TipoCorpo.AnaBranca => 15000,
                TipoCorpo.Estrela => 5778,
                _ => 300
            };

            EhLuminoso = Tipo == TipoCorpo.Estrela || Tipo == TipoCorpo.AnaBranca;
        }

        // ========== FÁBRICA DE CORPOS OTIMIZADA ==========
        public static Corpo CriarRealistaAleatorio(double canvasWidth, double canvasHeight)
        {
            var tipoCorpo = SelecionarTipoAstronomico();
            
            // ✅ LOOKUP TABLE para propriedades
            var (massa, densidade, temperatura, cor, velocidadeRotacao, prefixoNome) = tipoCorpo switch
            {
                TipoCorpo.Asteroide => (
                    _rnd.NextDouble() * 2.5 + 0.5,
                    _rnd.NextDouble() * 2.5 + 2.0,
                    _rnd.NextDouble() * 100 + 200,
                    GerarCorPorTemperaturaCache(200, 300, tipoCorpo),
                    _rnd.NextDouble() * 0.05 + 0.01,
                    "Ast"
                ),
                TipoCorpo.Cometa => (
                    _rnd.NextDouble() * 2 + 0.3,
                    _rnd.NextDouble() * 0.8 + 0.5,
                    _rnd.NextDouble() * 150 + 100,
                    "#" + _rnd.Next(200, 256).ToString("X2") + _rnd.Next(230, 256).ToString("X2") + _rnd.Next(240, 256).ToString("X2"),
                    _rnd.NextDouble() * 0.08 + 0.02,
                    "Com"
                ),
                TipoCorpo.Lua => (
                    _rnd.NextDouble() * 7 + 3,
                    _rnd.NextDouble() * 2 + 2.5,
                    _rnd.NextDouble() * 200 + 150,
                    GerarCorPorTemperaturaCache(150, 350, tipoCorpo),
                    _rnd.NextDouble() * 0.03 + 0.005,
                    "Lua"
                ),
                TipoCorpo.PlanetaRochoso => (
                    _rnd.NextDouble() * 17 + 8,
                    _rnd.NextDouble() * 3 + 3.5,
                    _rnd.NextDouble() * 300 + 250,
                    GerarCorPlanetaRochosoCache(_rnd.NextDouble() * 300 + 250),
                    _rnd.NextDouble() * 0.04 + 0.01,
                    "Terra"
                ),
                TipoCorpo.GiganteGasoso => (
                    _rnd.NextDouble() * 50 + 30,
                    _rnd.NextDouble() * 1.0 + 0.7,
                    _rnd.NextDouble() * 100 + 120,
                    GerarCorGiganteGasosoCache(),
                    _rnd.NextDouble() * 0.08 + 0.04,
                    "Júpiter"
                ),
                TipoCorpo.GiganteGelo => (
                    _rnd.NextDouble() * 35 + 35,
                    _rnd.NextDouble() * 0.8 + 1.2,
                    _rnd.NextDouble() * 80 + 50,
                    "#" + _rnd.Next(100, 161).ToString("X2") + _rnd.Next(180, 241).ToString("X2") + _rnd.Next(220, 256).ToString("X2"),
                    _rnd.NextDouble() * 0.06 + 0.03,
                    "Netuno"
                ),
                TipoCorpo.AnaBranca => (
                    _rnd.NextDouble() * 50 + 70,
                    _rnd.NextDouble() * 8 + 8,
                    _rnd.NextDouble() * 20000 + 10000,
                    _rnd.NextDouble() * 20000 + 10000 > 20000 ? "#E0F0FF" : "#FFF5E6",
                    _rnd.NextDouble() * 0.1 + 0.05,
                    "AnãBr"
                ),
                TipoCorpo.Estrela => GerarPropriedadesEstrela(),
                _ => (
                    _rnd.Next(10, 30),
                    3.0,
                    300.0,
                    "#FFFFFF",
                    0.01,
                    "Desconhecido"
                )
            };

            var (posX, posY) = GerarPosicaoDistribuidaOtimizada(canvasWidth, canvasHeight);

            var corpo = new Corpo($"{prefixoNome}{++_contador}", massa, densidade, posX, posY, cor)
            {
                VelX = 0,
                VelY = 0,
                VelocidadeRotacao = velocidadeRotacao,
                AnguloRotacao = _rnd.NextDouble() * 6.283185307, // 2 * PI
                Temperatura = temperatura,
                Tipo = tipoCorpo,
                EhLuminoso = tipoCorpo == TipoCorpo.Estrela || tipoCorpo == TipoCorpo.AnaBranca,
                Brilho = tipoCorpo == TipoCorpo.Estrela ? _rnd.NextDouble() * 0.3 + 0.7 : 1.0,
                PulsacaoBrilho = tipoCorpo == TipoCorpo.Estrela ? _rnd.NextDouble() * 0.01 : 0
            };

            return corpo;
        }

        // ========== MÉTODOS OTIMIZADOS COM CACHE ==========
        private static (double massa, double densidade, double temperatura, string cor, double velocidadeRotacao, string prefixoNome) GerarPropriedadesEstrela()
        {
            var classificacao = _rnd.Next(0, 5);
            var massa = classificacao switch
            {
                0 => _rnd.NextDouble() * 30 + 80,
                1 => _rnd.NextDouble() * 40 + 100,
                2 => _rnd.NextDouble() * 50 + 120,
                3 => _rnd.NextDouble() * 60 + 150,
                _ => _rnd.NextDouble() * 70 + 180
            };

            var temperatura = massa switch
            {
                <= 110 => _rnd.NextDouble() * 1000 + 2500,
                <= 140 => _rnd.NextDouble() * 1500 + 3500,
                <= 170 => _rnd.NextDouble() * 1500 + 5000,
                <= 210 => _rnd.NextDouble() * 2000 + 6500,
                _ => _rnd.NextDouble() * 6500 + 8500
            };

            var prefixo = temperatura switch
            {
                <= 3500 => "M-Anã",
                <= 5000 => "K-Lar",
                <= 6500 => "G-Sol",
                <= 8500 => "F-Brc",
                _ => "O-Azl"
            };

            return (massa, _rnd.NextDouble() * 2 + 1.0, temperatura, GerarCorEstrelaCache(temperatura), _rnd.NextDouble() * 0.02 + 0.01, prefixo);
        }

        private static string GerarCorPorTemperaturaCache(double minTemp, double maxTemp, TipoCorpo tipo)
        {
            var key = $"{tipo}_{minTemp}_{maxTemp}";
            if (!_cacheCores.ContainsKey(key))
            {
                var temp = _rnd.NextDouble() * (maxTemp - minTemp) + minTemp;
                var cor = tipo switch
                {
                    TipoCorpo.Asteroide => $"#{_rnd.Next(120, 181):X2}{_rnd.Next(115, 166):X2}{_rnd.Next(110, 161):X2}",
                    TipoCorpo.Lua => $"#{_rnd.Next(160, 211):X2}{_rnd.Next(155, 206):X2}{_rnd.Next(145, 196):X2}",
                    _ => $"#{_rnd.Next(140, 201):X2}{_rnd.Next(130, 191):X2}{_rnd.Next(120, 181):X2}"
                };
                _cacheCores[key] = new int[] { _rnd.Next(140, 201), _rnd.Next(130, 191), _rnd.Next(120, 181) };
            }
            var rgb = _cacheCores[key];
            return $"#{rgb[0]:X2}{rgb[1]:X2}{rgb[2]:X2}";
        }

        private static string GerarCorPlanetaRochosoCache(double temperatura)
        {
            if (temperatura > 400)
                return $"#{_rnd.Next(180, 231):X2}{_rnd.Next(80, 131):X2}{_rnd.Next(50, 101):X2}";
            else if (temperatura > 270 && temperatura < 320)
                return $"#{_rnd.Next(80, 141):X2}{_rnd.Next(120, 181):X2}{_rnd.Next(200, 256):X2}";
            else
                return $"#{_rnd.Next(150, 201):X2}{_rnd.Next(140, 191):X2}{_rnd.Next(120, 171):X2}";
        }

        private static string GerarCorGiganteGasosoCache()
        {
            var tipo = _rnd.Next(0, 3);
            return tipo switch
            {
                0 => $"#{_rnd.Next(200, 241):X2}{_rnd.Next(140, 181):X2}{_rnd.Next(80, 121):X2}",
                1 => $"#{_rnd.Next(180, 221):X2}{_rnd.Next(150, 191):X2}{_rnd.Next(120, 161):X2}",
                _ => $"#{_rnd.Next(220, 256):X2}{_rnd.Next(180, 221):X2}{_rnd.Next(100, 141):X2}"
            };
        }

        private static string GerarCorEstrelaCache(double temperatura)
        {
            return temperatura switch
            {
                <= 3500 => $"#{_rnd.Next(200, 256):X2}{_rnd.Next(100, 151):X2}{_rnd.Next(80, 121):X2}",
                <= 5000 => $"#{_rnd.Next(220, 256):X2}{_rnd.Next(160, 201):X2}{_rnd.Next(100, 141):X2}",
                <= 6500 => $"#{_rnd.Next(240, 256):X2}{_rnd.Next(230, 256):X2}{_rnd.Next(180, 221):X2}",
                <= 8500 => $"#{_rnd.Next(240, 256):X2}{_rnd.Next(245, 256):X2}{_rnd.Next(230, 256):X2}",
                _ => $"#{_rnd.Next(200, 241):X2}{_rnd.Next(220, 256):X2}{_rnd.Next(240, 256):X2}"
            };
        }

        // ========== DISTRIBUIÇÃO PROBABILÍSTICA OTIMIZADA ==========
        private static TipoCorpo SelecionarTipoAstronomico()
        {
            var roll = _rnd.NextDouble();
            return roll switch
            {
                < 0.30 => TipoCorpo.Asteroide,
                < 0.45 => TipoCorpo.Cometa,
                < 0.55 => TipoCorpo.Lua,
                < 0.70 => TipoCorpo.PlanetaRochoso,
                < 0.80 => TipoCorpo.GiganteGasoso,
                < 0.88 => TipoCorpo.GiganteGelo,
                < 0.93 => TipoCorpo.AnaBranca,
                _ => TipoCorpo.Estrela
            };
        }

        // ========== POSIÇÃO DISTRIBUÍDA OTIMIZADA ==========
        private static (double x, double y) GerarPosicaoDistribuidaOtimizada(double width, double height)
        {
            // Distribuição mais simples e rápida
            double x = (_rnd.NextDouble() * 0.8 + 0.1) * width;
            double y = (_rnd.NextDouble() * 0.8 + 0.1) * height;
            return (x, y);
        }

        // ========== FÍSICA GRAVITACIONAL ULTRA-OTIMIZADA ==========
        public void AplicarGravidade(Corpo outro, double fatorSimulacao, double deltaTime)
        {
            double dx = outro.PosX - PosX;
            double dy = outro.PosY - PosY;
            double dist2 = dx * dx + dy * dy;

            // ✅ VERIFICAÇÃO DE COLISÃO OTIMIZADA
            double raioSoma = Raio + outro.Raio;
            if (dist2 < raioSoma * raioSoma) return;

            // ✅ CÁLCULO DE FORÇA OTIMIZADO
            double forca = G_REAL * (Massa * outro.Massa) / dist2 * fatorSimulacao;
            double dist = Math.Sqrt(dist2);
            double forcaDist = forca / dist;

            // ✅ ACELERAÇÃO OTIMIZADA (evita divisões repetidas)
            double ax = forcaDist * dx * _massaInversa;
            double ay = forcaDist * dy * _massaInversa;
            double bx = forcaDist * -dx * outro._massaInversa;
            double by = forcaDist * -dy * outro._massaInversa;

            // ✅ INTEGRAÇÃO DE VELOCIDADE OTIMIZADA
            VelX += ax * deltaTime;
            VelY += ay * deltaTime;
            outro.VelX += bx * deltaTime;
            outro.VelY += by * deltaTime;
        }

        // ========== ATUALIZAÇÃO DE POSIÇÃO ULTRA-OTIMIZADA ==========
        public void AtualizarPosicao(double canvasWidth, double canvasHeight)
        {
            // ✅ ATUALIZAÇÃO DE POSIÇÃO
            PosX += VelX;
            PosY += VelY;

            // ✅ ROTAÇÃO OTIMIZADA
            AnguloRotacao += VelocidadeRotacao;
            if (AnguloRotacao > 6.283185307) // 2 * PI
                AnguloRotacao -= 6.283185307;

            // ✅ PULSAÇÃO DE BRILHO OTIMIZADA
            if (EhLuminoso && PulsacaoBrilho > 0)
            {
                Brilho = 0.85 + 0.15 * Math.Sin(AnguloRotacao * 5 + PulsacaoBrilho * 100);
            }

            // ✅ COLISÃO COM BORDAS OTIMIZADA
            double metadeRaio = Raio * 0.5;
            
            if (PosX < metadeRaio)
            {
                PosX = metadeRaio;
                VelX = -VelX * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao += VelX * 0.01;
            }
            else if (PosX > canvasWidth - metadeRaio)
            {
                PosX = canvasWidth - metadeRaio;
                VelX = -VelX * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao -= VelX * 0.01;
            }

            if (PosY < metadeRaio)
            {
                PosY = metadeRaio;
                VelY = -VelY * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao += VelY * 0.01;
            }
            else if (PosY > canvasHeight - metadeRaio)
            {
                PosY = canvasHeight - metadeRaio;
                VelY = -VelY * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao -= VelY * 0.01;
            }

            // ✅ LIMITAÇÃO DE ROTAÇÃO OTIMIZADA
            if (Math.Abs(VelocidadeRotacao) > LIMITE_ROTACAO)
                VelocidadeRotacao *= FATOR_AMORTECIMENTO;
        }

        // ========== FUSÃO OTIMIZADA ==========
        public static Corpo Fundir(Corpo a, Corpo b)
        {
            double massaTotal = a.Massa + b.Massa;
            double densidadeMedia = (a.Densidade * a.Massa + b.Densidade * b.Massa) / massaTotal;
            
            double x = (a.PosX * a.Massa + b.PosX * b.Massa) / massaTotal;
            double y = (a.PosY * a.Massa + b.PosY * b.Massa) / massaTotal;
            
            double velX = (a.Massa * a.VelX + b.Massa * b.VelX) / massaTotal;
            double velY = (a.Massa * a.VelY + b.Massa * b.VelY) / massaTotal;

            // ✅ CÁLCULO DE MOMENTO ANGULAR OTIMIZADO
            double momAngularTotal = a.Massa * a._raioQuadrado * a.VelocidadeRotacao + 
                                   b.Massa * b._raioQuadrado * b.VelocidadeRotacao;

            string cor = MixColorsOtimizado(a.Cor, b.Cor, a.Massa, b.Massa);

            var novoCorpo = new Corpo($"Fus{++_contador}", massaTotal, densidadeMedia, x, y, cor)
            {
                VelX = velX,
                VelY = velY,
                VelocidadeRotacao = 0,
                AnguloRotacao = (a.AnguloRotacao + b.AnguloRotacao) * 0.5,
                Brilho = 1.0,
                Tipo = massaTotal switch
                {
                    < 4 => TipoCorpo.Asteroide,
                    < 12 => TipoCorpo.Lua,
                    < 30 => TipoCorpo.PlanetaRochoso,
                    < 85 => TipoCorpo.GiganteGasoso,
                    _ => TipoCorpo.Estrela
                }
            };

            // ✅ CÁLCULO DE ROTAÇÃO APÓS FUSÃO
            double novoRaioQuadrado = novoCorpo._raioQuadrado;
            if (novoRaioQuadrado > 0)
                novoCorpo.VelocidadeRotacao = momAngularTotal / (massaTotal * novoRaioQuadrado);

            novoCorpo.EhLuminoso = novoCorpo.Tipo == TipoCorpo.Estrela || novoCorpo.Tipo == TipoCorpo.AnaBranca;

            return novoCorpo;
        }

        // ========== MISTURA DE CORES OTIMIZADA ==========
        private static string MixColorsOtimizado(string corA, string corB, double massaA, double massaB)
        {
            try
            {
                if (corA.StartsWith("#") && corB.StartsWith("#") && corA.Length == 7 && corB.Length == 7)
                {
                    int r1 = Convert.ToInt32(corA.Substring(1, 2), 16);
                    int g1 = Convert.ToInt32(corA.Substring(3, 2), 16);
                    int b1 = Convert.ToInt32(corA.Substring(5, 2), 16);
                    
                    int r2 = Convert.ToInt32(corB.Substring(1, 2), 16);
                    int g2 = Convert.ToInt32(corB.Substring(3, 2), 16);
                    int b2 = Convert.ToInt32(corB.Substring(5, 2), 16);

                    double total = massaA + massaB;
                    int r = (int)((r1 * massaA + r2 * massaB) / total);
                    int g = (int)((g1 * massaA + g2 * massaB) / total);
                    int b = (int)((b1 * massaA + b2 * massaB) / total);

                    return $"#{Math.Min(255, Math.Max(0, r)):X2}{Math.Min(255, Math.Max(0, g)):X2}{Math.Min(255, Math.Max(0, b)):X2}";
                }
            }
            catch { }

            return massaA >= massaB ? corA : corB;
        }
    }

    public enum TipoCorpo
    {
        Asteroide, Cometa, Lua, PlanetaRochoso, GiganteGasoso, GiganteGelo, AnaBranca, Estrela
    }
}