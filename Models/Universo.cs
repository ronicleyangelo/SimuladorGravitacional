using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Classe principal que representa o universo da simulação
    /// Gerencia todos os corpos celestes e suas interações físicas
    /// </summary>
    public class Universo
    {
        // Lista principal de corpos celestes no universo
        // Usamos List para acesso rápido por índice e manipulação eficiente
        public List<Corpo> Corpos { get; private set; } = new List<Corpo>();

        // Dimensões do canvas para verificação de limites
        // Estes valores definem os limites do espaço simulado
        private double canvasWidth;
        private double canvasHeight;

        // Fator de simulação para escalar as forças gravitacionais
        // Como as distâncias reais no espaço são muito grandes, usamos este fator
        // para tornar a simulação visualmente interessante e computacionalmente viável
        private double fatorSimulacao;

        // Estruturas temporárias para gerenciar colisões de forma eficiente
        // HashSet para remoção rápida (O(1) para verificação de existência)
        // List para adição ordenada de novos corpos
        private HashSet<Corpo> _corposParaRemover = new HashSet<Corpo>();
        private List<Corpo> _novosCorpos = new List<Corpo>();

        // Contador de colisões para métricas e debug
        // Útil para analisar o comportamento da simulação ao longo do tempo
        public int ColisoesDetectadas { get; private set; } = 0;

        // Cache para otimização de verificações de pares
        // Matriz booleana que rastreia quais pares de corpos já foram processados
        // Evita cálculos duplicados em frames consecutivos
        private bool[,] _verificados;

        // Armazena o último número de corpos para detectar mudanças na contagem
        // Se a contagem mudar, a matriz de verificação precisa ser redimensionada
        private int _ultimoCount = 0;

        /// <summary>
        /// Construtor do universo
        /// </summary>
        /// <param name="canvasWidth">Largura do espaço simulado em pixels/unidades</param>
        /// <param name="canvasHeight">Altura do espaço simulado em pixels/unidades</param>
        /// <param name="fatorSimulacao">Fator de escala para forças gravitacionais (padrão: 100,000)</param>
        public Universo(double canvasWidth, double canvasHeight, double fatorSimulacao = 1e5)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.fatorSimulacao = fatorSimulacao;
        }

        /// <summary>
        /// Adiciona um novo corpo ao universo
        /// </summary>
        /// <param name="corpo">Corpo celeste a ser adicionado</param>
        public void AdicionarCorpo(Corpo corpo)
        {
            // Adição simples à lista principal
            // Em cenários com muitos corpos, poderíamos considerar estruturas mais eficientes
            Corpos.Add(corpo);
        }

        /// <summary>
        /// Método principal de simulação com otimizações adaptativas
        /// Escolhe automaticamente a melhor estratégia baseada no número de corpos
        /// </summary>
        /// <param name="deltaTime">Tempo decorrido desde o último frame em segundos</param>
        public void Simular(double deltaTime)
        {
            // Cache do count para evitar múltiplas chamadas à propriedade Count
            int count = Corpos.Count;

            // Aplica gravidade apenas se houver mais de um corpo
            // Não faz sentido calcular gravidade com apenas um corpo
            if (count > 1)
            {
                AplicarGravidadeOtimizada(deltaTime, count);
            }

            // Escolhe estratégia de atualização baseada no número de corpos
            // Para poucos corpos, overhead do paralelismo não compensa
            // Para muitos corpos, paralelismo traz benefícios significativos
            if (count > 20)
            {
                AtualizarPosicoesParalelo(count);
            }
            else
            {
                AtualizarPosicoesSequencial(count);
            }

            // Verifica colisões apenas se houver mais de um corpo
            // Também usa estratégias diferentes baseadas no número de corpos
            if (count > 1)
            {
                TratarColisoesOtimizado(count);
            }
        }

        /// <summary>
        /// Método de simulação paralela para alto desempenho com muitos corpos
        /// Divide o trabalho em fases distintas para melhor paralelização
        /// </summary>
        /// <param name="deltaTime">Tempo decorrido desde o último frame em segundos</param>
        public void SimularParalelo(double deltaTime)
        {
            int count = Corpos.Count;

            // Saída rápida se não há corpos para processar
            if (count == 0) return;

            // Fase 1: Cálculo de forças gravitacionais em paralelo
            // Esta é a fase mais computacionalmente intensiva
            if (count > 1)
            {
                AplicarGravidadeParalela(deltaTime, count);
            }

            // Fase 2: Atualização de posições em paralelo
            // Atualiza as posições baseadas nas velocidades calculadas
            AtualizarPosicoesParalelo(count);

            // Fase 3: Detecção de colisões em paralelo
            // Usa spatial hashing para detecção eficiente
            if (count > 1)
            {
                DetectarColisoesParalela(count);
            }
        }

        /// <summary>
        /// Implementação paralela do cálculo de gravidade usando a Lei da Gravitação Universal
        /// Paraleliza o loop externo para distribuir o trabalho entre threads
        /// </summary>
        /// <param name="deltaTime">Tempo decorrido para cálculo de aceleração</param>
        /// <param name="count">Número de corpos a processar</param>
        private void AplicarGravidadeParalela(double deltaTime, int count)
        {
            // Configura opções de paralelismo para evitar sobrecarga do sistema
            // Reserva uma CPU para o sistema operacional e outras aplicações
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1)
            };

            // Paralelização por blocos para melhor aproveitamento de cache
            // Cada thread processa um subconjunto de pares de corpos
            Parallel.For(0, count - 1, options, i =>
            {
                var corpoA = Corpos[i];

                // Loop interno sequencial para cada corpo externo
                // Processa todos os pares (i, j) onde j > i para evitar duplicatas
                for (int j = i + 1; j < count; j++)
                {
                    var corpoB = Corpos[j];

                    // Cálculo do vetor distância entre corpos
                    // Diferença nas coordenadas X e Y
                    double dx = corpoB.PosX - corpoA.PosX;
                    double dy = corpoB.PosY - corpoA.PosY;

                    // Distância ao quadrado (evita cálculo de raiz quadrada inicialmente)
                    // Mais eficiente computacionalmente
                    double distanciaQuadrado = dx * dx + dy * dy;

                    // Verificação rápida de colisão para evitar cálculo desnecessário
                    // Se os corpos já estão colidindo, ignora cálculo de gravidade
                    double somaRaios = corpoA.Raio + corpoB.Raio;
                    if (distanciaQuadrado < somaRaios * somaRaios) continue;

                    // Cálculo da força gravitacional usando Lei da Gravitação Universal
                    // F = G * (m1 * m2) / r^2
                    double forca = 6.674e-11 * (corpoA.Massa * corpoB.Massa) / distanciaQuadrado * fatorSimulacao;

                    // Agora calcula a raiz quadrada para obter distância real
                    double distancia = Math.Sqrt(distanciaQuadrado);
                    double forcaPorDistancia = forca / distancia;

                    // Cálculo das acelerações para ambos os corpos usando F = m * a
                    // a = F / m para cada componente (x, y)
                    double aceleracaoAX = forcaPorDistancia * dx / corpoA.Massa;
                    double aceleracaoAY = forcaPorDistancia * dy / corpoA.Massa;
                    double aceleracaoBX = forcaPorDistancia * -dx / corpoB.Massa;
                    double aceleracaoBY = forcaPorDistancia * -dy / corpoB.Massa;

                    // Atualização direta das velocidades (thread-safe por design)
                    // Cada thread trabalha com seus próprios corpos, sem conflitos
                    // v = v0 + a * Δt
                    corpoA.VelX += aceleracaoAX * deltaTime;
                    corpoA.VelY += aceleracaoAY * deltaTime;
                    corpoB.VelX += aceleracaoBX * deltaTime;
                    corpoB.VelY += aceleracaoBY * deltaTime;
                }
            });
        }

        /// <summary>
        /// Implementação sequencial otimizada do cálculo de gravidade
        /// Usa cache para evitar recálculos desnecessários entre frames
        /// </summary>
        /// <param name="deltaTime">Tempo decorrido para cálculo de aceleração</param>
        /// <param name="count">Número de corpos a processar</param>
        private void AplicarGravidadeOtimizada(double deltaTime, int count)
        {
            // Recalcula matriz de verificação apenas se o número de corpos mudou
            // Isso evita realocação frequente da matriz
            if (count != _ultimoCount)
            {
                _verificados = new bool[count, count];
                _ultimoCount = count;
            }

            // Loop aninhado para verificar todos os pares de corpos únicos
            // Complexidade O(n²) mas com otimizações
            for (int i = 0; i < count - 1; i++)
            {
                var corpoA = Corpos[i];
                for (int j = i + 1; j < count; j++)
                {
                    // Evita cálculos duplicados usando cache entre frames
                    // Se este par já foi verificado no frame atual, pula
                    if (_verificados[i, j]) continue;

                    var corpoB = Corpos[j];

                    // Delega o cálculo de gravidade para o próprio corpo
                    // Mantém a lógica de física encapsulada na classe Corpo
                    corpoA.AplicarGravidade(corpoB, fatorSimulacao, deltaTime);

                    // Marca este par como verificado para evitar trabalho duplicado
                    _verificados[i, j] = true;
                }
            }
        }

        /// <summary>
        /// Atualização sequencial de posições (eficiente para poucos corpos)
        /// Simples e direto, sem overhead de paralelismo
        /// </summary>
        /// <param name="count">Número de corpos a atualizar</param>
        private void AtualizarPosicoesSequencial(int count)
        {
            // Loop simples por todos os corpos
            // Para poucos corpos, esta abordagem é mais eficiente
            for (int i = 0; i < count; i++)
            {
                // Cada corpo atualiza sua própria posição baseada na velocidade
                // Também trata colisões com as bordas do universo
                Corpos[i].AtualizarPosicao(canvasWidth, canvasHeight);
            }
        }

        /// <summary>
        /// Atualização paralela de posições (eficiente para muitos corpos)
        /// Divide o trabalho entre múltiplas threads
        /// </summary>
        /// <param name="count">Número de corpos a atualizar</param>
        private void AtualizarPosicoesParalelo(int count)
        {
            // Configura opções de paralelismo
            // Usa todas as CPUs disponíveis para esta fase
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            // Divide o trabalho em partições para melhor balanceamento de carga
            // Partições menores permitem melhor distribuição de trabalho
            var particoes = ParticionarCorpos(count, Environment.ProcessorCount * 2);

            // Processa cada partição em paralelo
            Parallel.ForEach(particoes, options, particao =>
            {
                // Cada thread processa sua própria partição
                for (int i = particao.Start; i < particao.End; i++)
                {
                    Corpos[i].AtualizarPosicao(canvasWidth, canvasHeight);
                }
            });
        }

        /// <summary>
        /// Cria partições para processamento paralelo balanceado
        /// Divide a lista de corpos em segmentos aproximadamente iguais
        /// </summary>
        /// <param name="total">Número total de corpos</param>
        /// <param name="numParticoes">Número desejado de partições</param>
        /// <returns>Lista de partições (start, end)</returns>
        private List<(int Start, int End)> ParticionarCorpos(int total, int numParticoes)
        {
            var particoes = new List<(int, int)>();

            // Calcula o tamanho base de cada partição
            int tamanhoParticao = total / numParticoes;

            // Cria as partições
            for (int i = 0; i < numParticoes; i++)
            {
                int inicio = i * tamanhoParticao;

                // A última partição pega todos os elementos restantes
                // Isso lida com casos onde total não é divisível por numParticoes
                int fim = (i == numParticoes - 1) ? total : inicio + tamanhoParticao;

                particoes.Add((inicio, fim));
            }

            return particoes;
        }

        /// <summary>
        /// Detecção de colisões usando abordagem paralela com spatial hashing
        /// Combina paralelismo com estrutura de dados espacial para eficiência
        /// </summary>
        /// <param name="count">Número de corpos a verificar</param>
        private void DetectarColisoesParalela(int count)
        {
            // Para poucos corpos, usa força bruta (mais simples e direto)
            // O overhead do spatial hashing não compensa para poucos elementos
            if (count < 50)
            {
                UsarForcaBrutaParaColisoes(count);
                return;
            }

            // Para muitos corpos, usa spatial hashing para otimização
            // Tamanho da célula: 100 unidades (pode ser ajustado baseado no tamanho médio dos corpos)
            var spatialHash = new SpatialHash(100, canvasWidth, canvasHeight);

            // Fase 1: Insere todos os corpos no spatial hash em paralelo
            // Cada corpo pode estar em múltiplas células se for grande o suficiente
            Parallel.For(0, count, i =>
            {
                spatialHash.Adicionar(Corpos[i]);
            });

            // Fase 2: Detecta colisões consultando o spatial hash
            // Usa ConcurrentBag para coleção thread-safe de resultados
            var colisoes = new ConcurrentBag<(Corpo, Corpo)>();

            // Verifica cada corpo contra seus vizinhos no spatial hash
            Parallel.For(0, count, i =>
            {
                var corpo = Corpos[i];

                // Obtém todos os corpos nas células vizinhas
                // Isso reduz significativamente o número de verificações
                var vizinhos = spatialHash.ObterVizinhos(corpo);

                // Verifica colisão com cada vizinho
                foreach (var vizinho in vizinhos)
                {
                    // Evita verificar colisão consigo mesmo
                    if (vizinho == corpo) continue;

                    // Verificação rápida de colisão
                    if (VerificarColisaoRapida(corpo, vizinho))
                    {
                        // Armazena o par de corpos colidindo
                        colisoes.Add((corpo, vizinho));
                    }
                }
            });

            // Fase 3: Processa colisões sequencialmente para evitar condições de corrida
            // Convertemos para lista para processamento seguro
            ProcessarColisoesParalela(colisoes.ToList());
        }

        /// <summary>
        /// Método principal para tratamento de colisões com estratégias adaptativas
        /// Escolhe automaticamente entre força bruta e QuadTree baseado no número de corpos
        /// </summary>
        /// <param name="count">Número de corpos a verificar</param>
        private void TratarColisoesOtimizado(int count)
        {
            // Limpa estruturas temporárias do frame anterior
            _corposParaRemover.Clear();
            _novosCorpos.Clear();

            // Escolhe estratégia baseada no número de corpos
            // QuadTree é mais eficiente para muitos corpos, força bruta para poucos
            if (count > 10)
            {
                UsarQuadTreeParaColisoes(count);
            }
            else
            {
                UsarForcaBrutaParaColisoes(count);
            }

            // Aplica mudanças se houver colisões para processar
            // Verificação eficiente antes de operações potencialmente custosas
            if (_corposParaRemover.Count > 0 || _novosCorpos.Count > 0)
            {
                AplicarMudancasColisoes();
            }
        }

        /// <summary>
        /// Detecção de colisões por força bruta (complexidade O(n²))
        /// Simples mas eficiente para pequeno número de corpos
        /// </summary>
        /// <param name="count">Número de corpos a verificar</param>
        private void UsarForcaBrutaParaColisoes(int count)
        {
            // Loop por todos os pares únicos de corpos
            for (int i = 0; i < count - 1; i++)
            {
                var corpoA = Corpos[i];

                // Se o corpo A já está marcado para remoção, ignora
                if (_corposParaRemover.Contains(corpoA)) continue;

                // Verifica contra todos os corpos subsequentes
                for (int j = i + 1; j < count; j++)
                {
                    var corpoB = Corpos[j];

                    // Se o corpo B já está marcado para remoção, ignora
                    if (_corposParaRemover.Contains(corpoB)) continue;

                    // Verificação rápida de colisão
                    if (VerificarColisaoRapida(corpoA, corpoB))
                    {
                        // Processa a colisão e marca para não processar novamente
                        ProcessarColisao(corpoA, corpoB);
                        break; // Um corpo só colide uma vez por frame
                    }
                }
            }
        }

        /// <summary>
        /// Detecção de colisões usando QuadTree (complexidade O(n log n) em média)
        /// Muito eficiente para grande número de corpos
        /// </summary>
        /// <param name="count">Número de corpos a verificar</param>
        private void UsarQuadTreeParaColisoes(int count)
        {
            // Cria uma nova QuadTree do tamanho do universo
            var quadTree = new QuadTree(canvasWidth, canvasHeight);

            // Fase 1: Insere todos os corpos na QuadTree
            // A QuadTree organiza os corpos espacialmente
            for (int i = 0; i < count; i++)
            {
                quadTree.Inserir(Corpos[i]);
            }

            // Fase 2: Consulta a QuadTree para possíveis colisões
            // Para cada corpo, obtém apenas os corpos potencialmente colidindo
            for (int i = 0; i < count; i++)
            {
                var corpoA = Corpos[i];

                // Ignora corpos já marcados para remoção
                if (_corposParaRemover.Contains(corpoA)) continue;

                // A QuadTree retorna apenas corpos na mesma região espacial
                // Reduz significativamente o número de verificações
                var possiveisColisoes = quadTree.ObterPossiveisColisoes(corpoA);

                // Verifica colisão com cada corpo potencial
                foreach (var corpoB in possiveisColisoes)
                {
                    // Ignora se já marcado para remoção ou se é o mesmo corpo
                    if (_corposParaRemover.Contains(corpoB) || corpoA == corpoB) continue;

                    // Verificação final de colisão
                    if (VerificarColisaoRapida(corpoA, corpoB))
                    {
                        ProcessarColisao(corpoA, corpoB);
                        break; // Processa apenas uma colisão por corpo por frame
                    }
                }
            }
        }

        /// <summary>
        /// Processa as colisões detectadas na abordagem paralela
        /// Agrupa as colisões e aplica todas as mudanças de uma vez
        /// </summary>
        /// <param name="colisoes">Lista de pares de corpos colidindo</param>
        private void ProcessarColisoesParalela(List<(Corpo, Corpo)> colisoes)
        {
            // Limpa estruturas temporárias
            _corposParaRemover.Clear();
            _novosCorpos.Clear();

            // Processa cada colisão detectada
            foreach (var (corpoA, corpoB) in colisoes)
            {
                // Verifica se algum dos corpos já foi processado em outra colisão
                if (_corposParaRemover.Contains(corpoA) || _corposParaRemover.Contains(corpoB))
                    continue;

                // Cria novo corpo resultante da fusão dos dois corpos
                // Mantém conservação de massa e momento
                var novoCorpo = Corpo.Fundir(corpoA, corpoB);
                _novosCorpos.Add(novoCorpo);

                // Marca os corpos originais para remoção
                _corposParaRemover.Add(corpoA);
                _corposParaRemover.Add(corpoB);

                // Atualiza contador de métricas
                ColisoesDetectadas++;
            }

            // Aplica todas as mudanças acumuladas
            AplicarMudancasColisoes();
        }

        /// <summary>
        /// Verificação ultra-rápida de colisão usando distância quadrada
        /// Evita cálculo de raiz quadrada comparando quadrados das distâncias
        /// </summary>
        /// <param name="a">Primeiro corpo</param>
        /// <param name="b">Segundo corpo</param>
        /// <returns>True se os corpos estão colidindo</returns>
        private bool VerificarColisaoRapida(Corpo a, Corpo b)
        {
            // Calcula diferenças nas coordenadas
            double dx = a.PosX - b.PosX;
            double dy = a.PosY - b.PosY;

            // Distância ao quadrado entre centros
            double distanciaQuadrado = dx * dx + dy * dy;

            // Soma dos raios ao quadrado
            double somaRaios = a.Raio + b.Raio;
            double somaRaiosQuadrado = somaRaios * somaRaios;

            // Colisão ocorre se distância <= soma dos raios
            // Comparação com quadrados evita cálculo de raiz quadrada
            return distanciaQuadrado <= somaRaiosQuadrado;
        }

        /// <summary>
        /// Processa uma colisão individual entre dois corpos
        /// Cria novo corpo fusionado e marca originais para remoção
        /// </summary>
        /// <param name="a">Primeiro corpo colidindo</param>
        /// <param name="b">Segundo corpo colidindo</param>
        private void ProcessarColisao(Corpo a, Corpo b)
        {
            // Fusão inelástica - os corpos se combinam em um novo corpo
            var novoCorpo = Corpo.Fundir(a, b);
            _novosCorpos.Add(novoCorpo);

            // Marca corpos originais para remoção
            _corposParaRemover.Add(a);
            _corposParaRemover.Add(b);

            // Atualiza estatísticas
            ColisoesDetectadas++;
        }

        /// <summary>
        /// Aplica as mudanças resultantes das colisões ao universo
        /// Remove corpos colididos e adiciona novos corpos fusionados
        /// </summary>
        private void AplicarMudancasColisoes()
        {
            // Remove corpos que colidiram e foram fusionados
            // Usa RemoveAll com predicate para eficiência
            if (_corposParaRemover.Count > 0)
            {
                Corpos.RemoveAll(corpo => _corposParaRemover.Contains(corpo));
            }

            // Adiciona novos corpos resultantes de fusões
            // AddRange é eficiente para adição múltipla
            if (_novosCorpos.Count > 0)
            {
                Corpos.AddRange(_novosCorpos);
            }
        }

        /// <summary>
        /// Limpeza periódica para evitar acúmulo de corpos e vazamento de memória
        /// Remove corpos muito pequenos ou que saíram dos limites do universo
        /// </summary>
        public void LimparCorposRemovidos()
        {
            // Remove corpos baseado em critérios de limpeza:
            // 1. Massa muito pequena (próximo de zero)
            // 2. Fora dos limites visíveis com margem de 1000 unidades
            Corpos.RemoveAll(corpo =>
                corpo.Massa < 0.1 ||
                corpo.PosX < -1000 || corpo.PosX > canvasWidth + 1000 ||
                corpo.PosY < -1000 || corpo.PosY > canvasHeight + 1000);
        }
    }

    /// <summary>
    /// Implementação de Spatial Hashing para detecção espacial eficiente
    /// Divide o espaço em células e armazena corpos nas células que ocupam
    /// Reduz complexidade de O(n²) para O(n) em média para detecção de colisões
    /// </summary>
    public class SpatialHash
    {
        // Tamanho de cada célula do grid em unidades do mundo
        // Valores menores: mais precisão mas mais células para verificar
        // Valores maiores: menos células mas mais verificações por célula
        private readonly int _tamanhoCelula;

        // Dimensões totais do espaço
        private readonly double _largura, _altura;

        // Dicionário thread-safe que mapeia coordenadas de célula para listas de corpos
        // ConcurrentDictionary permite acesso seguro de múltiplas threads
        private readonly ConcurrentDictionary<(int, int), List<Corpo>> _celulas;

        /// <summary>
        /// Construtor do Spatial Hash
        /// </summary>
        /// <param name="tamanhoCelula">Tamanho de cada célula do grid</param>
        /// <param name="largura">Largura total do espaço</param>
        /// <param name="altura">Altura total do espaço</param>
        public SpatialHash(int tamanhoCelula, double largura, double altura)
        {
            _tamanhoCelula = tamanhoCelula;
            _largura = largura;
            _altura = altura;
            _celulas = new ConcurrentDictionary<(int, int), List<Corpo>>();
        }

        /// <summary>
        /// Adiciona um corpo às células apropriadas do spatial hash
        /// Um corpo pode ocupar múltiplas células se for maior que uma célula
        /// </summary>
        /// <param name="corpo">Corpo a ser adicionado</param>
        public void Adicionar(Corpo corpo)
        {
            // Calcula todas as células que este corpo ocupa
            var chaves = ObterChavesCelula(corpo);

            // Adiciona o corpo a cada célula que ele toca
            foreach (var chave in chaves)
            {
                // GetOrAdd é thread-safe - cria a lista se não existir
                var lista = _celulas.GetOrAdd(chave, _ => new List<Corpo>());

                // Lock na lista específica para adição thread-safe
                // Isso permite que múltiplas threads adicionem a células diferentes simultaneamente
                lock (lista)
                {
                    lista.Add(corpo);
                }
            }
        }

        /// <summary>
        /// Obtém todos os corpos vizinhos a um determinado corpo
        /// Retorna apenas corpos nas células próximas, reduzindo verificações
        /// </summary>
        /// <param name="corpo">Corpo de referência</param>
        /// <returns>Lista de corpos potencialmente colidindo</returns>
        public List<Corpo> ObterVizinhos(Corpo corpo)
        {
            var vizinhos = new List<Corpo>();

            // Obtém células que este corpo ocupa
            var chaves = ObterChavesCelula(corpo);

            // Coleta todos os corpos de todas as células relevantes
            foreach (var chave in chaves)
            {
                if (_celulas.TryGetValue(chave, out var celula))
                {
                    // Lock na célula durante a leitura para thread-safety
                    lock (celula)
                    {
                        vizinhos.AddRange(celula);
                    }
                }
            }

            // Remove duplicatas e o próprio corpo da lista
            // Distinct() é necessário porque um corpo pode aparecer em múltiplas células
            return vizinhos.Distinct().Where(c => c != corpo).ToList();
        }

        /// <summary>
        /// Calcula as chaves das células que um corpo ocupa
        /// Considera o raio do corpo para determinar células afetadas
        /// </summary>
        /// <param name="corpo">Corpo a calcular</param>
        /// <returns>Lista de coordenadas de células ocupadas</returns>
        private List<(int, int)> ObterChavesCelula(Corpo corpo)
        {
            var chaves = new List<(int, int)>();

            // Calcula os limites do corpo em coordenadas de célula
            // Converte coordenadas do mundo para índices de célula
            int xMin = (int)((corpo.PosX - corpo.Raio) / _tamanhoCelula);
            int xMax = (int)((corpo.PosX + corpo.Raio) / _tamanhoCelula);
            int yMin = (int)((corpo.PosY - corpo.Raio) / _tamanhoCelula);
            int yMax = (int)((corpo.PosY + corpo.Raio) / _tamanhoCelula);

            // Adiciona todas as células dentro do bounding box do corpo
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

    /// <summary>
    /// Implementação de QuadTree para divisão espacial recursiva
    /// Estrutura de dados hierárquica que divide o espaço em quadrantes
    /// Muito eficiente para consultas espaciais e detecção de colisões
    /// </summary>
    public class QuadTree
    {
        // Capacidade máxima de corpos por nó antes de subdividir
        // Balance entre overhead de subdivisão e eficiência de busca
        private const int CAPACIDADE_MAXIMA = 8;

        // Lista de corpos neste nó (apenas para nós folha)
        private readonly List<Corpo> _corpos = new List<Corpo>();

        // Limites espaciais deste nó da QuadTree
        private readonly double _x, _y, _largura, _altura;

        // Subdivisões deste nó (null se não subdividido)
        private QuadTree[] _subdivisoes;

        // Flag indicando se este nó foi subdividido
        private bool _subdividido = false;

        /// <summary>
        /// Construtor da QuadTree
        /// </summary>
        /// <param name="largura">Largura do espaço deste nó</param>
        /// <param name="altura">Altura do espaço deste nó</param>
        /// <param name="x">Coordenada X do canto superior esquerdo</param>
        /// <param name="y">Coordenada Y do canto superior esquerdo</param>
        public QuadTree(double largura, double altura, double x = 0, double y = 0)
        {
            _x = x;
            _y = y;
            _largura = largura;
            _altura = altura;
        }

        /// <summary>
        /// Insere um corpo na QuadTree
        /// Recursivamente encontra o nó apropriado para o corpo
        /// </summary>
        /// <param name="corpo">Corpo a ser inserido</param>
        /// <returns>True se a inserção foi bem-sucedida</returns>
        public bool Inserir(Corpo corpo)
        {
            // Verifica se o corpo está dentro dos limites desta subdivisão
            // Considera o raio do corpo para verificação precisa
            if (!Contem(corpo.PosX, corpo.PosY, corpo.Raio))
                return false;

            // Adiciona se ainda há capacidade e não foi subdividido
            if (_corpos.Count < CAPACIDADE_MAXIMA && !_subdividido)
            {
                _corpos.Add(corpo);
                return true;
            }

            // Subdivide se necessário (atingiu capacidade máxima)
            if (!_subdividido)
            {
                Subdividir();
            }

            // Tenta inserir nas subdivisões recursivamente
            // Um corpo pode caber em múltiplas subdivisões se estiver perto dos limites
            foreach (var subdivisao in _subdivisoes)
            {
                if (subdivisao.Inserir(corpo))
                    return true;
            }

            // Se não conseguiu inserir em nenhuma subdivisão
            // Isso não deveria acontecer se Contem() retornou true
            return false;
        }

        /// <summary>
        /// Obtém possíveis colisões para um corpo
        /// Consulta recursivamente a QuadTree para corpos na mesma região
        /// </summary>
        /// <param name="corpo">Corpo de referência</param>
        /// <returns>Lista de corpos potencialmente colidindo</returns>
        public List<Corpo> ObterPossiveisColisoes(Corpo corpo)
        {
            var resultados = new List<Corpo>();
            ObterPossiveisColisoesRecursivo(corpo, resultados);
            return resultados;
        }

        /// <summary>
        /// Busca recursiva por possíveis colisões na QuadTree
        /// </summary>
        /// <param name="corpo">Corpo de referência</param>
        /// <param name="resultados">Lista acumuladora de resultados</param>
        private void ObterPossiveisColisoesRecursivo(Corpo corpo, List<Corpo> resultados)
        {
            // Se o corpo não está nesta subdivisão, retorna
            if (!Contem(corpo.PosX, corpo.PosY, corpo.Raio))
                return;

            // Adiciona todos os corpos desta subdivisão aos resultados
            resultados.AddRange(_corpos);

            // Propaga a busca para subdivisões (se existirem)
            if (_subdividido)
            {
                foreach (var subdivisao in _subdivisoes)
                {
                    subdivisao.ObterPossiveisColisoesRecursivo(corpo, resultados);
                }
            }
        }

        /// <summary>
        /// Verifica se um ponto com raio está contido nesta subdivisão
        /// </summary>
        /// <param name="x">Coordenada X do ponto</param>
        /// <param name="y">Coordenada Y do ponto</param>
        /// <param name="raio">Raio de influência</param>
        /// <returns>True se está contido</returns>
        private bool Contem(double x, double y, double raio)
        {
            // Verifica se o círculo (corpo) intersecta o retângulo (subdivisão)
            // Considera o raio para verificação precisa dos limites
            return x - raio >= _x && x + raio <= _x + _largura &&
                   y - raio >= _y && y + raio <= _y + _altura;
        }

        /// <summary>
        /// Subdivide a QuadTree em quatro partes iguais
        /// Redistribui os corpos existentes para as novas subdivisões
        /// </summary>
        private void Subdividir()
        {
            // Calcula dimensões das subdivisões
            double meiaLargura = _largura / 2;
            double meiaAltura = _altura / 2;

            // Cria as quatro subdivisões (quadrantes)
            _subdivisoes = new QuadTree[4];

            // Noroeste
            _subdivisoes[0] = new QuadTree(meiaLargura, meiaAltura, _x, _y);

            // Nordeste
            _subdivisoes[1] = new QuadTree(meiaLargura, meiaAltura, _x + meiaLargura, _y);

            // Sudoeste
            _subdivisoes[2] = new QuadTree(meiaLargura, meiaAltura, _x, _y + meiaAltura);

            // Sudeste
            _subdivisoes[3] = new QuadTree(meiaLargura, meiaAltura, _x + meiaLargura, _y + meiaAltura);

            // Marca como subdividido
            _subdividido = true;

            // Redistribui os corpos existentes para as subdivisões
            foreach (var corpo in _corpos)
            {
                bool inserido = false;

                // Tenta inserir em cada subdivisão
                foreach (var subdivisao in _subdivisoes)
                {
                    if (subdivisao.Inserir(corpo))
                    {
                        inserido = true;
                        break;
                    }
                }

                // Se não foi possível inserir em nenhuma subdivisão,
                // mantém o corpo neste nó (caso raro de corpo muito grande)
            }

            // Limpa a lista de corpos deste nó agora que eles estão nas subdivisões
            _corpos.Clear();
        }
    }
}