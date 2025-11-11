using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Services;
using Microsoft.JSInterop;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        private Universo universo;
        private readonly ApiService _apiService;
        private int _ultimaQuantidadeCorpos = 0;
        private int _colisoesNaUltimaIteracao = 0;

        public List<Corpo> Corpos => universo.Corpos;
        public int Iteracoes { get; private set; }
        public int Colisoes { get; private set; }
        public bool Rodando { get; private set; }

        public double Gravidade { get; set; } = 5.0;
        public int NumCorpos { get; set; } = 8;
        public double CanvasWidth { get; set; } = 800;
        public double CanvasHeight { get; set; } = 600;

        public List<string> Eventos { get; } = new();

        // ✅ CONSTRUTOR SIMPLIFICADO - Sem arquivos
        public SimuladorService(ApiService apiService)
        {
            _apiService = apiService;
            universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            Resetar();
        }

        // ========== MÉTODOS DE BANCO DE DADOS ==========

        public async Task<bool> SalvarNoBancoAsync(string nomeSimulacao)
        {
            try
            {
                var request = new SimulacaoSalvarRequest
                {
                    Nome = nomeSimulacao,
                    Corpos = Corpos,
                    Iteracoes = Iteracoes,
                    Colisoes = Colisoes,
                    Gravidade = Gravidade
                };

                bool sucesso = await _apiService.SalvarSimulacaoAsync(request);
                
                if (sucesso)
                {
                    AdicionarEvento($"💾 SIMULAÇÃO SALVA NO BANCO: '{nomeSimulacao}'");
                    AdicionarEvento($"📊 Backup realizado: {Corpos.Count} corpos, {Iteracoes} iterações, {Colisoes} colisões");
                }
                else
                {
                    AdicionarEvento("❌ Falha ao salvar no banco de dados");
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO Banco de Dados: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CarregarDoBancoAsync(int id)
        {
            try
            {
                var simulacao = await _apiService.CarregarSimulacaoAsync(id);
                if (simulacao?.Corpos?.Count > 0)
                {
                    universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
                    Corpos.Clear();
                    
                    foreach (var corpo in simulacao.Corpos)
                    {
                        universo.AdicionarCorpo(corpo);
                    }

                    Iteracoes = simulacao.Iteracoes;
                    Colisoes = simulacao.Colisoes;
                    Gravidade = simulacao.Gravidade;
                    _ultimaQuantidadeCorpos = simulacao.Corpos.Count;
                    Rodando = false;

                    AdicionarEvento($"📂 SIMULAÇÃO CARREGADA: '{simulacao.Nome}'");
                    AdicionarEvento($"🔄 Sistema restaurado: {simulacao.Corpos.Count} corpos, {simulacao.Iteracoes} iterações");

                    return true;
                }
                else
                {
                    AdicionarEvento("❌ Simulação não encontrada ou inválida");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao carregar: {ex.Message}");
                return false;
            }
        }

        public async Task<List<SimulacaoSnapshot>> ListarSimulacoesSalvasAsync()
        {
            try
            {
                return await _apiService.GetSimulacoesAsync();
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro ao carregar lista: {ex.Message}");
                return new List<SimulacaoSnapshot>();
            }
        }

        public async Task<bool> DeletarSimulacaoAsync(int id, string nomeSimulacao = "")
        {
            try
            {
                bool sucesso = await _apiService.DeletarSimulacaoAsync(id);
                
                if (sucesso)
                {
                    AdicionarEvento($"🗑️ Simulação excluída: '{nomeSimulacao}' (ID: {id})");
                }
                else
                {
                    AdicionarEvento($"❌ Falha ao excluir simulação ID: {id}");
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro ao deletar: {ex.Message}");
                return false;
            }
        }

        // ========== MÉTODOS DA SIMULAÇÃO ==========

        public void Resetar()
        {
            universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            Corpos.Clear();

            for (int i = 0; i < NumCorpos; i++)
            {
                universo.AdicionarCorpo(Corpo.CriarDistribuido(CanvasWidth, CanvasHeight, i, NumCorpos));
            }

            Iteracoes = 0;
            Colisoes = 0;
            _ultimaQuantidadeCorpos = NumCorpos;
            _colisoesNaUltimaIteracao = 0;
            Eventos.Clear();

            AdicionarEvento($"🌌 Universo criado com {NumCorpos} corpos celestes");
            AdicionarEvento($"⚡ Configuração: Gravidade = {Gravidade}, Canvas = {CanvasWidth}x{CanvasHeight}");
        }

        public void Iniciar()
        {
            if (Corpos.Count != NumCorpos)
            {
                Resetar();
            }

            Rodando = true;
            AdicionarEvento($"🚀 SIMULAÇÃO INICIADA - {NumCorpos} corpos em movimento");
        }

        public void Parar()
        {
            Rodando = false;
            AdicionarEvento($"⏸️ SIMULAÇÃO PAUSADA - {Iteracoes} iterações, {Colisoes} colisões");
        }

        public void Atualizar(double deltaTime)
        {
            if (!Rodando) return;

            int corposAntes = Corpos.Count;
            universo.Simular(deltaTime);
            int corposAgora = Corpos.Count;

            Iteracoes++;
            Colisoes = universo.ColisoesDetectadas;

            // Eventos especiais durante a simulação
            VerificarEventosEspeciais(corposAntes, corposAgora);
        }

        private void VerificarEventosEspeciais(int corposAntes, int corposAgora)
        {
            // Colisão detectada
            if (Colisoes > _colisoesNaUltimaIteracao)
            {
                int novasColisoes = Colisoes - _colisoesNaUltimaIteracao;
                AdicionarEvento($"💥 COLISÃO DETECTADA! {novasColisoes} nova(s) fusão(ões)");
                _colisoesNaUltimaIteracao = Colisoes;
            }

            // Redução significativa de corpos
            if (corposAgora < corposAntes)
            {
                int corposFundidos = corposAntes - corposAgora;
                AdicionarEvento($"🔄 Sistema consolidado: {corposFundidos} corpos fundidos → {corposAgora} restantes");
            }

            // Milestones de iterações
            if (Iteracoes % 100 == 0)
            {
                AdicionarEvento($"🎯 Milestone: {Iteracoes} iterações completadas");
            }

            // Últimos corpos
            if (corposAgora <= 3 && corposAgora < _ultimaQuantidadeCorpos)
            {
                AdicionarEvento($"🌟 FASE FINAL: Apenas {corposAgora} corpo(s) restante(s) no sistema");
                _ultimaQuantidadeCorpos = corposAgora;
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        public void AdicionarEvento(string msg)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            Eventos.Insert(0, $"[{hora}] {msg}");

            // Manter apenas os últimos 25 eventos
            if (Eventos.Count > 25)
                Eventos.RemoveAt(Eventos.Count - 1);
        }

        public void AdicionarEventoManual(string tipo, string descricao)
        {
            var emojis = new Dictionary<string, string>
            {
                ["info"] = "ℹ️",
                ["alerta"] = "⚠️",
                ["erro"] = "❌",
                ["sucesso"] = "✅",
                ["dica"] = "💡",
                ["config"] = "⚙️",
                ["banco"] = "💾"
            };

            string emoji = emojis.ContainsKey(tipo) ? emojis[tipo] : "📝";
            AdicionarEvento($"{emoji} {descricao}");
        }

        public string ObterEstatisticas()
        {
            return $"Corpos: {Corpos.Count} | Iterações: {Iteracoes} | Colisões: {Colisoes} | Gravidade: {Gravidade}";
        }

        public void LimparEventos()
        {
            Eventos.Clear();
            AdicionarEvento("📝 Log de eventos limpo");
        }

        // ✅ MÉTODO PARA EXPORTAR DADOS (opcional, se precisar para outros usos)
        public SimulacaoSalvarRequest ObterDadosSimulacao(string nome = "")
        {
            return new SimulacaoSalvarRequest
            {
                Nome = string.IsNullOrEmpty(nome) ? $"Simulação_{DateTime.Now:yyyyMMdd_HHmmss}" : nome,
                Corpos = Corpos,
                Iteracoes = Iteracoes,
                Colisoes = Colisoes,
                Gravidade = Gravidade
            };
        }
    }
}