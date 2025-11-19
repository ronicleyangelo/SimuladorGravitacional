using ProgramacaoAvancada.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        // ✅ Usar SUA classe Universo existente
        private Universo _universo;

        public List<Corpo> Corpos => _universo.Corpos;
        public List<string> Eventos { get; private set; } = new List<string>();

        // ✅ CORREÇÃO CRÍTICA: Garantir que Iteracoes e Colisoes são incrementadas
        public int Iteracoes { get; private set; } = 0;
        public int Colisoes { get; private set; } = 0;

        // ✅ Área expandida para 1000+ corpos
        public double CanvasWidth { get; set; } = 2000;
        public double CanvasHeight { get; set; } = 1600;
        public double Gravidade { get; set; } = 50.0;

        // ✅ SEM LIMITE - apenas contador de referência
        public int NumCorpos
        {
            get => Corpos.Count;
            set { } // Ignorar set, usar métodos de adição
        }

        public bool Executando { get; private set; } = false;
        public bool Rodando => Executando;

        // ✅ Configuração de paralelismo
        public bool UsarParalelismo { get; set; } = true;
        public int GrauParalelismo { get; set; } = Environment.ProcessorCount;

        // ✅ Sistema de auto-otimização
        private OtimizadorParalelo _otimizador = new OtimizadorParalelo();
        private object _lockObject = new object();

        // ✅ CORREÇÃO: Adicionar evento para notificar mudanças
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public SimuladorService()
        {
            Resetar();
        }

        public void Resetar(int quantidadeCorpos = 20)
        {
            // ✅ CORREÇÃO: Usar fator de gravidade MUITO maior
            double fatorGravidade = 1e10 * Gravidade;

            _universo = new Universo(CanvasWidth, CanvasHeight, fatorGravidade);

            // Limpar corpos existentes
            Corpos.Clear();

            // ✅ ADICIONAR CORPOS SEM LIMITE
            if (quantidadeCorpos > 0)
            {
                AdicionarCorpos(quantidadeCorpos);
            }

            // ✅ CORREÇÃO: Zerar contadores com notificação
            Iteracoes = 0;
            Colisoes = 0;
            Eventos.Clear();

            AdicionarEvento($"🌌 Universo criado com {quantidadeCorpos} corpos");
            AdicionarEvento($"📐 Área: {CanvasWidth}x{CanvasHeight} (4x maior)");
            AdicionarEvento($"⚡ Gravidade: {Gravidade}, Fator: {fatorGravidade:e2}");
            AdicionarEvento($"🎯 Sistema realista com movimento inicial");
            AdicionarEvento($"🔧 Paralelismo: {(UsarParalelismo ? "Ativo" : "Inativo")}");

            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA ADICIONAR CORPOS DINAMICAMENTE SEM LIMITAÇÃO
        public void AdicionarCorpos(int quantidade)
        {
            if (quantidade <= 0) return;

            var random = new Random();
            int corposAdicionados = 0;

            // ✅ ADICIONAR EM LOTE COM PARALELISMO para muitos corpos
            if (quantidade > 500 && UsarParalelismo)
            {
                var novosCorpos = new Corpo[quantidade];
                var options = new ParallelOptions { MaxDegreeOfParallelism = GrauParalelismo };

                Parallel.For(0, quantidade, options, i =>
                {
                    var novoCorpo = Corpo.CriarRealistaAleatorio(CanvasWidth, CanvasHeight);
                    // ✅ AUMENTADO: Velocidades iniciais para área maior
                    novoCorpo.VelX = (random.NextDouble() - 0.5) * 3.0;
                    novoCorpo.VelY = (random.NextDouble() - 0.5) * 3.0;
                    novosCorpos[i] = novoCorpo;
                });

                lock (_lockObject)
                {
                    foreach (var corpo in novosCorpos)
                    {
                        _universo.AdicionarCorpo(corpo);
                    }
                    corposAdicionados = quantidade;
                }
            }
            else
            {
                // ✅ ADIÇÃO SEQUENCIAL para poucos corpos
                lock (_lockObject)
                {
                    for (int i = 0; i < quantidade; i++)
                    {
                        var novoCorpo = Corpo.CriarRealistaAleatorio(CanvasWidth, CanvasHeight);
                        novoCorpo.VelX = (random.NextDouble() - 0.5) * 3.0;
                        novoCorpo.VelY = (random.NextDouble() - 0.5) * 3.0;
                        _universo.AdicionarCorpo(novoCorpo);
                        corposAdicionados++;
                    }
                }
            }

            AdicionarEvento($"🆕 {corposAdicionados} corpos adicionados. Total: {Corpos.Count}");
            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA ADICIONAR CORPOS EM POSIÇÕES ESPECÍFICAS
        public void AdicionarCorposEmPosicoes(List<Tuple<double, double>> posicoes)
        {
            if (posicoes == null || !posicoes.Any()) return;

            int corposAdicionados = 0;
            var random = new Random();

            if (posicoes.Count > 100 && UsarParalelismo)
            {
                var novosCorpos = new Corpo[posicoes.Count];
                var options = new ParallelOptions { MaxDegreeOfParallelism = GrauParalelismo };

                Parallel.For(0, posicoes.Count, options, i =>
                {
                    var pos = posicoes[i];
                    var novoCorpo = Corpo.CriarRealistaAleatorio(CanvasWidth, CanvasHeight);
                    novoCorpo.PosX = pos.Item1;
                    novoCorpo.PosY = pos.Item2;
                    novoCorpo.VelX = (random.NextDouble() - 0.5) * 3.0;
                    novoCorpo.VelY = (random.NextDouble() - 0.5) * 3.0;
                    novosCorpos[i] = novoCorpo;
                });

                lock (_lockObject)
                {
                    foreach (var corpo in novosCorpos)
                    {
                        _universo.AdicionarCorpo(corpo);
                    }
                    corposAdicionados = posicoes.Count;
                }
            }
            else
            {
                lock (_lockObject)
                {
                    foreach (var pos in posicoes)
                    {
                        var novoCorpo = Corpo.CriarRealistaAleatorio(CanvasWidth, CanvasHeight);
                        novoCorpo.PosX = pos.Item1;
                        novoCorpo.PosY = pos.Item2;
                        novoCorpo.VelX = (random.NextDouble() - 0.5) * 3.0;
                        novoCorpo.VelY = (random.NextDouble() - 0.5) * 3.0;
                        _universo.AdicionarCorpo(novoCorpo);
                        corposAdicionados++;
                    }
                }
            }

            AdicionarEvento($"📍 {corposAdicionados} corpos adicionados em posições específicas. Total: {Corpos.Count}");
            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA ADICIONAR CORPOS COM CONFIGURAÇÃO PERSONALIZADA
        public void AdicionarCorposPersonalizados(List<Corpo> corposPersonalizados)
        {
            if (corposPersonalizados == null || !corposPersonalizados.Any()) return;

            lock (_lockObject)
            {
                foreach (var corpo in corposPersonalizados)
                {
                    _universo.AdicionarCorpo(corpo);
                }
            }

            AdicionarEvento($"🎨 {corposPersonalizados.Count} corpos personalizados adicionados. Total: {Corpos.Count}");
            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA REMOVER CORPOS
        public void RemoverCorpos(int quantidade)
        {
            if (quantidade <= 0) return;

            lock (_lockObject)
            {
                int quantidadeRemover = Math.Min(quantidade, Corpos.Count);
                if (quantidadeRemover > 0)
                {
                    // Remove os primeiros X corpos
                    Corpos.RemoveRange(0, quantidadeRemover);
                    AdicionarEvento($"🗑️ {quantidadeRemover} corpos removidos. Total: {Corpos.Count}");
                    NotifyStateChanged();
                }
            }
        }

        // ✅ MÉTODO PARA LIMPAR TODOS OS CORPOS
        public void LimparTodosCorpos()
        {
            lock (_lockObject)
            {
                int quantidade = Corpos.Count;
                Corpos.Clear();
                Iteracoes = 0;
                Colisoes = 0;
                AdicionarEvento($"🧹 Todos os {quantidade} corpos removidos");
                NotifyStateChanged();
            }
        }

        public void Iniciar()
        {
            Executando = true;
            AdicionarEvento("🚀 Simulação iniciada");
            NotifyStateChanged();
        }

        public void Parar()
        {
            Executando = false;
            AdicionarEvento("⏸️ Simulação parada");
            NotifyStateChanged();
        }

        public void Pausar() => Parar();

        // ✅ CORREÇÃO CRÍTICA: Método Atualizar com incremento garantido de Iteracoes
        public void Atualizar()
        {
            if (!Executando) return;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                int countAntes = Corpos.Count;
                bool usarParalelo = _otimizador.DeveUsarParalelismo(countAntes) && UsarParalelismo;

                if (usarParalelo && countAntes > 20)
                {
                    // ✅ MODO PARALELO ULTRA-OTIMIZADO
                    _universo.SimularParalelo(0.05);
                }
                else
                {
                    // ✅ MODO SEQUENCIAL (para poucos corpos)
                    _universo.Simular(0.05);
                }

                // ✅ CORREÇÃO CRÍTICA: INCREMENTAR ITERAÇÕES SEMPRE
                Iteracoes++;
                stopwatch.Stop();

                // ✅ REGISTRAR PARA AUTO-OTIMIZAÇÃO
                _otimizador.RegistrarTempo(usarParalelo, stopwatch.Elapsed.TotalMilliseconds);

                // ✅ CORREÇÃO CRÍTICA: DETECTAR COLISÕES CORRETAMENTE
                int countDepois = Corpos.Count;
                if (countDepois < countAntes)
                {
                    int novasColisoes = countAntes - countDepois;
                    Colisoes += novasColisoes;
                    AdicionarEvento($"💥 {novasColisoes} fusão(ões)! {countDepois} corpos restantes");
                }

                // ✅ FEEDBACK DE PERFORMANCE
                if (Iteracoes % 25 == 0)
                {
                    var speedup = _otimizador.CalcularSpeedupMedio();
                    AdicionarEvento($"📈 It: {Iteracoes} | Corpos: {Corpos.Count} | Col: {Colisoes}");
                    AdicionarEvento($"⚡ Paralelo: {usarParalelo} | Speedup: {speedup:0.0}x | Tempo: {stopwatch.Elapsed.TotalMilliseconds:0.000}ms");

                    if (Corpos.Any())
                    {
                        double velocidadeMedia;
                        if (UsarParalelismo && Corpos.Count > 1000)
                        {
                            velocidadeMedia = Corpos.AsParallel()
                                .WithDegreeOfParallelism(GrauParalelismo)
                                .Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));
                        }
                        else
                        {
                            velocidadeMedia = Corpos.Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));
                        }

                        if (velocidadeMedia > 0.01)
                        {
                            AdicionarEvento($"🎯 Velocidade média: {velocidadeMedia:0.000}");
                        }
                    }
                }

                // ✅ DETECTAR FIM DA SIMULAÇÃO
                if (Corpos.Count <= 1)
                {
                    if (Corpos.Count == 1)
                    {
                        var corpoFinal = Corpos[0];
                        AdicionarEvento($"🏁 CONCLUÍDO! Corpo final: {corpoFinal.Tipo} (Massa: {corpoFinal.Massa:0.0})");
                    }
                    else
                    {
                        AdicionarEvento($"🏁 CONCLUÍDO! Todos os corpos fundiram-se");
                    }
                    Executando = false;
                }

                // ✅ CORREÇÃO: Notificar mudanças após cada atualização
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro na iteração {Iteracoes}: {ex.Message}");
                Executando = false;
                NotifyStateChanged();
            }
        }

        // ✅ MÉTODO PARA SISTEMA SOLAR SIMULADO SEM LIMITE DE PLANETAS
        public void CriarSistemaSolar(int numeroPlanetas = 8)
        {
            double fatorGravidade = 1e12 * Gravidade;
            _universo = new Universo(CanvasWidth, CanvasHeight, fatorGravidade);
            Corpos.Clear();

            // ✅ CORREÇÃO: Zerar contadores
            Iteracoes = 0;
            Colisoes = 0;

            var random = new Random();

            // ✅ SOL (no centro)
            var sol = new Corpo("Sol", 200.0, 1.0, CanvasWidth / 2, CanvasHeight / 2, "#FFD700")
            {
                VelX = 0,
                VelY = 0,
                Tipo = TipoCorpo.Estrela,
                EhLuminoso = true,
                Brilho = 1.0,
                VelocidadeRotacao = 0.02
            };
            _universo.AdicionarCorpo(sol);

            // ✅ PLANETAS (em órbitas) - SEM LIMITE
            string[] nomesBase = { "Mercúrio", "Vênus", "Terra", "Marte", "Júpiter", "Saturno", "Urano", "Netuno" };
            string[] coresBase = { "#A9A9A9", "#FFA500", "#1E90FF", "#FF4500", "#DEB887", "#F0E68C", "#87CEEB", "#1E90FF" };

            // ✅ AUMENTADO: Órbitas maiores para área expandida
            int orbitaBase = 200;

            // ✅ ADICIONAR PLANETAS EM PARALELO se muitos
            if (numeroPlanetas > 10 && UsarParalelismo)
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = GrauParalelismo };
                var planetas = new List<Corpo>();

                Parallel.For(0, numeroPlanetas, options, i =>
                {
                    double raioOrbita = orbitaBase + (i * 100);
                    double angulo = random.NextDouble() * 2 * Math.PI;

                    double posX = CanvasWidth / 2 + Math.Cos(angulo) * raioOrbita;
                    double posY = CanvasHeight / 2 + Math.Sin(angulo) * raioOrbita;

                    double velocidadeOrbital = Math.Sqrt(200.0 / raioOrbita) * 2.5;
                    double velX = -Math.Sin(angulo) * velocidadeOrbital;
                    double velY = Math.Cos(angulo) * velocidadeOrbital;

                    string nome = i < nomesBase.Length ? nomesBase[i] : $"Planeta {i + 1}";
                    string cor = i < coresBase.Length ? coresBase[i] : GerarCorAleatoria(random);

                    var planeta = new Corpo(nome, 10.0 + i * 3, 2.0 + i * 0.3, posX, posY, cor)
                    {
                        VelX = velX,
                        VelY = velY,
                        Tipo = i < 4 ? TipoCorpo.PlanetaRochoso : TipoCorpo.GiganteGasoso,
                        VelocidadeRotacao = 0.01 + i * 0.003
                    };

                    lock (_lockObject)
                    {
                        planetas.Add(planeta);
                    }
                });

                foreach (var planeta in planetas)
                {
                    _universo.AdicionarCorpo(planeta);
                }
            }
            else
            {
                // Método sequencial para poucos planetas
                for (int i = 0; i < numeroPlanetas; i++)
                {
                    double raioOrbita = orbitaBase + (i * 100);
                    double angulo = random.NextDouble() * 2 * Math.PI;

                    double posX = CanvasWidth / 2 + Math.Cos(angulo) * raioOrbita;
                    double posY = CanvasHeight / 2 + Math.Sin(angulo) * raioOrbita;

                    double velocidadeOrbital = Math.Sqrt(200.0 / raioOrbita) * 2.5;
                    double velX = -Math.Sin(angulo) * velocidadeOrbital;
                    double velY = Math.Cos(angulo) * velocidadeOrbital;

                    string nome = i < nomesBase.Length ? nomesBase[i] : $"Planeta {i + 1}";
                    string cor = i < coresBase.Length ? coresBase[i] : GerarCorAleatoria(random);

                    var planeta = new Corpo(nome, 10.0 + i * 3, 2.0 + i * 0.3, posX, posY, cor)
                    {
                        VelX = velX,
                        VelY = velY,
                        Tipo = i < 4 ? TipoCorpo.PlanetaRochoso : TipoCorpo.GiganteGasoso,
                        VelocidadeRotacao = 0.01 + i * 0.003
                    };

                    _universo.AdicionarCorpo(planeta);
                }
            }

            AdicionarEvento($"☀️ Sistema Solar criado com {numeroPlanetas} planetas em área expandida!");
            NotifyStateChanged();
        }

        private string GerarCorAleatoria(Random random)
        {
            return string.Format("#{0:X6}", random.Next(0x1000000));
        }

        // ✅ MÉTODO PARA CRIAR AGLOMERADO SEM LIMITE
        public void CriarAglomerado(int quantidadeCorpos = 100)
        {
            double fatorGravidade = 1e11 * Gravidade;
            _universo = new Universo(CanvasWidth, CanvasHeight, fatorGravidade);
            Corpos.Clear();

            // ✅ CORREÇÃO: Zerar contadores
            Iteracoes = 0;
            Colisoes = 0;

            // ✅ SIMPLESMENTE ADICIONAR CORPOS SEM LIMITE
            AdicionarCorpos(quantidadeCorpos);

            AdicionarEvento($"🌠 Aglomerado criado com {quantidadeCorpos} corpos em área expandida!");
            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA CRIAR GALÁXIA COM MUITOS CORPOS
        public void CriarGalaxia(int quantidadeEstrelas = 1000)
        {
            double fatorGravidade = 1e11 * Gravidade;
            _universo = new Universo(CanvasWidth, CanvasHeight, fatorGravidade);
            Corpos.Clear();

            // ✅ CORREÇÃO: Zerar contadores
            Iteracoes = 0;
            Colisoes = 0;

            var random = new Random();

            // ✅ NÚCLEO GALÁCTICO (grande massa central)
            var nucleo = new Corpo("Núcleo Galáctico", 500.0, 5.0, CanvasWidth / 2, CanvasHeight / 2, "#FFD700")
            {
                VelX = 0,
                VelY = 0,
                Tipo = TipoCorpo.Estrela,
                EhLuminoso = true,
                Brilho = 2.0
            };
            _universo.AdicionarCorpo(nucleo);

            // ✅ ADICIONAR ESTRELAS EM ESPIRAL
            AdicionarCorpos(quantidadeEstrelas);

            AdicionarEvento($"🌌 Galáxia criada com {quantidadeEstrelas + 1} corpos em área expandida!");
            NotifyStateChanged();
        }

        // ✅ NOVO MÉTODO: CRIAR SIMULAÇÃO GIGANTE
        public void CriarSimulacaoGigante(int quantidadeCorpos = 5000)
        {
            double fatorGravidade = 1e10 * Gravidade;
            _universo = new Universo(CanvasWidth, CanvasHeight, fatorGravidade);
            Corpos.Clear();

            // ✅ CORREÇÃO: Zerar contadores
            Iteracoes = 0;
            Colisoes = 0;

            // ✅ ADICIONAR MUITOS CORPOS COM PARALELISMO
            AdicionarCorpos(quantidadeCorpos);

            AdicionarEvento($"🌠 SIMULAÇÃO GIGANTE criada com {quantidadeCorpos} corpos!");
            AdicionarEvento($"📐 Área máxima: {CanvasWidth}x{CanvasHeight}");
            AdicionarEvento($"⚡ Use paralelismo para melhor performance");
            NotifyStateChanged();
        }

        private void AdicionarEvento(string mensagem)
        {
            Eventos.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {mensagem}");
            if (Eventos.Count > 100) Eventos.RemoveAt(Eventos.Count - 1);
        }

        public void AdicionarEventoManual(string mensagem) => AdicionarEvento($"💬 {mensagem}");

        // ✅ MÉTODO PARA CONFIGURAR PARALELISMO
        public void ConfigurarParalelismo(bool usarParalelismo, int? grauParalelismo = null)
        {
            UsarParalelismo = usarParalelismo;
            if (grauParalelismo.HasValue)
            {
                GrauParalelismo = Math.Max(1, Math.Min(Environment.ProcessorCount * 2, grauParalelismo.Value));
            }

            AdicionarEvento($"🔧 Paralelismo {(usarParalelismo ? "ativado" : "desativado")} " +
                          $"{(grauParalelismo.HasValue ? $"(Grau: {GrauParalelismo})" : "")}");
            NotifyStateChanged();
        }

        // ✅ MÉTODO PARA CONFIGURAÇÃO AUTOMÁTICA
        public void ConfigurarParalelismoAutomatico()
        {
            int numCores = Environment.ProcessorCount;
            int numCorpos = Corpos.Count;

            if (numCorpos < 100)
            {
                UsarParalelismo = false;
                GrauParalelismo = 1;
            }
            else if (numCorpos < 500)
            {
                UsarParalelismo = true;
                GrauParalelismo = Math.Max(2, numCores / 2);
            }
            else // 500+ corpos
            {
                UsarParalelismo = true;
                GrauParalelismo = numCores;
            }

            AdicionarEvento($"🔧 Paralelismo automático: {(UsarParalelismo ? "Ativo" : "Inativo")}");
            AdicionarEvento($"🎯 Configuração: {numCorpos} corpos → {GrauParalelismo} threads");
            NotifyStateChanged();
        }

        public string GerarConteudoArquivo()
        {
            // ✅ CÁLCULO PARALELO DA DISTRIBUIÇÃO para muitos corpos
            var distribuicao = UsarParalelismo && Corpos.Count > 1000
                ? Corpos.AsParallel()
                    .WithDegreeOfParallelism(GrauParalelismo)
                    .GroupBy(c => c.Tipo)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
                : Corpos.GroupBy(c => c.Tipo)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

            return $"=== SIMULAÇÃO GRAVITACIONAL ===\n" +
                   $"Corpos: {Corpos.Count}\n" +
                   $"Iterações: {Iteracoes}\n" +
                   $"Colisões: {Colisoes}\n" +
                   $"Área: {CanvasWidth}x{CanvasHeight}\n" +
                   $"Paralelismo: {(UsarParalelismo ? "Ativo" : "Inativo")}\n" +
                   $"Speedup Médio: {_otimizador.CalcularSpeedupMedio():0.0}x\n" +
                   $"Distribuição: {string.Join(", ", distribuicao.Select(kv => $"{kv.Key}:{kv.Value}"))}";
        }

        public void CarregarDeTxt(string conteudo)
        {
            AdicionarEvento("📁 Arquivo carregado: " + conteudo.Substring(0, Math.Min(50, conteudo.Length)) + "...");
            NotifyStateChanged();
        }

        public void AdicionarCorpoPersonalizado(double x, double y, string tipoStr)
        {
            try
            {
                if (Enum.TryParse<TipoCorpo>(tipoStr, out var tipo))
                {
                    var corpo = Corpo.CriarRealistaAleatorio(CanvasWidth, CanvasHeight);
                    corpo.PosX = x;
                    corpo.PosY = y;
                    corpo.VelX = (new Random().NextDouble() - 0.5) * 3.0;
                    corpo.VelY = (new Random().NextDouble() - 0.5) * 3.0;

                    _universo.AdicionarCorpo(corpo);
                    AdicionarEvento($"🆕 {tipo} adicionado em ({x:0}, {y:0}). Total: {Corpos.Count}");
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro ao adicionar corpo: {ex.Message}");
            }
        }

        public object ObterEstatisticasDetalhadas()
        {
            if (!Corpos.Any())
                return new { Mensagem = "Nenhum corpo na simulação" };

            // ✅ CÁLCULOS PARALELOS PARA GRANDES QUANTIDADES
            double massaTotal, massaMedia, velocidadeMedia;
            var distribuicao = new Dictionary<string, int>();

            if (UsarParalelismo && Corpos.Count > 1000)
            {
                massaTotal = Corpos.AsParallel()
                    .WithDegreeOfParallelism(GrauParalelismo)
                    .Sum(c => c.Massa);

                massaMedia = Corpos.AsParallel()
                    .WithDegreeOfParallelism(GrauParalelismo)
                    .Average(c => c.Massa);

                velocidadeMedia = Corpos.AsParallel()
                    .WithDegreeOfParallelism(GrauParalelismo)
                    .Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));

                distribuicao = Corpos.AsParallel()
                    .WithDegreeOfParallelism(GrauParalelismo)
                    .GroupBy(c => c.Tipo)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());
            }
            else
            {
                massaTotal = Corpos.Sum(c => c.Massa);
                massaMedia = Corpos.Average(c => c.Massa);
                velocidadeMedia = Corpos.Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));
                distribuicao = Corpos.GroupBy(c => c.Tipo)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());
            }

            var statsPerformance = _otimizador.ObterEstatisticas();

            return new
            {
                TotalCorpos = Corpos.Count,
                MassaTotal = massaTotal,
                MassaMedia = massaMedia,
                VelocidadeMedia = velocidadeMedia,
                DistribuicaoTipos = distribuicao,
                Iteracoes = Iteracoes,
                Colisoes = Colisoes,
                Executando = Executando,
                Area = $"{CanvasWidth}x{CanvasHeight}",
                Paralelismo = UsarParalelismo ? $"Ativo (Grau: {GrauParalelismo})" : "Inativo",
                Performance = new
                {
                    SpeedupMedio = statsPerformance.SpeedupMedio,
                    TempoMedioFrame = statsPerformance.TempoMedioFrame,
                    Eficiencia = statsPerformance.Eficiencia
                }
            };
        }

        // ✅ MÉTODO DE DIAGNÓSTICO MELHORADO
        public string Diagnosticar()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== DIAGNÓSTICO ===");
            sb.AppendLine($"Corpos: {Corpos.Count}");
            sb.AppendLine($"Iterações: {Iteracoes}");
            sb.AppendLine($"Colisões: {Colisoes}");
            sb.AppendLine($"Área: {CanvasWidth}x{CanvasHeight}");
            sb.AppendLine($"Executando: {Executando}");
            sb.AppendLine($"Paralelismo: {(UsarParalelismo ? "Ativo" : "Inativo")}");
            sb.AppendLine($"Grau Paralelismo: {GrauParalelismo}");
            sb.AppendLine($"Processadores: {Environment.ProcessorCount}");

            var stats = _otimizador.ObterEstatisticas();
            sb.AppendLine($"Speedup Médio: {stats.SpeedupMedio:0.0}x");
            sb.AppendLine($"Eficiência: {stats.Eficiencia:0}%");
            sb.AppendLine($"Tempo/Frame: {stats.TempoMedioFrame:0.000}ms");

            if (Corpos.Any())
            {
                double velMedia;
                if (UsarParalelismo && Corpos.Count > 1000)
                {
                    velMedia = Corpos.AsParallel()
                        .WithDegreeOfParallelism(GrauParalelismo)
                        .Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));
                }
                else
                {
                    velMedia = Corpos.Average(c => Math.Sqrt(c.VelX * c.VelX + c.VelY * c.VelY));
                }

                sb.AppendLine($"Velocidade média: {velMedia:0.000000}");

                if (velMedia < 0.001)
                    sb.AppendLine("⚠️  Velocidades muito baixas - Aumente a gravidade!");
                else
                    sb.AppendLine("✅ Movimento detectado");

                // Informações de performance
                if (Corpos.Count > 1000)
                {
                    sb.AppendLine($"💡 Performance: {(UsarParalelismo ? "Otimizada para muitos corpos" : "Considere ativar paralelismo")}");
                }

                // Densidade de corpos
                double areaTotal = CanvasWidth * CanvasHeight;
                double densidade = Corpos.Count / areaTotal * 1000000;
                sb.AppendLine($"📊 Densidade: {densidade:0.00} corpos/Mpx");

                if (densidade > 10)
                    sb.AppendLine("💡 Área bem utilizada");
                else if (densidade > 5)
                    sb.AppendLine("💡 Área moderadamente utilizada");
                else
                    sb.AppendLine("💡 Área com espaço disponível");
            }

            return sb.ToString();
        }
    }


    // ✅ CLASSE DE OTIMIZAÇÃO PARALELA
    public class OtimizadorParalelo
    {
        private readonly Queue<double> _temposSequencial = new();
        private readonly Queue<double> _temposParalelo = new();
        private const int HISTORICO_MAXIMO = 10;

        public bool DeveUsarParalelismo(int numCorpos)
        {
            // ✅ HEURÍSTICAS INTELIGENTES
            if (numCorpos < 20) return false;
            if (numCorpos > 500) return true;

            // ✅ DECISÃO BASEADA EM HISTÓRICO
            double speedupMedio = CalcularSpeedupMedio();
            return speedupMedio > 1.2; // Só usa paralelo se for 20% mais rápido
        }

        public void RegistrarTempo(bool paralelo, double tempo)
        {
            var fila = paralelo ? _temposParalelo : _temposSequencial;
            fila.Enqueue(tempo);

            if (fila.Count > HISTORICO_MAXIMO)
                fila.Dequeue();
        }

        public double CalcularSpeedupMedio()
        {
            if (_temposSequencial.Count == 0 || _temposParalelo.Count == 0)
                return 1.0;

            double tempoSeq = _temposSequencial.Average();
            double tempoPar = _temposParalelo.Average();

            return tempoSeq / tempoPar;
        }

        public (double SpeedupMedio, double TempoMedioFrame, double Eficiencia) ObterEstatisticas()
        {
            double speedup = CalcularSpeedupMedio();
            double tempoMedio = (_temposSequencial.Concat(_temposParalelo).Any()) ?
                               _temposSequencial.Concat(_temposParalelo).Average() : 0;
            double eficiencia = (speedup / Environment.ProcessorCount) * 100;

            return (speedup, tempoMedio, eficiencia);
        }
    }
}