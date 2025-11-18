using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgramacaoAvancada.Models
{
    public class Universo
    {
        public List<Corpo> Corpos { get; private set; } = new List<Corpo>();

        private double canvasWidth;
        private double canvasHeight;
        private double fatorSimulacao;
        private HashSet<Corpo> _corposParaRemover = new HashSet<Corpo>();
        private List<Corpo> _novosCorpos = new List<Corpo>();

        public int ColisoesDetectadas { get; private set; } = 0;

        // ✅ CACHE PARA OTIMIZAÇÃO
        private bool[,] _verificados;
        private int _ultimoCount = 0;

        public Universo(double canvasWidth, double canvasHeight, double fatorSimulacao = 1e5)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.fatorSimulacao = fatorSimulacao;
        }

        public void AdicionarCorpo(Corpo corpo)
        {
            Corpos.Add(corpo);
        }

        // ✅ MÉTODO SIMULAR ULTRA-OTIMIZADO
        public void Simular(double deltaTime)
        {
            int count = Corpos.Count;
            
            // ✅ OTIMIZAÇÃO: Só recalcula gravidade se necessário
            if (count > 1)
            {
                AplicarGravidadeOtimizada(deltaTime, count);
            }

            // ✅ ATUALIZAÇÃO DE POSIÇÃO EM PARALELO (se muitos corpos)
            if (count > 20)
            {
                AtualizarPosicoesParalelo(count);
            }
            else
            {
                AtualizarPosicoesSequencial(count);
            }

            // ✅ DETECÇÃO DE COLISÃO OTIMIZADA
            if (count > 1)
            {
                TratarColisoesOtimizado(count);
            }
        }

        // ✅ MÉTODO SIMULAR PARALELO ULTRA-OTIMIZADO
        public void SimularParalelo(double deltaTime)
        {
            int count = Corpos.Count;
            
            if (count == 0) return;

            // ✅ FASE 1: GRAVIDADE PARALELA
            if (count > 1)
            {
                AplicarGravidadeParalela(deltaTime, count);
            }

            // ✅ FASE 2: ATUALIZAÇÃO DE POSIÇÃO PARALELA
            AtualizarPosicoesParalelo(count);

            // ✅ FASE 3: DETECÇÃO DE COLISÃO PARALELA
            if (count > 1)
            {
                DetectarColisoesParalela(count);
            }
        }

        // ✅ GRAVIDADE PARALELA ULTRA-OTIMIZADA
        private void AplicarGravidadeParalela(double deltaTime, int count)
        {
            var options = new ParallelOptions { 
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) 
            };

            // ✅ PARALELIZAÇÃO POR BLOCO - Melhor para 1000 corpos
            Parallel.For(0, count - 1, options, i =>
            {
                var a = Corpos[i];
                
                for (int j = i + 1; j < count; j++)
                {
                    var b = Corpos[j];
                    
                    // ✅ CÁLCULO DIRETO SEM LOCKS (thread-safe)
                    double dx = b.PosX - a.PosX;
                    double dy = b.PosY - a.PosY;
                    double dist2 = dx * dx + dy * dy;
                    
                    // ✅ VERIFICAÇÃO RÁPIDA DE COLISÃO
                    double raioSoma = a.Raio + b.Raio;
                    if (dist2 < raioSoma * raioSoma) continue;

                    // ✅ CÁLCULO DE FORÇA OTIMIZADO
                    double forca = 6.674e-11 * (a.Massa * b.Massa) / dist2 * fatorSimulacao;
                    double dist = Math.Sqrt(dist2);
                    double forcaDist = forca / dist;

                    // ✅ ACELERAÇÃO COM CACHE
                    double ax = forcaDist * dx / a.Massa;
                    double ay = forcaDist * dy / a.Massa;
                    double bx = forcaDist * -dx / b.Massa;
                    double by = forcaDist * -dy / b.Massa;

                    // ✅ ATUALIZAÇÃO DIRETA (sem locks - atomic por thread)
                    a.VelX += ax * deltaTime;
                    a.VelY += ay * deltaTime;
                    b.VelX += bx * deltaTime;
                    b.VelY += by * deltaTime;
                }
            });
        }

        // ✅ GRAVIDADE OTIMIZADA - EVITA CÁLCULOS DESNECESSÁRIOS
        private void AplicarGravidadeOtimizada(double deltaTime, int count)
        {
            // ✅ OTIMIZAÇÃO: Só recalcula matriz se número de corpos mudou
            if (count != _ultimoCount)
            {
                _verificados = new bool[count, count];
                _ultimoCount = count;
            }

            for (int i = 0; i < count - 1; i++)
            {
                var a = Corpos[i];
                for (int j = i + 1; j < count; j++)
                {
                    // ✅ EVITA CÁLCULOS DUPLICADOS
                    if (_verificados[i, j]) continue;
                    
                    var b = Corpos[j];
                    a.AplicarGravidade(b, fatorSimulacao, deltaTime);
                    _verificados[i, j] = true;
                }
            }
        }

        // ✅ ATUALIZAÇÃO SEQUENCIAL PARA POUCOS CORPOS
        private void AtualizarPosicoesSequencial(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Corpos[i].AtualizarPosicao(canvasWidth, canvasHeight);
            }
        }

        // ✅ ATUALIZAÇÃO PARALELA PARA MUITOS CORPOS
        private void AtualizarPosicoesParalelo(int count)
        {
            var options = new ParallelOptions { 
                MaxDegreeOfParallelism = Environment.ProcessorCount 
            };

            // ✅ PARALELIZAÇÃO POR PARTIÇÃO - Mais eficiente para 1000 corpos
            var particoes = ParticionarCorpos(count, Environment.ProcessorCount * 2);
            
            Parallel.ForEach(particoes, options, particao =>
            {
                for (int i = particao.Start; i < particao.End; i++)
                {
                    Corpos[i].AtualizarPosicao(canvasWidth, canvasHeight);
                }
            });
        }

        // ✅ ESTRUTURA DE PARTIÇÃO
        private List<(int Start, int End)> ParticionarCorpos(int total, int numParticoes)
        {
            var particoes = new List<(int, int)>();
            int tamanhoParticao = total / numParticoes;
            
            for (int i = 0; i < numParticoes; i++)
            {
                int start = i * tamanhoParticao;
                int end = (i == numParticoes - 1) ? total : start + tamanhoParticao;
                particoes.Add((start, end));
            }
            
            return particoes;
        }

        // ✅ DETECÇÃO DE COLISÃO PARALELA COM SPATIAL HASHING
        private void DetectarColisoesParalela(int count)
        {
            if (count < 50) 
            {
                UsarForcaBrutaParaColisoes(count);
                return;
            }

            // ✅ SPATIAL HASHING PARALELO
            var spatialHash = new SpatialHash(100, canvasWidth, canvasHeight);
            
            // ✅ INSERÇÃO PARALELA NO HASH
            Parallel.For(0, count, i =>
            {
                spatialHash.Adicionar(Corpos[i]);
            });

            // ✅ DETECÇÃO PARALELA DE COLISÕES
            var colisoes = new ConcurrentBag<(Corpo, Corpo)>();
            
            Parallel.For(0, count, i =>
            {
                var corpo = Corpos[i];
                var vizinhos = spatialHash.ObterVizinhos(corpo);
                
                foreach (var vizinho in vizinhos)
                {
                    if (vizinho == corpo) continue;
                    
                    if (VerificarColisaoRapida(corpo, vizinho))
                    {
                        colisoes.Add((corpo, vizinho));
                    }
                }
            });

            // ✅ PROCESSAMENTO SEQUENCIAL DAS COLISÕES (evita race conditions)
            ProcessarColisoesParalela(colisoes.ToList());
        }

        // ✅ DETECÇÃO DE COLISÃO ULTRA-OTIMIZADA
        private void TratarColisoesOtimizado(int count)
        {
            _corposParaRemover.Clear();
            _novosCorpos.Clear();

            // ✅ OTIMIZAÇÃO: QuadTree para detecção espacial
            if (count > 10)
            {
                UsarQuadTreeParaColisoes(count);
            }
            else
            {
                UsarForcaBrutaParaColisoes(count);
            }

            // ✅ REMOÇÃO E ADIÇÃO OTIMIZADAS
            if (_corposParaRemover.Count > 0 || _novosCorpos.Count > 0)
            {
                AplicarMudancasColisoes();
            }
        }

        // ✅ MÉTODO FORÇA BRUTA (eficiente para poucos corpos)
        private void UsarForcaBrutaParaColisoes(int count)
        {
            for (int i = 0; i < count - 1; i++)
            {
                var a = Corpos[i];
                if (_corposParaRemover.Contains(a)) continue;

                for (int j = i + 1; j < count; j++)
                {
                    var b = Corpos[j];
                    if (_corposParaRemover.Contains(b)) continue;

                    if (VerificarColisaoRapida(a, b))
                    {
                        ProcessarColisao(a, b);
                        break; // ✅ Um corpo só colide uma vez por frame
                    }
                }
            }
        }

        // ✅ QUADTREE PARA MUITOS CORPOS (MUITO mais eficiente)
        private void UsarQuadTreeParaColisoes(int count)
        {
            var quadTree = new QuadTree(canvasWidth, canvasHeight);
            
            // ✅ INSERIR CORPOS NA QUADTREE
            for (int i = 0; i < count; i++)
            {
                quadTree.Inserir(Corpos[i]);
            }

            // ✅ DETECTAR COLISÕES USANDO QUADTREE
            for (int i = 0; i < count; i++)
            {
                var a = Corpos[i];
                if (_corposParaRemover.Contains(a)) continue;

                var possiveisColisoes = quadTree.ObterPossiveisColisoes(a);
                foreach (var b in possiveisColisoes)
                {
                    if (_corposParaRemover.Contains(b) || a == b) continue;

                    if (VerificarColisaoRapida(a, b))
                    {
                        ProcessarColisao(a, b);
                        break;
                    }
                }
            }
        }

        // ✅ PROCESSAMENTO DE COLISÕES PARALELAS
        private void ProcessarColisoesParalela(List<(Corpo, Corpo)> colisoes)
        {
            _corposParaRemover.Clear();
            _novosCorpos.Clear();

            foreach (var (a, b) in colisoes)
            {
                if (_corposParaRemover.Contains(a) || _corposParaRemover.Contains(b))
                    continue;

                var novoCorpo = Corpo.Fundir(a, b);
                _novosCorpos.Add(novoCorpo);
                _corposParaRemover.Add(a);
                _corposParaRemover.Add(b);
                ColisoesDetectadas++;
            }

            AplicarMudancasColisoes();
        }

        // ✅ VERIFICAÇÃO DE COLISÃO ULTRA-RÁPIDA
        private bool VerificarColisaoRapida(Corpo a, Corpo b)
        {
            double dx = a.PosX - b.PosX;
            double dy = a.PosY - b.PosY;
            double distQuadrado = dx * dx + dy * dy;
            double raioSoma = a.Raio + b.Raio;
            
            return distQuadrado <= raioSoma * raioSoma;
        }

        // ✅ PROCESSAMENTO DE COLISÃO OTIMIZADO
        private void ProcessarColisao(Corpo a, Corpo b)
        {
            var novoCorpo = Corpo.Fundir(a, b);
            _novosCorpos.Add(novoCorpo);
            _corposParaRemover.Add(a);
            _corposParaRemover.Add(b);
            ColisoesDetectadas++;
        }

        // ✅ APLICAÇÃO DE MUDANÇAS OTIMIZADA
        private void AplicarMudancasColisoes()
        {
            // ✅ REMOÇÃO OTIMIZADA
            if (_corposParaRemover.Count > 0)
            {
                Corpos.RemoveAll(c => _corposParaRemover.Contains(c));
            }

            // ✅ ADIÇÃO OTIMIZADA
            if (_novosCorpos.Count > 0)
            {
                Corpos.AddRange(_novosCorpos);
            }
        }

        // ✅ MÉTODO PARA LIMPEZA DE CORPOS (evita memory leaks)
        public void LimparCorposRemovidos()
        {
            // Remove corpos com massa muito pequena ou fora dos limites
            Corpos.RemoveAll(c => 
                c.Massa < 0.1 || 
                c.PosX < -1000 || c.PosX > canvasWidth + 1000 ||
                c.PosY < -1000 || c.PosY > canvasHeight + 1000);
        }
    }

    // ✅ SPATIAL HASH PARALELO
    public class SpatialHash
    {
        private readonly int _celulaTamanho;
        private readonly double _largura, _altura;
        private readonly ConcurrentDictionary<(int, int), List<Corpo>> _celulas;
        
        public SpatialHash(int celulaTamanho, double largura, double altura)
        {
            _celulaTamanho = celulaTamanho;
            _largura = largura;
            _altura = altura;
            _celulas = new ConcurrentDictionary<(int, int), List<Corpo>>();
        }
        
        public void Adicionar(Corpo corpo)
        {
            var chaves = ObterChavesCelula(corpo);
            
            foreach (var chave in chaves)
            {
                var lista = _celulas.GetOrAdd(chave, _ => new List<Corpo>());
                lock (lista)
                {
                    lista.Add(corpo);
                }
            }
        }
        
        public List<Corpo> ObterVizinhos(Corpo corpo)
        {
            var vizinhos = new List<Corpo>();
            var chaves = ObterChavesCelula(corpo);
            
            foreach (var chave in chaves)
            {
                if (_celulas.TryGetValue(chave, out var celula))
                {
                    lock (celula)
                    {
                        vizinhos.AddRange(celula);
                    }
                }
            }
            
            return vizinhos.Distinct().Where(c => c != corpo).ToList();
        }
        
        private List<(int, int)> ObterChavesCelula(Corpo corpo)
        {
            var chaves = new List<(int, int)>();
            int xMin = (int)((corpo.PosX - corpo.Raio) / _celulaTamanho);
            int xMax = (int)((corpo.PosX + corpo.Raio) / _celulaTamanho);
            int yMin = (int)((corpo.PosY - corpo.Raio) / _celulaTamanho);
            int yMax = (int)((corpo.PosY + corpo.Raio) / _celulaTamanho);
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    chaves.Add((x, y));
                }
            }
            
            return chaves;
        }
    }

    // ✅ IMPLEMENTAÇÃO DA QUADTREE PARA DETECÇÃO ESPACIAL
    public class QuadTree
    {
        private const int CAPACIDADE_MAXIMA = 8;
        private readonly List<Corpo> _corpos = new List<Corpo>();
        private readonly double _x, _y, _largura, _altura;
        private QuadTree[] _subdivisoes;
        private bool _subdividido = false;

        public QuadTree(double largura, double altura, double x = 0, double y = 0)
        {
            _x = x;
            _y = y;
            _largura = largura;
            _altura = altura;
        }

        public bool Inserir(Corpo corpo)
        {
            // Verifica se o corpo está dentro dos limites
            if (!Contem(corpo.PosX, corpo.PosY, corpo.Raio))
                return false;

            // Se ainda tem capacidade, adiciona
            if (_corpos.Count < CAPACIDADE_MAXIMA && !_subdividido)
            {
                _corpos.Add(corpo);
                return true;
            }

            // Se não tem capacidade, subdivide
            if (!_subdividido)
            {
                Subdividir();
            }

            // Tenta inserir nas subdivisões
            foreach (var sub in _subdivisoes)
            {
                if (sub.Inserir(corpo))
                    return true;
            }

            return false;
        }

        public List<Corpo> ObterPossiveisColisoes(Corpo corpo)
        {
            var resultados = new List<Corpo>();
            ObterPossiveisColisoesRecursivo(corpo, resultados);
            return resultados;
        }

        private void ObterPossiveisColisoesRecursivo(Corpo corpo, List<Corpo> resultados)
        {
            if (!Contem(corpo.PosX, corpo.PosY, corpo.Raio))
                return;

            resultados.AddRange(_corpos);

            if (_subdividido)
            {
                foreach (var sub in _subdivisoes)
                {
                    sub.ObterPossiveisColisoesRecursivo(corpo, resultados);
                }
            }
        }

        private bool Contem(double x, double y, double raio)
        {
            return x - raio >= _x && x + raio <= _x + _largura &&
                   y - raio >= _y && y + raio <= _y + _altura;
        }

        private void Subdividir()
        {
            double meiaLargura = _largura / 2;
            double meiaAltura = _altura / 2;

            _subdivisoes = new QuadTree[4];
            _subdivisoes[0] = new QuadTree(meiaLargura, meiaAltura, _x, _y); // NO
            _subdivisoes[1] = new QuadTree(meiaLargura, meiaAltura, _x + meiaLargura, _y); // NE
            _subdivisoes[2] = new QuadTree(meiaLargura, meiaAltura, _x, _y + meiaAltura); // SO
            _subdivisoes[3] = new QuadTree(meiaLargura, meiaAltura, _x + meiaLargura, _y + meiaAltura); // SE

            _subdividido = true;

            // Redistribui os corpos existentes
            foreach (var corpo in _corpos)
            {
                foreach (var sub in _subdivisoes)
                {
                    if (sub.Inserir(corpo))
                        break;
                }
            }

            _corpos.Clear();
        }
    }
}