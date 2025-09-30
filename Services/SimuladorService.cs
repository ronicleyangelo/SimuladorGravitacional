using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Interface;
using ProgramacaoAvancada.Arquivos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        private Universo universo;
        private readonly IArquivo<Corpo> gerenciadorArquivo;
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

        private readonly string caminhoArquivo = "estado_simulacao.txt";

        public SimuladorService(IArquivo<Corpo> arquivo = null)
        {
            gerenciadorArquivo = arquivo ?? new Arquivo();
            universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            Resetar();
        }

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

        public void AdicionarEvento(string msg)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            Eventos.Insert(0, $"[{hora}] {msg}");

            // Manter apenas os últimos 25 eventos (aumentado para mais histórico)
            if (Eventos.Count > 25)
                Eventos.RemoveAt(Eventos.Count - 1);
        }

        // ✅ MÉTODO CORRIGIDO: Compatível com a interface IArquivo
        public async Task SalvarEmTxtAsync(IJSRuntime js)
        {
            try
            {
                if (gerenciadorArquivo is Arquivo arquivoConcreto)
                {
                    string conteudo = arquivoConcreto.GerarConteudoArquivo(
                        Corpos,
                        $"Simulação Gravidade 2D - Iterações: {Iteracoes}",
                        Iteracoes,
                        0.016
                    );
                    await js.InvokeVoidAsync("baixarArquivo", "estado_simulacao.txt", conteudo);

                    // ✅ EVENTO: Salvamento com estatísticas
                    AdicionarEvento($"💾 ESTADO SALVO - {Corpos.Count} corpos, {Iteracoes} iterações");
                    AdicionarEvento($"📁 Arquivo: estado_simulacao.txt ({conteudo.Length} bytes)");
                }
                else
                {
                    gerenciadorArquivo.Salvar(caminhoArquivo, Corpos, Iteracoes, 0.016);
                    AdicionarEvento("💾 Arquivo TXT salvo no servidor.");
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao salvar: {ex.Message}");
            }
        }

        // ✅ NOVO MÉTODO: Para uso direto na página Blazor
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

                        // ✅ EVENTO: Carregamento bem-sucedido
                        AdicionarEvento($"📂 UNIVERSO CARREGADO - {lista.Count} corpos, {iter} iterações anteriores");
                        AdicionarEvento($"🔄 Sistema restaurado - pronto para continuar");
                    }
                    else
                    {
                        AdicionarEvento("❌ Arquivo inválido ou vazio.");
                    }
                }
                else
                {
                    var (lista, iter, tempo) = gerenciadorArquivo.Carregar(caminhoArquivo);
                    if (lista.Count > 0)
                    {
                        universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
                        foreach (var c in lista)
                            universo.AdicionarCorpo(c);

                        Iteracoes = iter;
                        AdicionarEvento($"📂 Arquivo carregado com {lista.Count} corpos (modo servidor).");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao carregar arquivo: {ex.Message}");
            }
        }

        // ✅ NOVO MÉTODO: Para eventos manuais (pode ser chamado da UI)
        public void AdicionarEventoManual(string tipo, string descricao)
        {
            var emojis = new Dictionary<string, string>
            {
                ["info"] = "ℹ️",
                ["alerta"] = "⚠️",
                ["erro"] = "❌",
                ["sucesso"] = "✅",
                ["dica"] = "💡",
                ["config"] = "⚙️"
            };

            string emoji = emojis.ContainsKey(tipo) ? emojis[tipo] : "📝";
            AdicionarEvento($"{emoji} {descricao}");
        }
    }
}
