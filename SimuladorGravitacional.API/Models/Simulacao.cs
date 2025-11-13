using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Representa uma simulação gravitacional de corpos celestes
    /// </summary>
    [Table("Simulacoes")]
    public class Simulacao
    {
        /// <summary>
        /// Identificador único da simulação
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da simulação
        /// </summary>
        [Required(ErrorMessage = "O nome da simulação é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome da simulação não pode exceder 200 caracteres")]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de criação da simulação
        /// </summary>
        [Required]
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data e hora da última atualização da simulação
        /// </summary>
        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Número total de iterações realizadas na simulação
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "O número de iterações deve ser não negativo")]
        [Column("numero_iteracoes")]
        public int NumeroIteracoes { get; set; }

        /// <summary>
        /// Número total de colisões detectadas durante a simulação
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "O número de colisões deve ser não negativo")]
        [Column("numero_colisoes")]
        public int NumeroColisoes { get; set; }

        /// <summary>
        /// Número de corpos celestes na simulação
        /// </summary>
        [Required]
        [Range(1, 1000, ErrorMessage = "O número de corpos celestes deve estar entre 1 e 1000")]
        [Column("quantidade_corpos")]
        public int QuantidadeCorpos { get; set; }

        /// <summary>
        /// Status atual da simulação
        /// </summary>
        [Required]
        [Column("status")]
        public StatusSimulacao Status { get; set; } = StatusSimulacao.Rascunho;

        /// <summary>
        /// Descrição da simulação (opcional)
        /// </summary>
        [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        [Column("descricao")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Configurações da simulação (serializadas em JSON)
        /// </summary>
        [Column("configuracao", TypeName = "jsonb")]
        public string? Configuracao { get; set; }

        // Propriedades de Navegação

        /// <summary>
        /// Universo associado a esta simulação (relação 1:1)
        /// </summary>
        public Universo Universo { get; set; } = null!;

        /// <summary>
        /// Coleção de eventos e logs da simulação
        /// </summary>
        public ICollection<EventoSimulacao> Eventos { get; set; } = new List<EventoSimulacao>();

        // Construtores

        /// <summary>
        /// Construtor sem parâmetros para Entity Framework Core
        /// </summary>
        public Simulacao() { }

        /// <summary>
        /// Cria uma nova simulação com parâmetros básicos
        /// </summary>
        /// <param name="nome">Nome da simulação</param>
        /// <param name="quantidadeCorpos">Número de corpos celestes</param>
        /// <param name="descricao">Descrição opcional</param>
        public Simulacao(string nome, int quantidadeCorpos, string? descricao = null)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            QuantidadeCorpos = quantidadeCorpos;
            Descricao = descricao;
            DataCriacao = DateTime.UtcNow;
            Status = StatusSimulacao.Rascunho;
        }

        /// <summary>
        /// Cria uma nova simulação com parâmetros completos
        /// </summary>
        public Simulacao(string nome, int numeroIteracoes, int numeroColisoes, int quantidadeCorpos, 
                        StatusSimulacao status = StatusSimulacao.Rascunho, string? descricao = null)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            NumeroIteracoes = numeroIteracoes;
            NumeroColisoes = numeroColisoes;
            QuantidadeCorpos = quantidadeCorpos;
            Status = status;
            Descricao = descricao;
            DataCriacao = DateTime.UtcNow;
        }

        // Métodos de Negócio

        /// <summary>
        /// Inicia a simulação e atualiza seu status
        /// </summary>
        public void Iniciar()
        {
            if (Status != StatusSimulacao.Rascunho && Status != StatusSimulacao.Pausada)
            {
                throw new InvalidOperationException("A simulação só pode ser iniciada a partir dos status Rascunho ou Pausada");
            }

            Status = StatusSimulacao.Executando;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Pausa a simulação
        /// </summary>
        public void Pausar()
        {
            if (Status != StatusSimulacao.Executando)
            {
                throw new InvalidOperationException("Apenas simulações em execução podem ser pausadas");
            }

            Status = StatusSimulacao.Pausada;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Para e finaliza a simulação
        /// </summary>
        public void Finalizar()
        {
            if (Status != StatusSimulacao.Executando && Status != StatusSimulacao.Pausada)
            {
                throw new InvalidOperationException("Apenas simulações em execução ou pausadas podem ser finalizadas");
            }

            Status = StatusSimulacao.Concluida;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Cancela a simulação
        /// </summary>
        public void Cancelar()
        {
            Status = StatusSimulacao.Cancelada;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Registra uma nova iteração na simulação
        /// </summary>
        public void RegistrarIteracao()
        {
            if (Status != StatusSimulacao.Executando)
            {
                throw new InvalidOperationException("Não é possível registrar iterações em uma simulação não executando");
            }

            NumeroIteracoes++;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Registra uma colisão na simulação
        /// </summary>
        public void RegistrarColisao()
        {
            if (Status != StatusSimulacao.Executando)
            {
                throw new InvalidOperationException("Não é possível registrar colisões em uma simulação não executando");
            }

            NumeroColisoes++;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Valida a configuração da simulação
        /// </summary>
        /// <returns>True se a simulação é válida, false caso contrário</returns>
        public bool Validar()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   QuantidadeCorpos >= 1 &&
                   QuantidadeCorpos <= 1000 &&
                   NumeroIteracoes >= 0 &&
                   NumeroColisoes >= 0;
        }

        /// <summary>
        /// Retorna um resumo da simulação
        /// </summary>
        public string ObterResumo()
        {
            return $"{Nome}: {QuantidadeCorpos} corpos, {NumeroIteracoes} iterações, {NumeroColisoes} colisões";
        }

        /// <summary>
        /// Atualiza a data de modificação
        /// </summary>
        public void AtualizarDataModificacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Representa os possíveis estados de uma simulação
    /// </summary>
    public enum StatusSimulacao
    {
        /// <summary>
        /// Simulação está em rascunho/editável
        /// </summary>
        Rascunho = 0,

        /// <summary>
        /// Simulação está executando
        /// </summary>
        Executando = 1,

        /// <summary>
        /// Simulação está pausada
        /// </summary>
        Pausada = 2,

        /// <summary>
        /// Simulação foi concluída com sucesso
        /// </summary>
        Concluida = 3,

        /// <summary>
        /// Simulação foi parada devido a um erro
        /// </summary>
        Erro = 4,

        /// <summary>
        /// Simulação foi cancelada pelo usuário
        /// </summary>
        Cancelada = 5
    }

    /// <summary>
    /// Representa um evento ou entrada de log para uma simulação
    /// </summary>
    [Table("EventosSimulacao")]
    public class EventoSimulacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("simulacao_id")]
        public int SimulacaoId { get; set; }

        [Required]
        [Column("data_hora")]
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        [Column("tipo_evento")]
        public string TipoEvento { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        [Column("detalhes", TypeName = "jsonb")]
        public string? Detalhes { get; set; }

        [Column("nivel")]
        public NivelEvento Nivel { get; set; } = NivelEvento.Informacao;

        // Propriedade de navegação
        public Simulacao Simulacao { get; set; } = null!;

        public EventoSimulacao() { }

        public EventoSimulacao(int simulacaoId, string tipoEvento, string mensagem, string? detalhes = null, NivelEvento nivel = NivelEvento.Informacao)
        {
            SimulacaoId = simulacaoId;
            TipoEvento = tipoEvento;
            Mensagem = mensagem;
            Detalhes = detalhes;
            Nivel = nivel;
            DataHora = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Níveis de severidade para eventos da simulação
    /// </summary>
    public enum NivelEvento
    {
        /// <summary>
        /// Evento informativo
        /// </summary>
        Informacao = 0,

        /// <summary>
        /// Aviso
        /// </summary>
        Aviso = 1,

        /// <summary>
        /// Erro
        /// </summary>
        Erro = 2,

        /// <summary>
        /// Evento crítico
        /// </summary>
        Critico = 3
    }
}