// using ProgramacaoAvancada.DTOs;
// using ProgramacaoAvancada.Arquivos;
// using Microsoft.JSInterop;
// using ProgramacaoAvancada.Models;

// namespace ProgramacaoAvancada.Services
// {
//     public class SimuladorService
//     {
//         private Universo universo;
//         private readonly Arquivo gerenciadorArquivo;
//         private readonly ApiService _apiService;
//         private int _ultimaQuantidadeCorpos = 0;
//         private int _colisoesNaUltimaIteracao = 0;

//         public List<Corpo> Corpos => universo.Corpos;
//         public int Iteracoes { get; private set; }
//         public int Colisoes { get; private set; }
//         public bool Rodando { get; private set; }

//         public double Gravidade { get; set; } = 5.0;
//         public int NumCorpos { get; set; } = 8;
//         public double CanvasWidth { get; set; } = 800;
//         public double CanvasHeight { get; set; } = 600;

//         public List<string> Eventos { get; } = new();

//         public SimuladorService(ApiService apiService)
//         {
//             gerenciadorArquivo = new Arquivo();
//             _apiService = apiService;
//             universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
//             Resetar();
//         }

//         // ========== MÉTODOS PRINCIPAIS DA SIMULAÇÃO ==========

//         public void Resetar()
//         {
//             universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
//             Corpos.Clear();

//             for (int i = 0; i < NumCorpos; i++)
//             {
//                 universo.AdicionarCorpo(Corpo.CriarDistribuido(CanvasWidth, CanvasHeight, i, NumCorpos));
//             }

//             Iteracoes = 0;
//             Colisoes = 0;
//             _ultimaQuantidadeCorpos = NumCorpos;
//             _colisoesNaUltimaIteracao = 0;
//             Eventos.Clear();

//             AdicionarEvento($"🌌 Universo criado com {NumCorpos} corpos celestes");
//             AdicionarEvento($"⚡ Configuração: Gravidade = {Gravidade}, Canvas = {CanvasWidth}x{CanvasHeight}");
//         }

//         public void Iniciar()
//         {
//             if (Corpos.Count != NumCorpos)
//             {
//                 Resetar();
//             }

//             Rodando = true;
//             AdicionarEvento($"🚀 SIMULAÇÃO INICIADA - {NumCorpos} corpos em movimento");
//             AdicionarEvento($"⏱️ Iteração {Iteracoes} - Sistema estabilizando...");
//         }

//         public void Parar()
//         {
//             Rodando = false;
//             AdicionarEvento($"⏸️ SIMULAÇÃO PAUSADA - {Iteracoes} iterações realizadas");
//             AdicionarEvento($"📊 Estatísticas: {Colisoes} colisões, {Corpos.Count} corpos restantes");
//         }

//         public void Atualizar(double deltaTime)
//         {
//             if (!Rodando) return;

//             int corposAntes = Corpos.Count;
//             universo.Simular(deltaTime);
//             int corposAgora = Corpos.Count;

//             Iteracoes++;
//             Colisoes = universo.ColisoesDetectadas;

//             VerificarEventosEspeciais(corposAntes, corposAgora);
//         }

//         private void VerificarEventosEspeciais(int corposAntes, int corposAgora)
//         {
//             if (Colisoes > _colisoesNaUltimaIteracao)
//             {
//                 int novasColisoes = Colisoes - _colisoesNaUltimaIteracao;
//                 AdicionarEvento($"💥 COLISÃO DETECTADA! {novasColisoes} nova(s) fusão(ões)");
//                 _colisoesNaUltimaIteracao = Colisoes;
//             }

//             if (corposAgora < corposAntes)
//             {
//                 int corposFundidos = corposAntes - corposAgora;
//                 AdicionarEvento($"🔄 Sistema consolidado: {corposFundidos} corpos fundidos → {corposAgora} restantes");
//             }

//             if (Iteracoes % 100 == 0)
//             {
//                 AdicionarEvento($"🎯 Milestone: {Iteracoes} iterações completadas");
//                 AdicionarEvento($"📈 Sistema ativo: {corposAgora} corpos, {Colisoes} colisões totais");
//             }

//             if (Iteracoes > 50 && Colisoes == _colisoesNaUltimaIteracao && Iteracoes % 50 == 0)
//             {
//                 AdicionarEvento($"⚖️ Sistema estabilizado - órbitas consistentes");
//             }

//             if (corposAgora <= 3 && corposAgora < _ultimaQuantidadeCorpos)
//             {
//                 AdicionarEvento($"🌟 FASE FINAL: Apenas {corposAgora} corpo(s) restante(s) no sistema");
//                 _ultimaQuantidadeCorpos = corposAgora;
//             }

//             if (corposAgora > 0)
//             {
//                 var maiorCorpo = Corpos.OrderByDescending(c => c.Massa).First();
//                 if (maiorCorpo.Massa > 50 && Iteracoes % 200 == 0)
//                 {
//                     AdicionarEvento($"🪐 Corpo dominante: {maiorCorpo.Nome} (Massa: {maiorCorpo.Massa:F1})");
//                 }
//             }
//         }

//         // ========== MÉTODOS DE BANCO DE DADOS ==========

//         public async Task<bool> SalvarNoBancoAsync(string nomeSimulacao)
//         {
//             try
//             {
//                 var request = new SalvarSimulacaoComCorposDto
//                 {
//                     Nome = nomeSimulacao,
//                     NomeUniverso = $"Universo - {nomeSimulacao}",
//                     CanvasWidth = CanvasWidth,
//                     CanvasHeight = CanvasHeight,
//                     FatorSimulacao = 1e10 * Gravidade,
//                     NumeroIteracoes = Iteracoes,
//                     NumeroColisoes = Colisoes,
//                     Corpos = Corpos.Select(c => new CreateCorpoDto
//                     {
//                         Nome = c.Nome,
//                         Massa = c.Massa,
//                         Densidade = c.Densidade,
//                         Raio = c.Raio,
//                         Cor = c.Cor,
//                         PosX = c.PosX,
//                         PosY = c.PosY,
//                         VelX = c.VelX,
//                         VelY = c.VelY
//                     }).ToList()
//                 };

//                 var resultado = await _apiService.SalvarSimulacaoComCorposAsync(request);
                
//                 if (resultado != null)
//                 {
//                     AdicionarEvento($"💾 SIMULAÇÃO SALVA NO BANCO: '{nomeSimulacao}' (ID: {resultado.Id})");
//                     AdicionarEvento($"📊 Backup realizado: {Corpos.Count} corpos, {Iteracoes} iterações, {Colisoes} colisões");
//                     return true;
//                 }
//                 else
//                 {
//                     AdicionarEvento("❌ Falha ao salvar no banco de dados");
//                     return false;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ ERRO Banco de Dados: {ex.Message}");
//                 return false;
//             }
//         }

//         public async Task<bool> CarregarDoBancoAsync(int id)
//         {
//             try
//             {
//                 var simulacao = await _apiService.GetSimulacaoAsync(id);
//                 if (simulacao?.Universo?.Corpos?.Count > 0)
//                 {
//                     // Atualizar configurações do universo
//                     CanvasWidth = simulacao.Universo.CanvasWidth;
//                     CanvasHeight = simulacao.Universo.CanvasHeight;
//                     Gravidade = simulacao.Universo.FatorSimulacao / 1e10;

//                     // Criar novo universo
//                     universo = new Universo(CanvasWidth, CanvasHeight, simulacao.Universo.FatorSimulacao);
//                     Corpos.Clear();

//                     // Adicionar corpos do banco
//                     foreach (var corpoDto in simulacao.Universo.Corpos)
//                     {
//                         var corpo = new Corpo(
//                             corpoDto.Nome,
//                             corpoDto.Massa,
//                             corpoDto.Densidade,
//                             corpoDto.Raio,
//                             corpoDto.Cor,
//                             corpoDto.PosX,
//                             corpoDto.PosY,
//                             corpoDto.VelX,
//                             corpoDto.VelY
//                         );
//                         universo.AdicionarCorpo(corpo);
//                     }

//                     Iteracoes = simulacao.NumeroIteracoes;
//                     Colisoes = simulacao.NumeroColisoes;
//                     _ultimaQuantidadeCorpos = simulacao.Universo.Corpos.Count;
//                     Rodando = false;

//                     AdicionarEvento($"📂 SIMULAÇÃO CARREGADA: '{simulacao.Nome}'");
//                     AdicionarEvento($"🔄 Sistema restaurado: {simulacao.Universo.Corpos.Count} corpos, {simulacao.NumeroIteracoes} iterações, {simulacao.NumeroColisoes} colisões");

//                     return true;
//                 }
//                 else
//                 {
//                     AdicionarEvento("❌ Simulação não encontrada ou inválida");
//                     return false;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ ERRO ao carregar: {ex.Message}");
//                 return false;
//             }
//         }

//         public async Task<List<SimulacaoDto>> ListarSimulacoesSalvasAsync()
//         {
//             try
//             {
//                 var simulacoes = await _apiService.GetSimulacoesAsync();
//                 AdicionarEvento($"📋 Carregadas {simulacoes.Count} simulações do banco");
//                 return simulacoes;
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ Erro ao carregar lista: {ex.Message}");
//                 return new List<SimulacaoDto>();
//             }
//         }

//         public async Task<bool> DeletarSimulacaoAsync(int id, string nomeSimulacao = "")
//         {
//             try
//             {
//                 bool sucesso = await _apiService.DeletarSimulacaoAsync(id);
                
//                 if (sucesso)
//                 {
//                     string nomeExibicao = string.IsNullOrEmpty(nomeSimulacao) ? $"ID {id}" : $"'{nomeSimulacao}'";
//                     AdicionarEvento($"🗑️ Simulação excluída: {nomeExibicao}");
//                     AdicionarEvento($"✅ Exclusão confirmada no banco de dados");
//                 }
//                 else
//                 {
//                     AdicionarEvento($"❌ Falha ao excluir simulação ID: {id}");
//                 }

//                 return sucesso;
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ Erro ao deletar: {ex.Message}");
//                 return false;
//             }
//         }

//         public async Task<bool> DeletarSimulacaoAsync(int id)
//         {
//             return await DeletarSimulacaoAsync(id, "");
//         }

//         // ========== MÉTODOS DE ARQUIVO (BACKUP) ==========

//         public async Task SalvarEmTxtAsync(IJSRuntime js)
//         {
//             try
//             {
//                 // Criar DTO temporário para exportação
//                 var simulacaoTemp = new SimulacaoDto
//                 {
//                     Nome = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}",
//                     DataCriacao = DateTime.Now,
//                     NumeroIteracoes = Iteracoes,
//                     NumeroColisoes = Colisoes,
//                     QuantidadeCorpos = Corpos.Count,
//                     Universo = new UniversoDto
//                     {
//                         Nome = "Universo Backup",
//                         CanvasWidth = CanvasWidth,
//                         CanvasHeight = CanvasHeight,
//                         FatorSimulacao = 1e10 * Gravidade,
//                         Corpos = Corpos.Select(c => new CorpoDto
//                         {
//                             Nome = c.Nome,
//                             Massa = c.Massa,
//                             Densidade = c.Densidade,
//                             Raio = c.Raio,
//                             Cor = c.Cor,
//                             PosX = c.PosX,
//                             PosY = c.PosY,
//                             VelX = c.VelX,
//                             VelY = c.VelY
//                         }).ToList()
//                     }
//                 };

//                 string conteudo = gerenciadorArquivo.ExportarCorpos(simulacaoTemp.Universo.Corpos);
//                 await js.InvokeVoidAsync("baixarArquivo", $"backup_simulacao_{DateTime.Now:yyyyMMdd_HHmmss}.txt", conteudo);

//                 AdicionarEvento($"💾 BACKUP TXT GERADO - {Corpos.Count} corpos, {Iteracoes} iterações");
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ ERRO ao gerar backup: {ex.Message}");
//             }
//         }

//         public void CarregarDeTxt(string conteudo)
//         {
//             try
//             {
//                 var corposImportados = gerenciadorArquivo.ImportarCorpos(conteudo);
//                 if (corposImportados.Count > 0)
//                 {
//                     universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
//                     Corpos.Clear();

//                     foreach (var corpoDto in corposImportados)
//                     {
//                         var corpo = new Corpo(
//                             corpoDto.Nome,
//                             corpoDto.Massa,
//                             corpoDto.Densidade,
//                             corpoDto.Raio,
//                             corpoDto.Cor,
//                             corpoDto.PosX,
//                             corpoDto.PosY,
//                             corpoDto.VelX,
//                             corpoDto.VelY
//                         );
//                         universo.AdicionarCorpo(corpo);
//                     }

//                     Iteracoes = 0;
//                     Colisoes = 0;
//                     _ultimaQuantidadeCorpos = corposImportados.Count;
//                     Rodando = false;

//                     AdicionarEvento($"📂 BACKUP CARREGADO - {corposImportados.Count} corpos");
//                     AdicionarEvento($"🔄 Sistema restaurado do arquivo local");
//                 }
//                 else
//                 {
//                     AdicionarEvento("❌ Arquivo de backup inválido ou vazio.");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 AdicionarEvento($"❌ ERRO ao carregar backup: {ex.Message}");
//             }
//         }

//         // ========== MÉTODOS AUXILIARES ==========

//         public void AdicionarEvento(string msg)
//         {
//             var hora = DateTime.Now.ToString("HH:mm:ss");
//             Eventos.Insert(0, $"[{hora}] {msg}");

//             if (Eventos.Count > 25)
//                 Eventos.RemoveAt(Eventos.Count - 1);
//         }

//         public string ObterEstatisticas()
//         {
//             return $"Corpos: {Corpos.Count} | Iterações: {Iteracoes} | Colisões: {Colisoes} | Gravidade: {Gravidade}";
//         }

//         public void LimparEventos()
//         {
//             Eventos.Clear();
//             AdicionarEvento("📝 Log de eventos limpo");
//         }

//         // Método para criar simulação rápida
//         public async Task<SimulacaoDto?> CriarSimulacaoRapida(string nome)
//         {
//             return await _apiService.CriarSimulacaoRapida(nome, NumCorpos);
//         }
//     }
// }