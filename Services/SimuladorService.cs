using Microsoft.EntityFrameworkCore; // Para Entity Framework
using Microsoft.JSInterop;
using Npgsql;
using ProgramacaoAvancada.Arquivos;
using ProgramacaoAvancada.Data; // Para SimulacaoDbContext
using ProgramacaoAvancada.Interface;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        private Universo universo;
        private readonly IArquivo<Corpo> gerenciadorArquivo;
        private readonly IJSRuntime _jsRuntime;
        private int _ultimaQuantidadeCorpos = 0;
        private int _colisoesNaUltimaIteracao = 0;
        private readonly string caminhoArquivo = "estado_simulacao.txt";

        // Suas propriedades existentes...
        public List<Corpo> Corpos => universo.Corpos;
        public int Iteracoes { get; private set; }
        public int Colisoes { get; private set; }
        public bool Rodando { get; private set; }
        public double Gravidade { get; set; } = 5.0;
        public int NumCorpos { get; set; } = 8;
        public double CanvasWidth { get; set; } = 800;
        public double CanvasHeight { get; set; } = 600;
        public List<string> Eventos { get; } = new();

        public SimuladorService(IArquivo<Corpo> gerenciadorArquivo, IJSRuntime jsRuntime)
        {
            this.gerenciadorArquivo = gerenciadorArquivo;
            _jsRuntime = jsRuntime;
            universo = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            Resetar();
        }

        // ✅ CONEXÃO COM NEON DATABASE
        private string GetConnectionString()
        {
            // ⚠️ SUBSTITUA PELA SUA CONNECTION STRING DO NEON!
            return "Host=ep-sparkling-wind-adq2d58k-pooler.c-2.us-east-1.aws.neon.tech;" +
                   "Database=neondb;" +
                   "Username=neondb_owner;" +
                   "Password=npg_DIAyl9NZd0xS;" +
                   "SSL Mode=Require;";
        }

        private SimulacaoDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SimulacaoDbContext>();
            optionsBuilder.UseNpgsql(GetConnectionString());
            return new SimulacaoDbContext(optionsBuilder.Options);
        }

        // ✅ MÉTODOS PARA NEON DATABASE
        public async Task<List<Universo>> ObterUniversosSalvosAsync()
        {
            try
            {
                using var context = CreateDbContext();
                return await context.Universos
                    .Include(u => u.CorposNavigation)
                    .OrderByDescending(u => u.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ Erro ao carregar universos: {ex.Message}");
                return new List<Universo>();
            }
        }

        public async Task<int> SalvarUniversoAtualAsync(string nome = null)
        {
            try
            {
                using var context = CreateDbContext();

                var universoDb = new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade)
                {
                    Nome = nome ?? $"Simulação {DateTime.Now:dd/MM HH:mm}",
                    Descricao = $"Iterações: {Iteracoes}, Colisões: {Colisoes}, Corpos: {Corpos.Count}",
                    DataCriacao = DateTime.Now,
                    DataUltimaModificacao = DateTime.Now,
                    ColisoesDetectadas = Colisoes
                };

                // Copiar corpos atuais
                foreach (var corpo in Corpos)
                {
                    var corpoDb = new Corpo(corpo.Nome, corpo.Massa, corpo.Densidade, corpo.PosX, corpo.PosY, corpo.Cor)
                    {
                        Raio = corpo.Raio,
                        VelX = corpo.VelX,
                        VelY = corpo.VelY,
                        Universo = universoDb
                    };
                    universoDb.CorposNavigation.Add(corpoDb);
                }

                context.Universos.Add(universoDb);
                await context.SaveChangesAsync();

                AdicionarEvento($"💾 UNIVERSO SALVO NO NEON: '{universoDb.Nome}' (ID: {universoDb.Id})");
                return universoDb.Id;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao salvar no Neon: {ex.Message}");
                return -1;
            }
        }

        public async Task<bool> CarregarUniversoAsync(int id)
        {
            try
            {
                using var context = CreateDbContext();

                var universoDb = await context.Universos
                    .Include(u => u.CorposNavigation)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (universoDb == null)
                {
                    AdicionarEvento("❌ Universo não encontrado no Neon");
                    return false;
                }

                // Configurar universo atual
                universo = new Universo(universoDb.CanvasWidth, universoDb.CanvasHeight, universoDb.FatorSimulacao);
                universo.CarregarCorpos(universoDb.CorposNavigation.ToList());

                CanvasWidth = universoDb.CanvasWidth;
                CanvasHeight = universoDb.CanvasHeight;
                Gravidade = universoDb.FatorSimulacao / 1e10;
                Colisoes = universoDb.ColisoesDetectadas;
                _ultimaQuantidadeCorpos = Corpos.Count;

                AdicionarEvento($"📂 UNIVERSO CARREGADO: '{universoDb.Nome}'");
                return true;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao carregar do Neon: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletarUniversoAsync(int id)
        {
            try
            {
                using var context = CreateDbContext();

                var universoDb = await context.Universos.FindAsync(id);
                if (universoDb == null) return false;

                context.Universos.Remove(universoDb);
                await context.SaveChangesAsync();

                AdicionarEvento($"🗑️ UNIVERSO DELETADO: ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao deletar do Neon: {ex.Message}");
                return false;
            }
        }

        // ✅ MÉTODOS QUE ESTAVAM FALTANDO:

        public async Task<bool> AtualizarUniversoAtualAsync(int id)
        {
            try
            {
                using var context = CreateDbContext();

                var universoDb = await context.Universos
                    .Include(u => u.CorposNavigation)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (universoDb == null) return false;

                // Atualizar propriedades
                universoDb.Nome = $"Simulação Atualizada {DateTime.Now:HH:mm}";
                universoDb.Descricao = $"Iterações: {Iteracoes}, Colisões: {Colisoes}, Corpos: {Corpos.Count}";
                universoDb.DataUltimaModificacao = DateTime.Now;
                universoDb.ColisoesDetectadas = Colisoes;
                universoDb.CanvasWidth = CanvasWidth;
                universoDb.CanvasHeight = CanvasHeight;
                universoDb.FatorSimulacao = 1e10 * Gravidade;

                // Remover corpos antigos
                context.Corpos.RemoveRange(universoDb.CorposNavigation);

                // Adicionar novos corpos
                foreach (var corpo in Corpos)
                {
                    var corpoDb = new Corpo(corpo.Nome, corpo.Massa, corpo.Densidade, corpo.PosX, corpo.PosY, corpo.Cor)
                    {
                        Raio = corpo.Raio,
                        VelX = corpo.VelX,
                        VelY = corpo.VelY,
                        UniversoId = universoDb.Id
                    };
                    context.Corpos.Add(corpoDb);
                }

                await context.SaveChangesAsync();

                AdicionarEvento($"✏️ UNIVERSO ATUALIZADO: ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao atualizar no banco: {ex.Message}");
                return false;
            }
        }

        public async Task<Universo> CriarNovoUniversoNoBancoAsync(string nome, int numeroCorpos)
        {
            try
            {
                using var context = CreateDbContext();

                var novoUniverso = Universo.CriarUniversoAleatorio(CanvasWidth, CanvasHeight, numeroCorpos, 1e10 * Gravidade);
                novoUniverso.Nome = nome;
                novoUniverso.DataCriacao = DateTime.Now;
                novoUniverso.DataUltimaModificacao = DateTime.Now;

                context.Universos.Add(novoUniverso);
                await context.SaveChangesAsync();

                AdicionarEvento($"🌟 NOVO UNIVERSO CRIADO: '{nome}' (ID: {novoUniverso.Id})");
                return novoUniverso;
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao criar universo: {ex.Message}");
                return new Universo(CanvasWidth, CanvasHeight, 1e10 * Gravidade);
            }
        }

        public async Task<bool> TestarConexaoNeonAsync()
        {
            try
            {
                using var context = CreateDbContext();
                var podeConectar = await context.Database.CanConnectAsync();

                if (podeConectar)
                {
                    var count = await context.Universos.CountAsync();
                    AdicionarEvento($"✅ CONEXÃO NEON OK! {count} universos no banco");
                    return true;
                }
                else
                {
                    AdicionarEvento("❌ Não foi possível conectar ao Neon");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ FALHA NA CONEXÃO: {ex.Message}");
                return false;
            }
        }

        // ✅ MÉTODOS DE SIMULAÇÃO ORIGINAIS:

        public void Iniciar()
        {
            if (Corpos.Count != NumCorpos)
            {
                Resetar();
            }

            Rodando = true;
            AdicionarEvento($"🚀 SIMULAÇÃO INICIADA - {NumCorpos} corpos");
        }

        public void Parar()
        {
            Rodando = false;
            AdicionarEvento($"⏸️ SIMULAÇÃO PAUSADA - {Iteracoes} iterações");
        }

        public void Atualizar(double deltaTime)
        {
            if (!Rodando) return;

            int corposAntes = Corpos.Count;
            universo.Simular(deltaTime);
            int corposAgora = Corpos.Count;

            Iteracoes++;
            Colisoes = universo.ColisoesDetectadas;

            // ✅ AGORA USA A VARIÁVEL _colisoesNaUltimaIteracao
            VerificarEventosEspeciais(corposAntes, corposAgora);
        }

        private void VerificarEventosEspeciais(int corposAntes, int corposAgora)
        {
            // ✅ EVENTO: Colisão detectada (AGORA USA A VARIÁVEL)
            if (Colisoes > _colisoesNaUltimaIteracao)
            {
                int novasColisoes = Colisoes - _colisoesNaUltimaIteracao;
                AdicionarEvento($"💥 COLISÃO DETECTADA! {novasColisoes} nova(s) fusão(ões)");
                _colisoesNaUltimaIteracao = Colisoes; // ✅ AGORA ESTÁ SENDO USADA
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

            AdicionarEvento($"🌌 Universo criado com {NumCorpos} corpos celestes");
        }

        // ✅ MÉTODOS DE ARQUIVO TXT:

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

                        AdicionarEvento($"📂 ARQUIVO CARREGADO - {lista.Count} corpos, {iter} iterações");
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
                        AdicionarEvento($"📂 Arquivo carregado com {lista.Count} corpos");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao carregar arquivo: {ex.Message}");
            }
        }

        public async Task SalvarEmTxtAsync(IJSRuntime js)
        {
            try
            {
                string conteudo = GerarConteudoArquivo(Corpos, Iteracoes, 0.016);
                await js.InvokeVoidAsync("baixarArquivo", "estado_simulacao.txt", conteudo);
                AdicionarEvento($"💾 ARQUIVO SALVO - {Corpos.Count} corpos, {Iteracoes} iterações");
            }
            catch (Exception ex)
            {
                AdicionarEvento($"❌ ERRO ao salvar: {ex.Message}");
            }
        }

        public void AdicionarEvento(string msg)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            Eventos.Insert(0, $"[{hora}] {msg}");
            if (Eventos.Count > 25)
                Eventos.RemoveAt(Eventos.Count - 1);
        }
    }
}