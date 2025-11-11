using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProgramacaoAvancada.Data;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SimulacaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SimulacaoSnapshot>>> GetSimulacoes()
        {
            return await _context.SimulacaoSnapshots
                .OrderByDescending(s => s.DataCriacao)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SimulacaoSnapshot>> SalvarSimulacao(SimulacaoSalvarRequest request)
        {
            try
            {
                // ✅ VALIDA TODOS OS CORPOS ANTES DE SALVAR
                foreach (var corpo in request.Corpos)
                {
                    corpo.ValidarValores(); // Ou corpo.PrepararParaSerializacao();
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                var simulacao = new SimulacaoSnapshot
                {
                    Nome = request.Nome,
                    ConteudoJson = JsonSerializer.Serialize(request.Corpos, jsonOptions),
                    NumeroIteracoes = request.Iteracoes,
                    NumeroColisoes = request.Colisoes,
                    QuantidadeCorpos = request.Corpos.Count,
                    DataCriacao = DateTime.UtcNow
                };

                _context.SimulacaoSnapshots.Add(simulacao);
                await _context.SaveChangesAsync();

                return Ok(simulacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimulacaoSalvarRequest>> GetSimulacao(int id)
        {
            var simulacao = await _context.SimulacaoSnapshots.FindAsync(id);
            if (simulacao == null) return NotFound();

            try
            {
                // ✅ MESMA CONFIGURAÇÃO PARA DESSERIALIZAR
                var jsonOptions = new JsonSerializerOptions
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                };

                var corpos = JsonSerializer.Deserialize<List<Corpo>>(simulacao.ConteudoJson, jsonOptions) ?? new List<Corpo>();

                return new SimulacaoSalvarRequest
                {
                    Nome = simulacao.Nome,
                    Corpos = corpos,
                    Iteracoes = simulacao.NumeroIteracoes,
                    Colisoes = simulacao.NumeroColisoes
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Erro ao carregar simulação: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSimulacao(int id)
        {
            var simulacao = await _context.SimulacaoSnapshots.FindAsync(id);
            if (simulacao == null) return NotFound();

            _context.SimulacaoSnapshots.Remove(simulacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}