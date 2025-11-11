using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Interface;
using ProgramacaoAvancada.Arquivos;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        private Universo universo;
        private readonly IArquivo<Corpo> gerenciadorArquivo;
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

        // ✅ REMOVIDO: private readonly string caminhoArquivo = "estado_simulacao.txt";

        public SimuladorService(ApiService apiService)
        {
            gerenciadorArquivo = new Arquivo();
            _apiService = apiService;
            universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            Resetar();
        }

        // ========== MÉTODOS PRINCIPAIS DA SIMULAÇÃO ==========

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

            // ✅ EVENTO: Simulação reiniciada
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

            // ✅ EVENTO: Simulação iniciada
            AdicionarEvento($"🚀 SIMULAÇÃO INICIADA - {NumCorpos} corpos em movimento");
            AdicionarEvento($"⏱️ Iteração {Iteracoes} - Sistema estabilizando...");
        }

        public void Parar()
        {
            Rodando = false;

            // ✅ EVENTO: Simulação pausada com estatísticas
            AdicionarEvento($"⏸️ SIMULAÇÃO PAUSADA - {Iteracoes} iterações realizadas");
            AdicionarEvento($"📊 Estatísticas: {Colisoes} colisões, {Corpos.Count} corpos restantes");
        }

        public void Atualizar(double deltaTime)
        {
            if (!Rodando) return;

            int corposAntes = Corpos.Count;
            universo.Simular(deltaTime);
            int corposAgora = Corpos.Count;

            Iteracoes++;
            Colisoes = universo.ColisoesDetectadas;

            // ✅ EVENTOS DURANTE A SIMULAÇÃO
            VerificarEventosEspeciais(corposAntes, corposAgora);
        }

        private void VerificarEventosEspeciais(int corposAntes, int corposAgora)
        {
            // ✅ EVENTO: Colisão detectada
            if (Colisoes > _colisoesNaUltimaIteracao)
            {
                int novasColisoes = Colisoes - _colisoesNaUltimaIteracao;
                AdicionarEvento($"💥 COLISÃO DETECTADA! {novasColisoes} nova(s) fusão(ões)");
                _colisoesNaUltimaIteracao = Colisoes;
            }

            // ✅ EVENTO: Redução significativa de corpos
            if (corposAgora < corposAntes)
            {
                int corposFundidos = corposAntes - corposAgora;
                AdicionarEvento($"🔄 Sistema consolidado: {corposFundidos} corpos fundidos → {corposAgora} restantes");
            }

            // ✅ EVENTO: Milestones de iterações
            if (Iteracoes % 100 == 0)
            {
                AdicionarEvento($"🎯 Milestone: {Iteracoes} iterações completadas");
                AdicionarEvento($"📈 Sistema ativo: {corposAgora} corpos, {Colisoes} colisões totais");
            }

            // ✅ EVENTO: Sistema estabilizando (poucas colisões)
            if (Iteracoes > 50 && Colisoes == _colisoesNaUltimaIteracao && Iteracoes % 50 == 0)
            {
                AdicionarEvento($"⚖️ Sistema estabilizado - órbitas consistentes");
            }

            // ✅ EVENTO: Últimos corpos
            if (corposAgora <= 3 && corposAgora < _ultimaQuantidadeCorpos)
            {
                AdicionarEvento($"🌟 FASE FINAL: Apenas {corposAgora} corpo(s) restante(s) no sistema");
                _ultimaQuantidadeCorpos = corposAgora;
            }

            // ✅ EVENTO: Corpo dominante
            if (corposAgora > 0)
            {
                var maiorCorpo = Corpos.OrderByDescending(c => c.Massa).First();
                if (maiorCorpo.Massa > 50 && Iteracoes % 200 == 0)
                {
                    AdicionarEvento($"🪐 Corpo dominante: {maiorCorpo.Nome} (Massa: {maiorCorpo.Massa:F1})");
                }
            }
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
                    AdicionarEvento($"🔄 Sistema restaurado: {simulacao.Corpos.Count} corpos, {simulacao.Iteracoes} iterações, {simulacao.Colisoes} colisões");

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
                var simulacoes = await _apiService.GetSimulacoesAsync();
                AdicionarEvento($"📋 Carregadas {simulacoes.Count} simulações do banco");
                return simulacoes;
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
                    string nomeExibicao = string.IsNullOrEmpty(nomeSimulacao) ? $"ID {id}" : $"'{nomeSimulacao}'";
                    AdicionarEvento($"🗑️ Simulação excluída: {nomeExibicao}");
                    AdicionarEvento($"✅ Exclusão confirmada no banco de dados");
                }
                else
                {
                    AdicionarEvento($"❌ Falha ao excluir simulação ID: {id}");
                    AdicionarEvento($"⚠️ A simulação pode não existir ou estar bloqueada");
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro ao deletar: {ex.Message}");
                AdicionarEvento($"🔧 Verifique a conexão com o banco de dados");
                return false;
            }
        }

        public async Task<bool> DeletarSimulacaoAsync(int id)
        {
            return await DeletarSimulacaoAsync(id, "");
        }

        // ========== MÉTODOS DE ARQUIVO (BACKUP) ==========

        public async Task SalvarEmTxtAsync(IJSRuntime js)
        {
            try
            {
                string conteudo = GerarConteudoArquivo(Corpos, Iteracoes, 0.016);
                await js.InvokeVoidAsync("baixarArquivo", $"backup_simulacao_{DateTime.Now:yyyyMMdd_HHmmss}.txt", conteudo);

                AdicionarEvento($"💾 BACKUP TXT GERADO - {Corpos.Count} corpos, {Iteracoes} iterações");
                AdicionarEvento($"📁 Arquivo salvo localmente ({conteudo.Length} bytes)");
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao gerar backup: {ex.Message}");
            }
        }

        public void CarregarDeTxt(string conteudo)
        {
            try
            {
                if (gerenciadorArquivo is Arquivo arquivoConcreto)
                {
                    var (lista, iter, tempo) = arquivoConcreto.CarregarDeConteudo(conteudo);
                    if (lista.Count > 0)
                    {
                        universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
                        foreach (var c in lista)
                            universo.AdicionarCorpo(c);

                        Iteracoes = iter;
                        Colisoes = 0;
                        _ultimaQuantidadeCorpos = lista.Count;
                        Rodando = false;

                        AdicionarEvento($"📂 BACKUP CARREGADO - {lista.Count} corpos, {iter} iterações anteriores");
                        AdicionarEvento($"🔄 Sistema restaurado do arquivo local");
                    }
                    else
                    {
                        AdicionarEvento("❌ Arquivo de backup inválido ou vazio.");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao carregar backup: {ex.Message}");
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        public string GerarConteudoArquivo(List<Corpo> corpos, int iteracoes, double deltaTime)
        {
            if (gerenciadorArquivo is Arquivo arquivoConcreto)
            {
                return arquivoConcreto.GerarConteudoArquivo(
                    corpos,
                    $"Simulação Gravidade 2D - Iterações: {iteracoes}",
                    iteracoes,
                    deltaTime
                );
            }
            else
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"{corpos.Count};{iteracoes};{deltaTime}");
                foreach (var c in corpos)
                {
                    sb.AppendLine($"{c.Nome};{c.Massa};{c.Raio};{c.PosX};{c.PosY};{c.VelX};{c.VelY}");
                }
                return sb.ToString();
            }
        }

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

        public void AdicionarEvento(string msg)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            Eventos.Insert(0, $"[{hora}] {msg}");

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
                ["banco"] = "💾",
                ["backup"] = "📁"
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
    }
}