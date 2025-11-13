using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProgramacaoAvancada.DTOs
{
    /// <summary>
    /// DTO para representação completa de uma simulação
    /// </summary>
    public class SimulacaoDto
    {
        /// <summary>
        /// Identificador único da simulação
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da simulação
        /// </summary>
        /// <example>Simulação do Sistema Solar</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de criação da simulação
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data e hora da última atualização da simulação
        /// </summary>
        /// <example>2024-01-15T11:45:00Z</example>
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Número total de iterações realizadas
        /// </summary>
        /// <example>150</example>
        public int NumeroIteracoes { get; set; }

        /// <summary>
        /// Número total de colisões detectadas
        /// </summary>
        /// <example>3</example>
        public int NumeroColisoes { get; set; }

        /// <summary>
        /// Quantidade de corpos celestes na simulação
        /// </summary>
        /// <example>8</example>
        public int QuantidadeCorpos { get; set; }

        /// <summary>
        /// Status atual da simulação
        /// </summary>
        /// <example>Executando</example>
        public StatusSimulacao Status { get; set; }

        /// <summary>
        /// Descrição opcional da simulação
        /// </summary>
        /// <example>Simulação gravitacional do sistema solar interno</example>
        public string? Descricao { get; set; }

        /// <summary>
        /// Universo associado à simulação
        /// </summary>
        public UniversoDto Universo { get; set; } = null!;

        /// <summary>
        /// Resumo estatístico da simulação
        /// </summary>
        public EstatisticasSimulacaoDto Estatisticas { get; set; } = new EstatisticasSimulacaoDto();
    }

    /// <summary>
    /// DTO para criação de uma nova simulação
    /// </summary>
    public class CriarSimulacaoDto
    {
        /// <summary>
        /// Nome descritivo da simulação
        /// </summary>
        /// <example>Minha Simulação Gravitacional</example>
        [Required(ErrorMessage = "O nome da simulação é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional da simulação
        /// </summary>
        /// <example>Simulação personalizada com parâmetros específicos</example>
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Largura do canvas de simulação em pixels
        /// </summary>
        /// <example>1200</example>
        [Required(ErrorMessage = "A largura do canvas é obrigatória")]
        [Range(100, 10000, ErrorMessage = "A largura deve estar entre 100 e 10.000 pixels")]
        public double LarguraCanvas { get; set; } = 800;

        /// <summary>
        /// Altura do canvas de simulação em pixels
        /// </summary>
        /// <example>800</example>
        [Required(ErrorMessage = "A altura do canvas é obrigatória")]
        [Range(100, 10000, ErrorMessage = "A altura deve estar entre 100 e 10.000 pixels")]
        public double AlturaCanvas { get; set; } = 600;

        /// <summary>
        /// Fator de escala para a simulação física
        /// </summary>
        /// <example>100000</example>
        [Required(ErrorMessage = "O fator de simulação é obrigatório")]
        [Range(1, 1e9, ErrorMessage = "O fator de simulação deve estar entre 1 e 1.000.000.000")]
        public double FatorSimulacao { get; set; } = 1e5;

        /// <summary>
        /// Quantidade de corpos celestes a serem gerados
        /// </summary>
        /// <example>10</example>
        [Required(ErrorMessage = "A quantidade de corpos é obrigatória")]
        [Range(1, 1000, ErrorMessage = "A quantidade de corpos deve estar entre 1 e 1000")]
        public int QuantidadeCorpos { get; set; }

        /// <summary>
        /// Constante gravitacional personalizada
        /// </summary>
        /// <example>6.67430e-11</example>
        [Range(1e-11, 1e-9, ErrorMessage = "A constante gravitacional deve estar entre 1e-11 e 1e-9")]
        public double ConstanteGravitacional { get; set; } = 6.67430e-11;

        /// <summary>
        /// Passo de tempo para cada iteração (em segundos)
        /// </summary>
        /// <example>3600</example>
        [Range(0.001, 86400, ErrorMessage = "O passo de tempo deve estar entre 0.001 e 86400 segundos")]
        public double PassoTempo { get; set; } = 1.0;

        /// <summary>
        /// Indica se as colisões estão habilitadas
        /// </summary>
        /// <example>true</example>
        public bool ColisoesHabilitadas { get; set; } = true;

        /// <summary>
        /// Indica se as bordas são reflexivas
        /// </summary>
        /// <example>false</example>
        public bool BordasReflexivas { get; set; } = false;

        /// <summary>
        /// Cor de fundo do universo
        /// </summary>
        /// <example>rgb(0,0,0)</example>
        [RegularExpression(@"^rgb\(\d{1,3},\d{1,3},\d{1,3}\)$", ErrorMessage = "Formato de cor inválido. Use: rgb(255,255,255)")]
        public string CorFundo { get; set; } = "rgb(0,0,0)";
    }

    /// <summary>
    /// DTO para atualização de uma simulação existente
    /// </summary>
    public class AtualizarSimulacaoDto
    {
        /// <summary>
        /// Nome descritivo da simulação
        /// </summary>
        /// <example>Simulação Atualizada</example>
        [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        public string? Nome { get; set; }

        /// <summary>
        /// Descrição da simulação
        /// </summary>
        /// <example>Descrição atualizada da simulação</example>
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Status da simulação
        /// </summary>
        /// <example>Pausada</example>
        public StatusSimulacao? Status { get; set; }

        /// <summary>
        /// Número de iterações
        /// </summary>
        /// <example>200</example>
        [Range(0, int.MaxValue, ErrorMessage = "O número de iterações deve ser não negativo")]
        public int? NumeroIteracoes { get; set; }

        /// <summary>
        /// Número de colisões
        /// </summary>
        /// <example>5</example>
        [Range(0, int.MaxValue, ErrorMessage = "O número de colisões deve ser não negativo")]
        public int? NumeroColisoes { get; set; }
    }

    /// <summary>
    /// DTO para representação de estatísticas da simulação
    /// </summary>
    public class EstatisticasSimulacaoDto
    {
        /// <summary>
        /// Massa total de todos os corpos celestes
        /// </summary>
        /// <example>150.5</example>
        public double MassaTotal { get; set; }

        /// <summary>
        /// Energia cinética total do sistema
        /// </summary>
        /// <example>2450.75</example>
        public double EnergiaCineticaTotal { get; set; }

        /// <summary>
        /// Quantidade de corpos ativos
        /// </summary>
        /// <example>8</example>
        public int CorposAtivos { get; set; }

        /// <summary>
        /// Quantidade de corpos por tipo
        /// </summary>
        public Dictionary<string, int> CorposPorTipo { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Tempo total de simulação (em segundos virtuais)
        /// </summary>
        /// <example>540000</example>
        public double TempoTotalSimulacao { get; set; }

        /// <summary>
        /// Taxa de colisões por iteração
        /// </summary>
        /// <example>0.02</example>
        public double TaxaColisoes { get; set; }
    }

    /// <summary>
    /// DTO para resumo de simulação (listagens)
    /// </summary>
    public class SimulacaoResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public StatusSimulacao Status { get; set; }
        public int QuantidadeCorpos { get; set; }
        public int NumeroIteracoes { get; set; }
        public int NumeroColisoes { get; set; }
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO para resposta de criação de simulação
    /// </summary>
    public class SimulacaoCriadaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}