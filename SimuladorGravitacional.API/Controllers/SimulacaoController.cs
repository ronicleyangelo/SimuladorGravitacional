using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Data;
using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ProgramacaoAvancada.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações de gerenciamento de simulações de sistemas gravitacionais
    /// </summary>
    [ApiController]
    [Route("api/v1/simulacoes")]
    [Produces("application/json")]
    public class SimulacaoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SimulacaoController> _logger;

        public SimulacaoController(AppDbContext context, ILogger<SimulacaoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todas as simulações cadastradas no sistema
        /// </summary>
        /// <returns>Lista de simulações com seus respectivos universos e corpos celestes</returns>
        /// <response code="200">Lista de simulações recuperada com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SimulacaoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SimulacaoDto>>> ObterTodasSimulacoes()
        {
            try
            {
                _logger.LogInformation("Iniciando consulta de todas as simulações");

                var simulacoes = await _context.Simulacoes
                    .Include(s => s.Universo)
                        .ThenInclude(u => u.Corpos)
                    .OrderByDescending(s => s.DataCriacao)
                    .Select(s => new SimulacaoDto
                    {
                        Id = s.Id,
                        Nome = s.Nome,
                        DataCriacao = s.DataCriacao,
                        NumeroIteracoes = s.NumeroIteracoes,
                        NumeroColisoes = s.NumeroColisoes,
                        QuantidadeCorpos = s.QuantidadeCorpos,
                        Status = (DTOs.StatusSimulacao)s.Status,
                        Descricao = s.Descricao,
                        Universo = new UniversoDto
                        {
                            Id = s.Universo.Id,
                            Nome = s.Universo.Nome,
                            CanvasWidth = s.Universo.LarguraCanvas,
                            CanvasHeight = s.Universo.AlturaCanvas,
                            FatorSimulacao = s.Universo.FatorSimulacao,
                            Corpos = s.Universo.Corpos.Select(c => new CorpoDto
                            {
                                Id = c.Id,
                                Nome = c.Nome,
                                Massa = c.Massa,
                                Densidade = c.Densidade,
                                Raio = c.Raio,
                                Cor = c.Cor,
                                PosicaoX = c.PosicaoX,
                                PosicaoY = c.PosicaoY,
                                VelocidadeX = c.VelocidadeX,
                                VelocidadeY = c.VelocidadeY,
                                UniversoId = c.UniversoId
                            }).ToList()
                        }
                    })
                    .ToListAsync();

                _logger.LogInformation("Consulta finalizada. {Quantidade} simulações encontradas", simulacoes.Count);
                return Ok(simulacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as simulações");
                return StatusCode(500, new ProblemaDetalhes
                {
                    Titulo = "Erro interno do servidor",
                    Detalhe = "Ocorreu um erro ao processar a solicitação",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Obtém uma simulação específica pelo seu identificador único
        /// </summary>
        /// <param name="id">Identificador único da simulação</param>
        /// <returns>Dados completos da simulação solicitada</returns>
        /// <response code="200">Simulação encontrada e retornada com sucesso</response>
        /// <response code="404">Simulação não encontrada</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SimulacaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemaDetalhes), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SimulacaoDto>> ObterSimulacaoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Buscando simulação com ID: {SimulacaoId}", id);

                var simulacao = await _context.Simulacoes
                    .Include(s => s.Universo)
                        .ThenInclude(u => u.Corpos)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (simulacao == null)
                {
                    _logger.LogWarning("Simulação com ID {SimulacaoId} não encontrada", id);
                    return NotFound(new ProblemaDetalhes
                    {
                        Titulo = "Simulação não encontrada",
                        Detalhe = $"Nenhuma simulação com ID {id} foi encontrada no sistema",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var simulacaoDto = new SimulacaoDto
                {
                    Id = simulacao.Id,
                    Nome = simulacao.Nome,
                    DataCriacao = simulacao.DataCriacao,
                    NumeroIteracoes = simulacao.NumeroIteracoes,
                    NumeroColisoes = simulacao.NumeroColisoes,
                    QuantidadeCorpos = simulacao.QuantidadeCorpos,
                    Status = (DTOs.StatusSimulacao)simulacao.Status,
                    Descricao = simulacao.Descricao,
                    Universo = new UniversoDto
                    {
                        Id = simulacao.Universo.Id,
                        Nome = simulacao.Universo.Nome,
                        CanvasWidth = simulacao.Universo.LarguraCanvas,
                        CanvasHeight = simulacao.Universo.AlturaCanvas,
                        FatorSimulacao = simulacao.Universo.FatorSimulacao,
                        Corpos = simulacao.Universo.Corpos.Select(c => new CorpoDto
                        {
                            Id = c.Id,
                            Nome = c.Nome,
                            Massa = c.Massa,
                            Densidade = c.Densidade,
                            Raio = c.Raio,
                            Cor = c.Cor,
                            PosicaoX = c.PosicaoX,
                            PosicaoY = c.PosicaoY,
                            VelocidadeX = c.VelocidadeX,
                            VelocidadeY = c.VelocidadeY,
                            UniversoId = c.UniversoId
                        }).ToList()
                    }
                };

                _logger.LogInformation("Simulação com ID {SimulacaoId} encontrada com sucesso", id);
                return Ok(simulacaoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter simulação com ID: {SimulacaoId}", id);
                return StatusCode(500, new ProblemaDetalhes
                {
                    Titulo = "Erro interno do servidor",
                    Detalhe = "Ocorreu um erro ao processar a solicitação",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Cria uma nova simulação com configuração básica
        /// </summary>
        /// <param name="request">Dados para criação da simulação</param>
        /// <returns>Simulação criada com seus dados completos</returns>
        /// <response code="201">Simulação criada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(SimulacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemaDetalhes), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoDto>> CriarSimulacao(CriarSimulacaoDto request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação de nova simulação: {NomeSimulacao}", request.Nome);

                // Validação dos dados de entrada
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dados inválidos para criação de simulação");
                    return BadRequest(new ProblemaDetalhes
                    {
                        Titulo = "Dados inválidos",
                        Detalhe = "Os dados fornecidos para criação da simulação são inválidos",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Criar entidade de simulação
                var simulacao = new Simulacao(
                    nome: request.Nome.Trim(),
                    quantidadeCorpos: request.QuantidadeCorpos,
                    descricao: request.Descricao
                );

                // Criar universo associado
                var universo = new Universo(
                    nome: $"Universo - {request.Nome}",
                    larguraCanvas: request.LarguraCanvas,
                    alturaCanvas: request.AlturaCanvas,
                    fatorSimulacao: request.FatorSimulacao
                )
                {
                    Simulacao = simulacao
                };

                // Gerar corpos celestes distribuídos aleatoriamente
                var random = new Random();
                for (int i = 0; i < request.QuantidadeCorpos; i++)
                {
                    var corpo = new Corpo(
                        nome: $"Corpo Celeste {i + 1}",
                        massa: random.Next(5, 20),
                        densidade: random.NextDouble() * 4 + 1,
                        raio: random.Next(5, 15),
                        cor: $"rgb({random.Next(150, 256)}, {random.Next(150, 256)}, {random.Next(150, 256)})",
                        posicaoX: random.NextDouble() * request.LarguraCanvas,
                        posicaoY: random.NextDouble() * request.AlturaCanvas
                    )
                    {
                        Universo = universo
                    };
                    universo.Corpos.Add(corpo);
                }

                // Persistir no banco de dados
                _context.Simulacoes.Add(simulacao);
                await _context.SaveChangesAsync();

                // Registrar evento de criação
                var evento = new EventoSimulacao(
                    simulacaoId: simulacao.Id,
                    tipoEvento: "SIMULACAO_CRIADA",
                    mensagem: $"Simulação '{simulacao.Nome}' criada com {simulacao.QuantidadeCorpos} corpos celestes",
                    nivel: Models.NivelEvento.Informacao // Usando o alias
                );
                _context.EventosSimulacao.Add(evento);
                await _context.SaveChangesAsync();

                // Construir DTO de resposta
                var responseDto = await ConstruirSimulacaoDto(simulacao.Id);

                _logger.LogInformation("Simulação criada com sucesso. ID: {SimulacaoId}", simulacao.Id);
                return CreatedAtAction(nameof(ObterSimulacaoPorId), new { id = simulacao.Id }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar simulação: {NomeSimulacao}", request.Nome);
                return BadRequest(new ProblemaDetalhes
                {
                    Titulo = "Erro ao criar simulação",
                    Detalhe = "Não foi possível criar a simulação com os parâmetros fornecidos",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Cria uma simulação com corpos celestes específicos
        /// </summary>
        /// <param name="request">Dados completos da simulação incluindo corpos definidos</param>
        /// <returns>Simulação criada com os corpos especificados</returns>
        /// <response code="201">Simulação criada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos ou inconsistentes</response>
        [HttpPost("com-corpos-especificos")]
        [ProducesResponseType(typeof(SimulacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemaDetalhes), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoDto>> CriarSimulacaoComCorposEspecificos(CriarSimulacaoComCorposRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação de simulação com corpos específicos: {NomeSimulacao}", request.Nome);

                // Validações de negócio
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemaDetalhes
                    {
                        Titulo = "Dados inválidos",
                        Detalhe = "Os dados fornecidos para criação da simulação são inválidos",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (request.Corpos == null || !request.Corpos.Any())
                {
                    return BadRequest(new ProblemaDetalhes
                    {
                        Titulo = "Corpos celestes ausentes",
                        Detalhe = "É necessário especificar pelo menos um corpo celeste",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Criar entidade de simulação
                var simulacao = new Simulacao(
                    nome: request.Nome.Trim(),
                    numeroIteracoes: request.NumeroIteracoes,
                    numeroColisoes: request.NumeroColisoes,
                    quantidadeCorpos: request.Corpos.Count,
                    descricao: request.Descricao
                );

                // Criar universo com configurações especificadas
                var universo = new Universo(
                    nome: request.NomeUniverso ?? $"Universo - {request.Nome}",
                    larguraCanvas: request.LarguraCanvas,
                    alturaCanvas: request.AlturaCanvas,
                    fatorSimulacao: request.FatorSimulacao
                )
                {
                    Simulacao = simulacao
                };

                // Mapear corpos celestes do DTO para entidades
                foreach (var corpoDto in request.Corpos)
                {
                    var corpo = new Corpo(
                        corpoDto.Nome,
                        corpoDto.Massa,
                        corpoDto.Densidade,
                        corpoDto.Raio,
                        corpoDto.Cor,
                        corpoDto.PosicaoX,
                        corpoDto.PosicaoY,
                        corpoDto.VelocidadeX,
                        corpoDto.VelocidadeY
                    )
                    {
                        Universo = universo
                    };
                    universo.Corpos.Add(corpo);
                }

                // Persistir no banco de dados
                _context.Simulacoes.Add(simulacao);
                await _context.SaveChangesAsync();

                // Registrar evento
                var evento = new EventoSimulacao(
                    simulacaoId: simulacao.Id,
                    tipoEvento: "SIMULACAO_CRIADA",
                    mensagem: $"Simulação '{simulacao.Nome}' criada com {simulacao.QuantidadeCorpos} corpos celestes",
                    nivel: Models.NivelEvento.Informacao // Usando o alias
                );
                _context.EventosSimulacao.Add(evento);
                await _context.SaveChangesAsync();

                var responseDto = await ConstruirSimulacaoDto(simulacao.Id);

                _logger.LogInformation("Simulação com corpos específicos criada com sucesso. ID: {SimulacaoId}", simulacao.Id);
                return CreatedAtAction(nameof(ObterSimulacaoPorId), new { id = simulacao.Id }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar simulação com corpos específicos: {NomeSimulacao}", request.Nome);
                return BadRequest(new ProblemaDetalhes
                {
                    Titulo = "Erro ao criar simulação",
                    Detalhe = "Não foi possível criar a simulação com os corpos especificados",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Exclui permanentemente uma simulação do sistema
        /// </summary>
        /// <param name="id">Identificador único da simulação a ser removida</param>
        /// <returns>Resposta vazia indicando sucesso na operação</returns>
        /// <response code="204">Simulação removida com sucesso</response>
        /// <response code="404">Simulação não encontrada</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemaDetalhes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirSimulacao(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando exclusão da simulação ID: {SimulacaoId}", id);

                var simulacao = await _context.Simulacoes
                    .Include(s => s.Universo)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (simulacao == null)
                {
                    _logger.LogWarning("Simulação com ID {SimulacaoId} não encontrada para exclusão", id);
                    return NotFound(new ProblemaDetalhes
                    {
                        Titulo = "Simulação não encontrada",
                        Detalhe = $"Não foi possível encontrar a simulação com ID {id} para exclusão",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                _context.Simulacoes.Remove(simulacao);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Simulação com ID {SimulacaoId} excluída com sucesso", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir simulação com ID: {SimulacaoId}", id);
                return StatusCode(500, new ProblemaDetalhes
                {
                    Titulo = "Erro interno do servidor",
                    Detalhe = "Ocorreu um erro ao excluir a simulação",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Método auxiliar para construir o DTO da simulação
        /// </summary>
        private async Task<SimulacaoDto> ConstruirSimulacaoDto(int simulacaoId)
        {
            var simulacao = await _context.Simulacoes
                .Include(s => s.Universo)
                    .ThenInclude(u => u.Corpos)
                .FirstOrDefaultAsync(s => s.Id == simulacaoId);

            if (simulacao == null)
                throw new ArgumentException($"Simulação com ID {simulacaoId} não encontrada");

            return new SimulacaoDto
            {
                Id = simulacao.Id,
                Nome = simulacao.Nome,
                DataCriacao = simulacao.DataCriacao,
                NumeroIteracoes = simulacao.NumeroIteracoes,
                NumeroColisoes = simulacao.NumeroColisoes,
                QuantidadeCorpos = simulacao.QuantidadeCorpos,
                Status = (DTOs.StatusSimulacao)simulacao.Status,
                Descricao = simulacao.Descricao,
                Universo = new UniversoDto
                {
                    Id = simulacao.Universo.Id,
                    Nome = simulacao.Universo.Nome,
                    CanvasWidth = simulacao.Universo.LarguraCanvas,
                    CanvasHeight = simulacao.Universo.AlturaCanvas,
                    FatorSimulacao = simulacao.Universo.FatorSimulacao,
                    Corpos = simulacao.Universo.Corpos.Select(c => new CorpoDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Massa = c.Massa,
                        Densidade = c.Densidade,
                        Raio = c.Raio,
                        Cor = c.Cor,
                        PosicaoX = c.PosicaoX,
                        PosicaoY = c.PosicaoY,
                        VelocidadeX = c.VelocidadeX,
                        VelocidadeY = c.VelocidadeY,
                        UniversoId = c.UniversoId
                    }).ToList()
                }
            };
        }
    }

    /// <summary>
    /// DTO para criação de simulação com corpos celestes específicos
    /// </summary>
    public class CriarSimulacaoComCorposRequest
    {
        /// <summary>
        /// Nome descritivo da simulação
        /// </summary>
        /// <example>Simulação do Sistema Solar</example>
        [Required(ErrorMessage = "O nome da simulação é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Nome do universo simulado
        /// </summary>
        /// <example>Universo Solar</example>
        [StringLength(200, ErrorMessage = "O nome do universo não pode exceder 200 caracteres")]
        public string NomeUniverso { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional da simulação
        /// </summary>
        /// <example>Simulação gravitacional do nosso sistema solar</example>
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Largura do canvas de simulação em pixels
        /// </summary>
        /// <example>1200</example>
        [Range(100, 5000, ErrorMessage = "A largura do canvas deve estar entre 100 e 5000 pixels")]
        public double LarguraCanvas { get; set; } = 800;

        /// <summary>
        /// Altura do canvas de simulação em pixels
        /// </summary>
        /// <example>800</example>
        [Range(100, 5000, ErrorMessage = "A altura do canvas deve estar entre 100 e 5000 pixels")]
        public double AlturaCanvas { get; set; } = 600;

        /// <summary>
        /// Fator de escala para a simulação física
        /// </summary>
        /// <example>100000</example>
        [Range(1, 1e9, ErrorMessage = "O fator de simulação deve estar entre 1 e 1.000.000.000")]
        public double FatorSimulacao { get; set; } = 1e5;

        /// <summary>
        /// Número de iterações realizadas na simulação
        /// </summary>
        /// <example>150</example>
        [Range(0, int.MaxValue, ErrorMessage = "O número de iterações deve ser não negativo")]
        public int NumeroIteracoes { get; set; }

        /// <summary>
        /// Número de colisões detectadas durante a simulação
        /// </summary>
        /// <example>3</example>
        [Range(0, int.MaxValue, ErrorMessage = "O número de colisões deve ser não negativo")]
        public int NumeroColisoes { get; set; }

        /// <summary>
        /// Lista de corpos celestes que compõem a simulação
        /// </summary>
        [MinLength(1, ErrorMessage = "É necessário pelo menos um corpo celeste")]
        public List<CreateCorpoDto> Corpos { get; set; } = new List<CreateCorpoDto>();
    }

    /// <summary>
    /// Modelo para detalhes de problemas na API
    /// </summary>
    public class ProblemaDetalhes
    {
        public string Titulo { get; set; } = string.Empty;
        public string Detalhe { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}