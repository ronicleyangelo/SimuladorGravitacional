// using Microsoft.EntityFrameworkCore;
// using ProgramacaoAvancada.Data;
// using ProgramacaoAvancada.Models;
// using System.Text.Json;

// namespace ProgramacaoAvancada.Services
// {
//     public class DatabaseService
//     {
//         private readonly AppDbContext _context;

//         public DatabaseService(AppDbContext context)
//         {
//             _context = context;
//         }

//         // ========== OPERAÇÕES COM SNAPSHOTS ==========

//         /// <summary>
//         /// Salva o estado atual da simulação no banco
//         /// </summary>
//         public async Task<int> SalvarSnapshotAsync(
//             string nome,
//             List<Corpo> corpos,
//             int iteracoes,
//             int colisoes)
//         {
//             var snapshot = new SimulacaoSnapshot
//             {
//                 Nome = nome,
//                 DataCriacao = DateTime.Now,
//                 NumeroIteracoes = iteracoes,
//                 NumeroColisoes = colisoes,
//                 QuantidadeCorpos = corpos.Count,
//                 ConteudoJson = JsonSerializer.Serialize(corpos, new JsonSerializerOptions 
//                 { 
//                     WriteIndented = true 
//                 })
//             };

//             _context.SimulacaoSnapshots.Add(snapshot);
//             await _context.SaveChangesAsync();

//             return snapshot.Id;
//         }

//         /// <summary>
//         /// Carrega um snapshot específico
//         /// </summary>
//         public async Task<(List<Corpo> corpos, int iteracoes, int colisoes)?> CarregarSnapshotAsync(int id)
//         {
//             var snapshot = await _context.SimulacaoSnapshots
//                 .FirstOrDefaultAsync(s => s.Id == id);

//             if (snapshot == null || string.IsNullOrEmpty(snapshot.ConteudoJson))
//                 return null;

//             try
//             {
//                 var corpos = JsonSerializer.Deserialize<List<Corpo>>(snapshot.ConteudoJson) ?? new List<Corpo>();
//                 return (corpos, snapshot.NumeroIteracoes, snapshot.NumeroColisoes);
//             }
//             catch
//             {
//                 return null;
//             }
//         }

//         /// <summary>
//         /// Lista todos os snapshots salvos
//         /// </summary>
//         public async Task<List<SimulacaoSnapshot>> ListarSnapshotsAsync()
//         {
//             return await _context.SimulacaoSnapshots
//                 .OrderByDescending(s => s.DataCriacao)
//                 .ToListAsync();
//         }

//         /// <summary>
//         /// Deleta um snapshot
//         /// </summary>
//         public async Task<bool> DeletarSnapshotAsync(int id)
//         {
//             var snapshot = await _context.SimulacaoSnapshots.FindAsync(id);
//             if (snapshot == null)
//                 return false;

//             _context.SimulacaoSnapshots.Remove(snapshot);
//             await _context.SaveChangesAsync();
//             return true;
//         }

//         // ========== ESTATÍSTICAS ==========

//         /// <summary>
//         /// Retorna estatísticas gerais
//         /// </summary>
//         public async Task<(int totalSnapshots, DateTime? ultimoSalvamento)> ObterEstatisticasAsync()
//         {
//             var total = await _context.SimulacaoSnapshots.CountAsync();
//             var ultimo = await _context.SimulacaoSnapshots
//                 .MaxAsync(s => (DateTime?)s.DataCriacao);

//             return (total, ultimo);
//         }

//         // ========== TESTE DE CONEXÃO ==========

//         /// <summary>
//         /// Testa se a conexão com o banco está funcionando
//         /// </summary>
//         public async Task<bool> TestarConexaoAsync()
//         {
//             try
//             {
//                 return await _context.Database.CanConnectAsync();
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         /// <summary>
//         /// Retorna informações sobre a conexão
//         /// </summary>
//         public string ObterInfoConexao()
//         {
//             return _context.Database.GetConnectionString() ?? "Não configurado";
//         }
//     }
// }