using System;
using System.Collections.Generic;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Classe que representa um corpo celeste com física realista e otimizações de desempenho
    /// Implementa cálculos gravitacionais, colisões e propriedades astronômicas realistas
    /// </summary>
    public class Corpo
    {
        // Contador estático para gerar identificadores únicos para cada corpo criado
        private static int _contador = 0;

        // Gerador de números aleatórios compartilhado para toda a classe
        private static readonly Random _rnd = new();

        // Cache para cores geradas, evitando recálculos desnecessários
        private static readonly Dictionary<string, int[]> _cacheCores = new();

        // ========== PROPRIEDADES FÍSICAS BÁSICAS ==========

        /// <summary>
        /// Nome identificador do corpo celeste
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Tipo astronômico do corpo (asteroide, planeta, estrela, etc.)
        /// </summary>
        public TipoCorpo Tipo { get; set; }

        /// <summary>
        /// Massa do corpo em unidades arbitrárias (normalmente kg * escala)
        /// </summary>
        public double Massa { get; set; }

        /// <summary>
        /// Densidade do corpo em g/cm³ (valores realistas para corpos astronômicos)
        /// </summary>
        public double Densidade { get; set; }

        /// <summary>
        /// Raio visual do corpo em pixels/unidades de simulação
        /// </summary>
        public double Raio { get; set; }

        /// <summary>
        /// Cor hexadecimal do corpo para renderização
        /// </summary>
        public string Cor { get; set; }

        // ========== PROPRIEDADES CINEMÁTICAS ==========

        /// <summary>
        /// Posição horizontal do corpo no espaço simulado
        /// </summary>
        public double PosX { get; set; }

        /// <summary>
        /// Posição vertical do corpo no espaço simulado
        /// </summary>
        public double PosY { get; set; }

        /// <summary>
        /// Componente horizontal da velocidade
        /// </summary>
        public double VelX { get; set; }

        /// <summary>
        /// Componente vertical da velocidade
        /// </summary>
        public double VelY { get; set; }

        // ========== PROPRIEDADES DE ROTAÇÃO E ASPECTO VISUAL ==========

        /// <summary>
        /// Velocidade de rotação do corpo em radianos por frame
        /// </summary>
        public double VelocidadeRotacao { get; set; }

        /// <summary>
        /// Ângulo atual de rotação do corpo em radianos
        /// </summary>
        public double AnguloRotacao { get; set; }

        /// <summary>
        /// Intensidade do brilho do corpo (0.0 a 1.0)
        /// </summary>
        public double Brilho { get; set; }

        /// <summary>
        /// Fator de pulsação do brilho para estrelas variáveis
        /// </summary>
        public double PulsacaoBrilho { get; set; }

        // ========== PROPRIEDADES TÉRMICAS E LUMINOSAS ==========

        /// <summary>
        /// Temperatura superficial do corpo em Kelvin
        /// </summary>
        public double Temperatura { get; set; }

        /// <summary>
        /// Indica se o corpo emite sua própria luz (estrelas, anãs brancas)
        /// </summary>
        public bool EhLuminoso { get; set; }

        // ========== CACHE PARA CÁLCULOS OTIMIZADOS ==========

        /// <summary>
        /// Cache do quadrado do raio para cálculos de colisão otimizados
        /// </summary>
        private double _raioQuadrado;

        /// <summary>
        /// Cache do inverso da massa para cálculos gravitacionais otimizados
        /// </summary>
        private double _massaInversa;

        // ========== CONSTANTES FÍSICAS OTIMIZADAS ==========

        /// <summary>
        /// Constante gravitacional real (6.674 × 10^-11 N·m²/kg²)
        /// </summary>
        private const double G_REAL = 6.674e-11;

        /// <summary>
        /// Coeficiente de restituição para colisões com bordas (0.0 a 1.0)
        /// </summary>
        private const double COEFICIENTE_RESTITUICAO = 0.75;

        /// <summary>
        /// Fator de amortecimento para limitar rotações extremas
        /// </summary>
        private const double FATOR_AMORTECIMENTO = 0.95;

        /// <summary>
        /// Limite máximo para velocidade de rotação
        /// </summary>
        private const double LIMITE_ROTACAO = 0.2;

        // ========== CONSTRUTOR PRINCIPAL ==========

        /// <summary>
        /// Construtor principal para criar um corpo celeste
        /// </summary>
        /// <param name="nome">Nome identificador do corpo</param>
        /// <param name="massa">Massa do corpo</param>
        /// <param name="densidade">Densidade do corpo</param>
        /// <param name="posX">Posição horizontal inicial</param>
        /// <param name="posY">Posição vertical inicial</param>
        /// <param name="cor">Cor hexadecimal do corpo</param>
        public Corpo(string nome, double massa, double densidade, double posX, double posY, string cor)
        {
            Nome = nome;
            Massa = massa;
            Densidade = densidade;
            PosX = posX;
            PosY = posY;
            Cor = cor;

            // Calcula propriedades derivadas como raio, tipo, temperatura
            CalcularPropriedadesDerivadas();
        }

        // ========== MÉTODO PARA CÁLCULO DE PROPRIEDADES DERIVADAS ==========

        /// <summary>
        /// Calcula propriedades derivadas baseadas nas propriedades básicas
        /// Este método é chamado no construtor e após fusões de corpos
        /// </summary>
        private void CalcularPropriedadesDerivadas()
        {
            // Cálculo otimizado do raio a partir da massa e densidade
            // Fórmula física: Volume = Massa / Densidade
            // Fórmula astronômica: Raio = (3 * Volume / (4 * π))^(1/3)
            // Otimização: Pré-calcula (3/(4π))^(1/3) ≈ 0.238732414 para evitar operações repetidas
            // Aplicação: Usado para determinar tamanho visual e cálculos de colisão
            double volume = Massa / Densidade;
            Raio = Math.Pow(0.238732414 * volume, 0.333333333);

            // Cache de valores frequentemente usados em cálculos físicos
            _raioQuadrado = Raio * Raio;
            _massaInversa = 1.0 / Massa;

            // Classificação astronômica baseada em massa (unidades relativas):
            // - Asteroides: < 4 (corpos menores, restos de formação planetária)
            // - Luas: 4-12 (satélites naturais de planetas)  
            // - Planetas Rochosos: 12-30 (Terra ≈ 15, Marte ≈ 12)
            // - Gigantes Gasosos: 30-85 (Júpiter ≈ 70, Saturno ≈ 50)
            // - Estrelas: > 85 (estrelas de sequência principal)
            // Baseado na escala de massas do Sistema Solar
            Tipo = Massa switch
            {
                < 4 => TipoCorpo.Asteroide,
                < 12 => TipoCorpo.Lua,
                < 30 => TipoCorpo.PlanetaRochoso,
                < 85 => TipoCorpo.GiganteGasoso,
                _ => TipoCorpo.Estrela
            };

            // Temperaturas baseadas em observações astronômicas reais (Kelvin):
            // - Asteroides: ~250K (-23°C) - temperatura média no cinturão
            // - Cometas: ~150K (-123°C) - extremamente frios no espaço profundo
            // - Luas: ~250K - varia com distância da estrela
            // - Planetas Rochosos: ~350K (77°C) - considerando efeito estufa
            // - Gigantes Gasosos: ~150K - frios devido à distância
            // - Anãs Brancas: ~15,000K - remanescentes estelares quentes
            // - Estrelas: ~5,778K - temperatura do Sol (classe G)
            Temperatura = Tipo switch
            {
                TipoCorpo.Asteroide => 250,        // Asteroides: ~250K (-23°C)
                TipoCorpo.Cometa => 150,           // Cometas: ~150K (-123°C)
                TipoCorpo.Lua => 250,              // Luas: ~250K
                TipoCorpo.PlanetaRochoso => 350,   // Planetas rochosos: ~350K
                TipoCorpo.GiganteGasoso => 150,    // Gigantes gasosos: ~150K
                TipoCorpo.GiganteGelo => 80,       // Gigantes de gelo: ~80K
                TipoCorpo.AnaBranca => 15000,      // Anãs brancas: ~15,000K
                TipoCorpo.Estrela => 5778,         // Estrelas como o Sol: ~5,778K
                _ => 300                           // Valor padrão
            };

            // Corpos luminosos são aqueles que emitem sua própria luz
            EhLuminoso = Tipo == TipoCorpo.Estrela || Tipo == TipoCorpo.AnaBranca;
        }

        // ========== FÁBRICA DE CORPOS ALEATÓRIOS ==========

        /// <summary>
        /// Cria um corpo celeste realista com propriedades aleatórias baseadas em distribuições astronômicas
        /// </summary>
        /// <param name="canvasWidth">Largura do espaço simulado</param>
        /// <param name="canvasHeight">Altura do espaço simulado</param>
        /// <returns>Novo corpo celeste com propriedades realistas</returns>
        public static Corpo CriarRealistaAleatorio(double canvasWidth, double canvasHeight)
        {
            // Seleciona tipo de corpo baseado em distribuição probabilística
            var tipoCorpo = SelecionarTipoAstronomico();

            // Lookup table para propriedades baseadas no tipo
            // Retorna tupla com: massa, densidade, temperatura, cor, velocidadeRotacao, prefixoNome
            var (massa, densidade, temperatura, cor, velocidadeRotacao, prefixoNome) = tipoCorpo switch
            {
                TipoCorpo.Asteroide => (
                    // Asteroides: massa 0.5-3.0, densidade 2.0-4.5 g/cm³
                    _rnd.NextDouble() * 2.5 + 0.5,
                    _rnd.NextDouble() * 2.5 + 2.0,
                    _rnd.NextDouble() * 100 + 200,  // 200-300K
                    GerarCorPorTemperaturaCache(200, 300, tipoCorpo),
                    _rnd.NextDouble() * 0.05 + 0.01,  // Rotação lenta
                    "Ast"  // Prefixo para asteroides
                ),
                TipoCorpo.Cometa => (
                    // Cometas: massa 0.3-2.3, baixa densidade 0.5-1.3 g/cm³
                    _rnd.NextDouble() * 2 + 0.3,
                    _rnd.NextDouble() * 0.8 + 0.5,
                    _rnd.NextDouble() * 150 + 100,  // 100-250K (cometas são frios)
                    "#" + _rnd.Next(200, 256).ToString("X2") + _rnd.Next(230, 256).ToString("X2") + _rnd.Next(240, 256).ToString("X2"),  // Cores claras/azuladas
                    _rnd.NextDouble() * 0.08 + 0.02,
                    "Com"  // Prefixo para cometas
                ),
                TipoCorpo.Lua => (
                    // Luas: massa 3-10, densidade 2.5-4.5 g/cm³
                    _rnd.NextDouble() * 7 + 3,
                    _rnd.NextDouble() * 2 + 2.5,
                    _rnd.NextDouble() * 200 + 150,  // 150-350K
                    GerarCorPorTemperaturaCache(150, 350, tipoCorpo),
                    _rnd.NextDouble() * 0.03 + 0.005,  // Rotação muito lenta
                    "Lua"  // Prefixo para luas
                ),
                TipoCorpo.PlanetaRochoso => (
                    // Planetas rochosos: massa 8-25, densidade 3.5-6.5 g/cm³
                    _rnd.NextDouble() * 17 + 8,
                    _rnd.NextDouble() * 3 + 3.5,
                    _rnd.NextDouble() * 300 + 250,  // 250-550K
                    GerarCorPlanetaRochosoCache(_rnd.NextDouble() * 300 + 250),  // Cor baseada na temperatura
                    _rnd.NextDouble() * 0.04 + 0.01,
                    "Terra"  // Prefixo para planetas rochosos
                ),
                TipoCorpo.GiganteGasoso => (
                    // Gigantes gasosos: massa 30-80, baixa densidade 0.7-1.7 g/cm³
                    _rnd.NextDouble() * 50 + 30,
                    _rnd.NextDouble() * 1.0 + 0.7,
                    _rnd.NextDouble() * 100 + 120,  // 120-220K
                    GerarCorGiganteGasosoCache(),  // Cores características de Júpiter/Saturno
                    _rnd.NextDouble() * 0.08 + 0.04,  // Rotação mais rápida
                    "Júpiter"  // Prefixo para gigantes gasosos
                ),
                TipoCorpo.GiganteGelo => (
                    // Gigantes de gelo: massa 35-70, densidade 1.2-2.0 g/cm³
                    _rnd.NextDouble() * 35 + 35,
                    _rnd.NextDouble() * 0.8 + 1.2,
                    _rnd.NextDouble() * 80 + 50,  // 50-130K (muito frios)
                    "#" + _rnd.Next(100, 161).ToString("X2") + _rnd.Next(180, 241).ToString("X2") + _rnd.Next(220, 256).ToString("X2"),  // Azuis
                    _rnd.NextDouble() * 0.06 + 0.03,
                    "Netuno"  // Prefixo para gigantes de gelo
                ),
                TipoCorpo.AnaBranca => (
                    // Anãs brancas: massa 70-120, alta densidade 8-16 g/cm³
                    _rnd.NextDouble() * 50 + 70,
                    _rnd.NextDouble() * 8 + 8,
                    _rnd.NextDouble() * 20000 + 10000,  // 10,000-30,000K (muito quentes)
                    _rnd.NextDouble() * 20000 + 10000 > 20000 ? "#E0F0FF" : "#FFF5E6",  // Branco azulado ou branco amarelado
                    _rnd.NextDouble() * 0.1 + 0.05,
                    "AnãBr"  // Prefixo para anãs brancas
                ),
                TipoCorpo.Estrela => GerarPropriedadesEstrela(),  // Propriedades complexas para estrelas
                _ => (  // Caso padrão para tipos não especificados
                    _rnd.Next(10, 30),
                    3.0,
                    300.0,
                    "#FFFFFF",
                    0.01,
                    "Desconhecido"
                )
            };

            // Gera posição distribuída no canvas
            var (posX, posY) = GerarPosicaoDistribuidaOtimizada(canvasWidth, canvasHeight);

            // Cria o corpo com as propriedades calculadas
            var corpo = new Corpo($"{prefixoNome}{++_contador}", massa, densidade, posX, posY, cor)
            {
                VelX = 0,  // Velocidade inicial zero
                VelY = 0,
                VelocidadeRotacao = velocidadeRotacao,
                AnguloRotacao = _rnd.NextDouble() * 6.283185307,  // 2 * PI - ângulo aleatório
                Temperatura = temperatura,
                Tipo = tipoCorpo,
                EhLuminoso = tipoCorpo == TipoCorpo.Estrela || tipoCorpo == TipoCorpo.AnaBranca,
                Brilho = tipoCorpo == TipoCorpo.Estrela ? _rnd.NextDouble() * 0.3 + 0.7 : 1.0,  // Estrelas têm brilho variável
                PulsacaoBrilho = tipoCorpo == TipoCorpo.Estrela ? _rnd.NextDouble() * 0.01 : 0  // Pulsação apenas para estrelas
            };

            return corpo;
        }

        // ========== MÉTODOS DE GERAÇÃO DE PROPRIEDADES COM CACHE ==========

        /// <summary>
        /// Gera propriedades para estrelas baseadas em classificações estelares reais
        /// </summary>
        /// <returns>Tupla com propriedades da estrela</returns>
        private static (double massa, double densidade, double temperatura, string cor, double velocidadeRotacao, string prefixoNome) GerarPropriedadesEstrela()
        {
            // CLASSIFICAÇÃO ESTELAR POR MASSA (sequência principal):
            // - M (≤ 3,500K): Vermelhas, anãs, 70% das estrelas
            // - K (3,500-5,000K): Laranjas, estáveis como Alpha Centauri B  
            // - G (5,000-6,500K): Amarelas, como o Sol (5,778K)
            // - F (6,500-8,500K): Branco-amareladas, mais massivas
            // - O/B (>8,500K): Azuis, muito quentes e raras

            // CORRELÇÃO MASSA-TEMPERATURA: estrelas mais massivas = mais quentes
            var classificacao = _rnd.Next(0, 5);
            var massa = classificacao switch
            {
                0 => _rnd.NextDouble() * 30 + 80,   // Estrelas de baixa massa (M)
                1 => _rnd.NextDouble() * 40 + 100,  // Estrelas de massa média-baixa (K)
                2 => _rnd.NextDouble() * 50 + 120,  // Estrelas de massa média (G)
                3 => _rnd.NextDouble() * 60 + 150,  // Estrelas de massa média-alta (F)
                _ => _rnd.NextDouble() * 70 + 180   // Estrelas de alta massa (O/B)
            };

            // Temperatura correlacionada com massa (mais massa = mais quente)
            var temperatura = massa switch
            {
                <= 110 => _rnd.NextDouble() * 1000 + 2500,  // Estrelas M: 2,500-3,500K
                <= 140 => _rnd.NextDouble() * 1500 + 3500,  // Estrelas K: 3,500-5,000K
                <= 170 => _rnd.NextDouble() * 1500 + 5000,  // Estrelas G: 5,000-6,500K
                <= 210 => _rnd.NextDouble() * 2000 + 6500,  // Estrelas F: 6,500-8,500K
                _ => _rnd.NextDouble() * 6500 + 8500        // Estrelas O/B: 8,500-15,000K
            };

            // Prefixo baseado na temperatura (classificação espectral)
            var prefixo = temperatura switch
            {
                <= 3500 => "M-Anã",  // Classe M
                <= 5000 => "K-Lar",  // Classe K  
                <= 6500 => "G-Sol",  // Classe G (como o Sol)
                <= 8500 => "F-Brc",  // Classe F
                _ => "O-Azl"         // Classes O/B
            };

            return (massa, _rnd.NextDouble() * 2 + 1.0, temperatura, GerarCorEstrelaCache(temperatura), _rnd.NextDouble() * 0.02 + 0.01, prefixo);
        }

        /// <summary>
        /// Gera cor baseada em temperatura usando cache para otimização
        /// </summary>
        private static string GerarCorPorTemperaturaCache(double minTemp, double maxTemp, TipoCorpo tipo)
        {
            var key = $"{tipo}_{minTemp}_{maxTemp}";
            if (!_cacheCores.ContainsKey(key))
            {
                // Gera cor baseada no tipo de corpo
                var cor = tipo switch
                {
                    TipoCorpo.Asteroide => $"#{_rnd.Next(120, 181):X2}{_rnd.Next(115, 166):X2}{_rnd.Next(110, 161):X2}",  // Tons de cinza/marrom
                    TipoCorpo.Lua => $"#{_rnd.Next(160, 211):X2}{_rnd.Next(155, 206):X2}{_rnd.Next(145, 196):X2}",        // Tons claros
                    _ => $"#{_rnd.Next(140, 201):X2}{_rnd.Next(130, 191):X2}{_rnd.Next(120, 181):X2}"                     // Tons neutros
                };
                _cacheCores[key] = new int[] { _rnd.Next(140, 201), _rnd.Next(130, 191), _rnd.Next(120, 181) };
            }
            var rgb = _cacheCores[key];
            return $"#{rgb[0]:X2}{rgb[1]:X2}{rgb[2]:X2}";
        }

        /// <summary>
        /// Gera cor para planetas rochosos baseada na temperatura
        /// </summary>
        private static string GerarCorPlanetaRochosoCache(double temperatura)
        {
            // Cores baseadas na temperatura: quente = vermelho/alaranjado, temperado = azul/verde, frio = cinza
            if (temperatura > 400)
                return $"#{_rnd.Next(180, 231):X2}{_rnd.Next(80, 131):X2}{_rnd.Next(50, 101):X2}";  // Vermelho/alaranjado
            else if (temperatura > 270 && temperatura < 320)
                return $"#{_rnd.Next(80, 141):X2}{_rnd.Next(120, 181):X2}{_rnd.Next(200, 256):X2}";  // Azul/verde (como Terra)
            else
                return $"#{_rnd.Next(150, 201):X2}{_rnd.Next(140, 191):X2}{_rnd.Next(120, 171):X2}";  // Tons neutros
        }

        /// <summary>
        /// Gera cores características para gigantes gasosos
        /// </summary>
        private static string GerarCorGiganteGasosoCache()
        {
            var tipo = _rnd.Next(0, 3);
            return tipo switch
            {
                0 => $"#{_rnd.Next(200, 241):X2}{_rnd.Next(140, 181):X2}{_rnd.Next(80, 121):X2}",  // Como Júpiter (listras marrons)
                1 => $"#{_rnd.Next(180, 221):X2}{_rnd.Next(150, 191):X2}{_rnd.Next(120, 161):X2}",  // Como Saturno (dourado)
                _ => $"#{_rnd.Next(220, 256):X2}{_rnd.Next(180, 221):X2}{_rnd.Next(100, 141):X2}"   // Tons mais claros
            };
        }

        /// <summary>
        /// Gera cor para estrelas baseada na temperatura (lei de Wien)
        /// Estrelas mais frias = vermelho, mais quentes = azul
        /// </summary>
        private static string GerarCorEstrelaCache(double temperatura)
        {
            // LEI DE WIEN APROXIMADA: cor ≈ função da temperatura
            // Estrelas mais frias → vermelho (pico no infravermelho)
            // Estrelas mais quentes → azul (pico no ultravioleta)

            // Escala de cores por tipo espectral:
            return temperatura switch
            {
                <= 3500 => $"#{_rnd.Next(200, 256):X2}{_rnd.Next(100, 151):X2}{_rnd.Next(80, 121):X2}",  // M: Vermelho
                <= 5000 => $"#{_rnd.Next(220, 256):X2}{_rnd.Next(160, 201):X2}{_rnd.Next(100, 141):X2}", // K: Laranja  
                <= 6500 => $"#{_rnd.Next(240, 256):X2}{_rnd.Next(230, 256):X2}{_rnd.Next(180, 221):X2}", // G: Amarelo (Sol)
                <= 8500 => $"#{_rnd.Next(240, 256):X2}{_rnd.Next(245, 256):X2}{_rnd.Next(230, 256):X2}", // F: Branco
                _ => $"#{_rnd.Next(200, 241):X2}{_rnd.Next(220, 256):X2}{_rnd.Next(240, 256):X2}"        // O/B: Azul
            };
        }

        // ========== DISTRIBUIÇÃO PROBABILÍSTICA DE TIPOS ASTRONÔMICOS ==========

        /// <summary>
        /// Seleciona tipo de corpo baseado em distribuição probabilística realista
        /// Asteroides são mais comuns, estrelas são mais raras
        /// </summary>
        private static TipoCorpo SelecionarTipoAstronomico()
        {
            // DISTRIBUIÇÃO ASTRONÔMICA REALISTA (aproximada):
            // Baseada na função de massa inicial observada no universo
            // - Asteroides: 30% - mais comuns (cinturões)
            // - Cometas: 15% - núvem de Oort/Kuiper  
            // - Luas: 10% - sistemas planetários
            // - Planetas Rochosos: 15% - sistemas internos
            // - Gigantes Gasosos: 10% - sistemas externos
            // - Gigantes Gelo: 8% - limites do sistema
            // - Anãs Brancas: 5% - estágio evolutivo
            // - Estrelas: 7% - relativamente raras

            var roll = _rnd.NextDouble();
            return roll switch
            {
                < 0.30 => TipoCorpo.Asteroide,     // 30%
                < 0.45 => TipoCorpo.Cometa,        // 15% 
                < 0.55 => TipoCorpo.Lua,           // 10%
                < 0.70 => TipoCorpo.PlanetaRochoso, // 15%
                < 0.80 => TipoCorpo.GiganteGasoso, // 10%
                < 0.88 => TipoCorpo.GiganteGelo,   // 8%
                < 0.93 => TipoCorpo.AnaBranca,     // 5%
                _ => TipoCorpo.Estrela             // 7%
            };
        }

        // ========== DISTRIBUIÇÃO ESPACIAL OTIMIZADA ==========

        /// <summary>
        /// Gera posição distribuída no canvas, evitando bordas
        /// </summary>
        private static (double x, double y) GerarPosicaoDistribuidaOtimizada(double width, double height)
        {
            // Distribuição simples: 80% do espaço central, 10% de margem em cada lado
            double x = (_rnd.NextDouble() * 0.8 + 0.1) * width;
            double y = (_rnd.NextDouble() * 0.8 + 0.1) * height;
            return (x, y);
        }

        // ========== FÍSICA GRAVITACIONAL OTIMIZADA ==========

        /// <summary>
        /// Aplica força gravitacional entre este corpo e outro corpo
        /// Usa a Lei da Gravitação Universal de Newton com otimizações
        /// </summary>
        /// <param name="outro">Outro corpo para calcular interação gravitacional</param>
        /// <param name="fatorSimulacao">Fator de escala para forças gravitacionais</param>
        /// <param name="deltaTime">Tempo decorrido desde o último cálculo</param>
        public void AplicarGravidade(Corpo outro, double fatorSimulacao, double deltaTime)
        {
            // LEI DA GRAVITAÇÃO UNIVERSAL (Newton): F = G * (m1 * m2) / r²
            // Onde:
            // - G = 6.674 × 10^-11 N·m²/kg² (constante gravitacional)
            // - m1, m2 = massas dos corpos
            // - r = distância entre centros de massa
            // - fatorSimulacao = escala para tornar forças visíveis

            // OTIMIZAÇÕES IMPLEMENTADAS:
            // 1. Evita sqrt até necessário: usa dist² para verificações
            // 2. Cache de 1/massa: elimina divisões repetidas  
            // 3. Cálculo vetorial otimizado: componentes X/Y separados
            // 4. Verificação de colisão: ignora gravidade se corpos colidindo

            // Calcula vetor distância entre os corpos
            double dx = outro.PosX - PosX;
            double dy = outro.PosY - PosY;
            double dist2 = dx * dx + dy * dy;  // Distância ao quadrado (performance)

            // Verificação de colisão: se (raio1 + raio2)² > dist² → colidindo
            double raioSoma = Raio + outro.Raio;
            if (dist2 < raioSoma * raioSoma) return;

            // Força gravitacional completa
            double forca = G_REAL * (Massa * outro.Massa) / dist2 * fatorSimulacao;
            double dist = Math.Sqrt(dist2);  // Única raiz quadrada necessária
            double forcaDist = forca / dist;

            // Acelerações (F = m·a → a = F/m)
            double ax = forcaDist * dx * _massaInversa;  // Usa cache de 1/massa
            double ay = forcaDist * dy * _massaInversa;
            double bx = forcaDist * -dx * outro._massaInversa;
            double by = forcaDist * -dy * outro._massaInversa;

            // Integração de velocidade (Euler simples)
            VelX += ax * deltaTime;
            VelY += ay * deltaTime;
            outro.VelX += bx * deltaTime;
            outro.VelY += by * deltaTime;
        }

        // ========== ATUALIZAÇÃO DE POSIÇÃO E ESTADO ==========

        /// <summary>
        /// Atualiza a posição e estado do corpo baseado na velocidade atual
        /// Também trata colisões com bordas e atualiza rotação
        /// </summary>
        /// <param name="canvasWidth">Largura do espaço simulado</param>
        /// <param name="canvasHeight">Altura do espaço simulado</param>
        public void AtualizarPosicao(double canvasWidth, double canvasHeight)
        {
            // Atualização de posição: x = x0 + v * Δt (implícito)
            PosX += VelX;
            PosY += VelY;

            // Atualização de rotação
            AnguloRotacao += VelocidadeRotacao;
            // Mantém ângulo entre 0 e 2π
            if (AnguloRotacao > 6.283185307)  // 2 * PI
                AnguloRotacao -= 6.283185307;

            // Pulsação de brilho para corpos luminosos
            if (EhLuminoso && PulsacaoBrilho > 0)
            {
                // Efeito de pulsação suave usando seno
                Brilho = 0.85 + 0.15 * Math.Sin(AnguloRotacao * 5 + PulsacaoBrilho * 100);
            }

            // Colisão com bordas - tratamento otimizado
            double metadeRaio = Raio * 0.5;

            // FÍSICA DE COLISÃO COM BORDAS:
            // - Coeficiente de restituição: 0.75 (25% de energia perdida)
            // - Transferência momento→rotação: v_rot += v_lin * 0.01
            // - Amortecimento angular: limita rotações extremas

            // Colisão com borda esquerda
            if (PosX < metadeRaio)
            {
                PosX = metadeRaio;
                // Colisão elástica com amortecimento
                VelX = -VelX * COEFICIENTE_RESTITUICAO;  // Inverte direção + perda energia
                // Transferência de momento linear para angular
                VelocidadeRotacao += VelX * 0.01;  // Efeito de "rolamento"
            }
            // Colisão com borda direita
            else if (PosX > canvasWidth - metadeRaio)
            {
                PosX = canvasWidth - metadeRaio;
                VelX = -VelX * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao -= VelX * 0.01;
            }

            // Colisão com borda superior
            if (PosY < metadeRaio)
            {
                PosY = metadeRaio;
                VelY = -VelY * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao += VelY * 0.01;
            }
            // Colisão com borda inferior
            else if (PosY > canvasHeight - metadeRaio)
            {
                PosY = canvasHeight - metadeRaio;
                VelY = -VelY * COEFICIENTE_RESTITUICAO;
                VelocidadeRotacao -= VelY * 0.01;
            }

            // Limitação física para evitar rotações infinitas
            if (Math.Abs(VelocidadeRotacao) > LIMITE_ROTACAO)
                VelocidadeRotacao *= FATOR_AMORTECIMENTO;  // Amortecimento angular
        }

        // ========== FUSÃO DE CORPOS (COLISÕES) ==========

        /// <summary>
        /// Cria um novo corpo resultante da fusão de dois corpos
        /// Conserva massa, momento linear e momento angular
        /// </summary>
        /// <param name="a">Primeiro corpo</param>
        /// <param name="b">Segundo corpo</param>
        /// <returns>Novo corpo resultante da fusão</returns>
        public static Corpo Fundir(Corpo a, Corpo b)
        {
            // FUSÃO ASTRONÔMICA - conservação de quantidades físicas:

            // 1. CONSERVAÇÃO DE MASSA: m_total = m1 + m2
            double massaTotal = a.Massa + b.Massa;

            // 2. DENSIDADE MÉDIA PONDERADA: ρ = (m1·ρ1 + m2·ρ2) / (m1 + m2)
            double densidadeMedia = (a.Densidade * a.Massa + b.Densidade * b.Massa) / massaTotal;

            // 3. CENTRO DE MASSA: pos = (m1·pos1 + m2·pos2) / (m1 + m2)
            double x = (a.PosX * a.Massa + b.PosX * b.Massa) / massaTotal;
            double y = (a.PosY * a.Massa + b.PosY * b.Massa) / massaTotal;

            // 4. CONSERVAÇÃO DE MOMENTO LINEAR: v = (m1·v1 + m2·v2) / (m1 + m2)
            double velX = (a.Massa * a.VelX + b.Massa * b.VelX) / massaTotal;
            double velY = (a.Massa * a.VelY + b.Massa * b.VelY) / massaTotal;

            // 5. CONSERVAÇÃO DE MOMENTO ANGULAR: L = m1·r1²·ω1 + m2·r2²·ω2
            // Momento angular total antes da fusão
            double momAngularTotal = a.Massa * a._raioQuadrado * a.VelocidadeRotacao +
                                   b.Massa * b._raioQuadrado * b.VelocidadeRotacao;

            // MISTURA DE CORES ASTRONÔMICA:
            // - Média ponderada pelas massas: cor = (m1·cor1 + m2·cor2) / (m1 + m2)
            // - Fisicamente: representa composição química combinada
            // - Cor dominante: corpo mais massivo tem maior influência
            string cor = MixColorsOtimizado(a.Cor, b.Cor, a.Massa, b.Massa);

            // Cria novo corpo com propriedades calculadas
            var novoCorpo = new Corpo($"Fus{++_contador}", massaTotal, densidadeMedia, x, y, cor)
            {
                VelX = velX,
                VelY = velY,
                VelocidadeRotacao = 0,  // Será calculada abaixo
                AnguloRotacao = (a.AnguloRotacao + b.AnguloRotacao) * 0.5,  // Ângulo médio
                Brilho = 1.0,
                // Tipo baseado na nova massa
                Tipo = massaTotal switch
                {
                    < 4 => TipoCorpo.Asteroide,
                    < 12 => TipoCorpo.Lua,
                    < 30 => TipoCorpo.PlanetaRochoso,
                    < 85 => TipoCorpo.GiganteGasoso,
                    _ => TipoCorpo.Estrela
                }
            };

            // 6. VELOCIDADE ANGULAR APÓS FUSÃO: ω = L / (m_total · r_novo²)
            // Onde r_novo é o raio do corpo resultante
            double novoRaioQuadrado = novoCorpo._raioQuadrado;
            if (novoRaioQuadrado > 0)
                novoCorpo.VelocidadeRotacao = momAngularTotal / (massaTotal * novoRaioQuadrado);

            novoCorpo.EhLuminoso = novoCorpo.Tipo == TipoCorpo.Estrela || novoCorpo.Tipo == TipoCorpo.AnaBranca;

            return novoCorpo;
        }

        // ========== MISTURA DE CORES OTIMIZADA ==========

        /// <summary>
        /// Mistura duas cores baseada na massa dos corpos (cor ponderada por massa)
        /// </summary>
        private static string MixColorsOtimizado(string corA, string corB, double massaA, double massaB)
        {
            try
            {
                // Verifica se ambas as cores são hexadecimais válidas
                if (corA.StartsWith("#") && corB.StartsWith("#") && corA.Length == 7 && corB.Length == 7)
                {
                    // Converte cores hexadecimais para componentes RGB
                    int r1 = Convert.ToInt32(corA.Substring(1, 2), 16);
                    int g1 = Convert.ToInt32(corA.Substring(3, 2), 16);
                    int b1 = Convert.ToInt32(corA.Substring(5, 2), 16);

                    int r2 = Convert.ToInt32(corB.Substring(1, 2), 16);
                    int g2 = Convert.ToInt32(corB.Substring(3, 2), 16);
                    int b2 = Convert.ToInt32(corB.Substring(5, 2), 16);

                    // MISTURA DE CORES ASTRONÔMICA:
                    // - Média ponderada pelas massas: cor = (m1·cor1 + m2·cor2) / (m1 + m2)
                    // - Fisicamente: representa composição química combinada
                    // - Cor dominante: corpo mais massivo tem maior influência

                    double total = massaA + massaB;
                    int r = (int)((r1 * massaA + r2 * massaB) / total);  // Vermelho ponderado
                    int g = (int)((g1 * massaA + g2 * massaB) / total);  // Verde ponderado  
                    int b = (int)((b1 * massaA + b2 * massaB) / total);  // Azul ponderado

                    // Garante que os valores estejam no range 0-255 e converte para hexadecimal
                    return $"#{Math.Min(255, Math.Max(0, r)):X2}{Math.Min(255, Math.Max(0, g)):X2}{Math.Min(255, Math.Max(0, b)):X2}";
                }
            }
            catch
            {
                // Em caso de erro na conversão, fallback físico: cor do corpo mais massivo domina
            }

            return massaA >= massaB ? corA : corB;
        }
    }

    /// <summary>
    /// Enumeração dos tipos de corpos celestes suportados pela simulação
    /// Baseada em classificações astronômicas reais
    /// </summary>
    public enum TipoCorpo
    {
        /// <summary>Pequenos corpos rochosos ou metálicos</summary>
        Asteroide,

        /// <summary>Corpos de gelo e poeira com órbitas excêntricas</summary>
        Cometa,

        /// <summary>Satélites naturais que orbitam planetas</summary>
        Lua,

        /// <summary>Planetas com superfície sólida como Terra e Marte</summary>
        PlanetaRochoso,

        /// <summary>Planetas massivos compostos principalmente de gases</summary>
        GiganteGasoso,

        /// <summary>Planetas compostos principalmente de gelo como Urano e Netuno</summary>
        GiganteGelo,

        /// <summary>Restos densos de estrelas de baixa massa</summary>
        AnaBranca,

        /// <summary>Corpos celestes que emitem luz própria por fusão nuclear</summary>
        Estrela
    }
}