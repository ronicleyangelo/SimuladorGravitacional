using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProgramacaoAvancada.DTOs
{
    /// <summary>
    /// DTO para representar um universo de simulação na resposta da API
    /// Contém as propriedades de configuração do ambiente de simulação e os corpos celestes associados
    /// </summary>
    public class UniversoDto
    {
        /// <summary>
        /// Identificador único do universo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo do universo de simulação
        /// </summary>
        [Required(ErrorMessage = "O nome do universo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Largura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A largura do canvas deve estar entre 100 e 10000 unidades")]
        public double CanvasWidth { get; set; }

        /// <summary>
        /// Altura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A altura do canvas deve estar entre 100 e 10000 unidades")]
        public double CanvasHeight { get; set; }

        /// <summary>
        /// Fator de escala para a simulação (controla a precisão e performance)
        /// </summary>
        [Range(0.0001, 1000, ErrorMessage = "O fator de simulação deve estar entre 0.0001 e 1000")]
        public double FatorSimulacao { get; set; }

        /// <summary>
        /// Lista de corpos celestes presentes neste universo
        /// </summary>
        public List<CorpoDto> Corpos { get; set; } = new List<CorpoDto>();
    }

    /// <summary>
    /// DTO para criação de um novo universo de simulação
    /// Utilizado apenas para operações de inserção (POST)
    /// </summary>
    public class CreateUniversoDto
    {
        /// <summary>
        /// Nome descritivo do universo de simulação
        /// </summary>
        [Required(ErrorMessage = "O nome do universo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Largura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A largura do canvas deve estar entre 100 e 10000 unidades")]
        public double CanvasWidth { get; set; }

        /// <summary>
        /// Altura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A altura do canvas deve estar entre 100 e 10000 unidades")]
        public double CanvasHeight { get; set; }

        /// <summary>
        /// Fator de escala para a simulação (controla a precisão e performance)
        /// </summary>
        [Range(0.0001, 1000, ErrorMessage = "O fator de simulação deve estar entre 0.0001 e 1000")]
        public double FatorSimulacao { get; set; } = 1.0; // Valor padrão
    }

    /// <summary>
    /// DTO para atualização de um universo existente
    /// Utilizado para operações de atualização (PUT/PATCH)
    /// </summary>
    public class UpdateUniversoDto
    {
        /// <summary>
        /// Nome descritivo do universo de simulação
        /// </summary>
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string? Nome { get; set; }

        /// <summary>
        /// Largura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A largura do canvas deve estar entre 100 e 10000 unidades")]
        public double? CanvasWidth { get; set; }

        /// <summary>
        /// Altura do canvas de simulação em pixels ou unidades de simulação
        /// </summary>
        [Range(100, 10000, ErrorMessage = "A altura do canvas deve estar entre 100 e 10000 unidades")]
        public double? CanvasHeight { get; set; }

        /// <summary>
        /// Fator de escala para a simulação (controla a precisão e performance)
        /// </summary>
        [Range(0.0001, 1000, ErrorMessage = "O fator de simulação deve estar entre 0.0001 e 1000")]
        public double? FatorSimulacao { get; set; }
    }
}