using System.Net.Http.Json;
using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.DTOs;

namespace ProgramacaoAvancada.Services
{
    /// <summary>
    /// Serviço especializado para comunicação com a API de simulações gravitacionais
    /// Gerencia operações CRUD (Create, Read, Delete) para simulações salvas
    /// Implementa padrão de resilência com tratamento de erros robusto
    /// </summary>
    public class SimulacaoApiService
    {
        // Cliente HTTP configurado para comunicação com a API
        private readonly HttpClient _http;

        // URL base da API - ajuste conforme sua configuração local
        // ⚠️ IMPORTANTE: Verifique a porta do seu servidor backend
        private const string BaseUrl = "http://localhost:5001/api/simulacoes";

        /// <summary>
        /// Construtor que injeta o HttpClient (configurado no Program.cs)
        /// Seguindo as melhores práticas de injeção de dependência do .NET
        /// </summary>
        /// <param name="http">Cliente HTTP pré-configurado</param>
        public SimulacaoApiService(HttpClient http)
        {
            _http = http;
        }

        // ========== OPERAÇÕES PRINCIPAIS DA API ==========

        /// <summary>
        /// Salva uma simulação completa no banco de dados via API
        /// Inclui todos os corpos, configurações e estado atual
        /// </summary>
        /// <param name="request">DTO contendo todos os dados da simulação</param>
        /// <returns>Simulação salva com ID gerado ou null em caso de erro</returns>
        /// <remarks>
        /// FLUXO DE SALVAMENTO:
        /// 1. Serializa a simulação para JSON
        /// 2. Envia POST para /api/simulacoes
        /// 3. API valida e persiste no banco
        /// 4. Retorna simulação com ID único
        /// </remarks>
        public async Task<SimulacaoSalva?> SalvarSimulacaoAsync(SalvarSimulacaoRequest request)
        {
            try
            {
                // ENVIO DA REQUISIÇÃO POST
                // Serializa automaticamente o objeto para JSON
                var response = await _http.PostAsJsonAsync(BaseUrl, request);

                // Verifica se a resposta é bem-sucedida (status 2xx)
                // Lança exceção se for status 4xx ou 5xx
                response.EnsureSuccessStatusCode();

                // Desserializa a resposta JSON para objeto SimulacaoSalva
                return await response.Content.ReadFromJsonAsync<SimulacaoSalva>();
            }
            catch (Exception ex)
            {
                // TRATAMENTO ROBUSTO DE ERROS:
                // - Conexão recusada (servidor offline)
                // - Timeout (servidor lento)
                // - Erro de serialização (dados inválidos)
                // - Erro HTTP (4xx, 5xx)
                Console.WriteLine($"❌ Erro ao salvar simulação: {ex.Message}");
                return null; // Retorno seguro em caso de falha
            }
        }

        /// <summary>
        /// Busca uma simulação específica pelo ID único
        /// Recupera simulação completa com todos os corpos e configurações
        /// </summary>
        /// <param name="id">ID único da simulação no banco</param>
        /// <returns>Simulação completa ou null se não encontrada</returns>
        /// <remarks>
        /// USO TÍPICO:
        /// - Carregar simulação salva para continuar execução
        /// - Visualizar simulações anteriores
        /// - Recuperar estado após reinicialização
        /// </remarks>
        public async Task<SimulacaoSalva?> GetSimulacaoPorIdAsync(int id)
        {
            try
            {
                // REQUISIÇÃO GET SIMPLES
                // Constrói URL: /api/simulacoes/{id}
                // Desserializa automaticamente JSON para objeto
                return await _http.GetFromJsonAsync<SimulacaoSalva>($"{BaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                // CENÁRIOS DE ERRO:
                // - ID não existe (404 Not Found)
                // - Formato JSON inválido
                // - Problemas de conexão
                Console.WriteLine($"❌ Erro ao buscar simulação {id}: {ex.Message}");
                return null; // Retorno seguro - simulação não encontrada
            }
        }

        /// <summary>
        /// Lista as simulações mais recentes salvas no sistema
        /// Ordenadas por data de criação descendente
        /// </summary>
        /// <param name="limit">Número máximo de simulações a retornar (padrão: 10)</param>
        /// <returns>Lista de simulações ou lista vazia em caso de erro</returns>
        /// <remarks>
        /// OTIMIZAÇÕES:
        /// - Paginação implícita para muitas simulações
        /// - Retorna apenas metadados, não corpos (performance)
        /// - Ordenação por data para UX intuitiva
        /// </remarks>
        public async Task<List<SimulacaoSalva>?> GetSimulacoesRecentesAsync(int limit = 10)
        {
            try
            {
                // ENDPOINT ESPECIALIZADO PARA LISTAGEM
                // URL: /api/simulacoes/recentes/{limit}
                // Otimizado para retornar apenas dados de listagem
                return await _http.GetFromJsonAsync<List<SimulacaoSalva>>($"{BaseUrl}/recentes/{limit}");
            }
            catch (Exception ex)
            {
                // RESILIÊNCIA: Retorna lista vazia em vez de null
                // Evita NullReferenceException no código cliente
                Console.WriteLine($"❌ Erro ao listar simulações: {ex.Message}");
                return new List<SimulacaoSalva>(); // Lista vazia segura
            }
        }

        /// <summary>
        /// Remove permanentemente uma simulação do banco de dados
        /// Operação irreversível - deleta simulação e todos seus corpos
        /// </summary>
        /// <param name="id">ID único da simulação a ser deletada</param>
        /// <returns>True se sucesso, False se falha</returns>
        /// <remarks>
        /// SEGURANÇA:
        /// - Confirmação deve ser feita no cliente antes de chamar este método
        /// - Não há lixeira/recuperação - exclusão permanente
        /// - Permissões devem ser validadas no backend
        /// </remarks>
        public async Task<bool> DeletarSimulacaoAsync(int id)
        {
            try
            {
                // REQUISIÇÃO DELETE SIMPLES
                // URL: /api/simulacoes/{id}
                // Método HTTP DELETE semanticamente correto
                var response = await _http.DeleteAsync($"{BaseUrl}/{id}");

                // Retorna status booleano baseado no código HTTP
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // ERROS COMUNS:
                // - Simulação já deletada
                // - Permissões insuficientes
                // - Problemas de rede
                Console.WriteLine($"❌ Erro ao deletar simulação {id}: {ex.Message}");
                return false; // Indica falha na operação
            }
        }
    }

    // ========== DATA TRANSFER OBJECTS (DTOs) ==========

    /// <summary>
    /// DTO para envio de dados ao salvar uma simulação
    /// Otimizado para serialização JSON e comunicação com API
    /// Contém estado completo da simulação + metadados
    /// </summary>
    public class SalvarSimulacaoRequest
    {
        /// <summary>Nome amigável para identificação da simulação</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Número atual de corpos na simulação</summary>
        public int NumeroCorpos { get; set; }

        /// <summary>Contador de iterações executadas</summary>
        public int Iteracoes { get; set; }

        /// <summary>Total de colisões/fusões ocorridas</summary>
        public int Colisoes { get; set; }

        /// <summary>Configuração de gravidade ativa</summary>
        public double Gravidade { get; set; }

        /// <summary>Dimensões do espaço simulado</summary>
        public double CanvasWidth { get; set; }

        /// <summary>Dimensões do espaço simulado</summary>
        public double CanvasHeight { get; set; }

        /// <summary>Lista de todos os corpos celestes atuais</summary>
        public List<CorpoDto> Corpos { get; set; } = new();
    }
}