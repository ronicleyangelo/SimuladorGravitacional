using System;
using ProgramacaoAvancada.DTOs;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Classe que representa uma simulação gravitacional salva no banco de dados
    /// Esta classe é usada para persistir o estado completo de uma simulação
    /// permitindo que os usuários salvem e restaurem simulações específicas
    /// </summary>
    public class SimulacaoSalva
    {
        /// <summary>
        /// Identificador único da simulação no banco de dados
        /// Chave primária autoincrementada
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo dado pelo usuário para identificar a simulação
        /// Exemplos: "Sistema Solar", "Galáxia Espiral", "Colisão de Asteroides"
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Número total de corpos celestes na simulação quando foi salva
        /// Útil para filtragem e estatísticas sem precisar carregar todos os corpos
        /// </summary>
        public int NumeroCorpos { get; set; }

        /// <summary>
        /// Número de iterações (frames) executadas até o momento do salvamento
        /// Indica o progresso da simulação e pode ser usado para reiniciar de onde parou
        /// </summary>
        public int Iteracoes { get; set; }

        /// <summary>
        /// Número total de colisões detectadas durante a simulação
        /// Métrica importante para análise do comportamento do sistema
        /// Colisões resultam na fusão de corpos e alteram drasticamente o sistema
        /// </summary>
        public int Colisoes { get; set; }

        /// <summary>
        /// Valor da constante gravitacional usada na simulação
        /// Controla a intensidade das forças gravitacionais entre os corpos
        /// Valores mais altos resultam em órbitas mais rápidas e colisões mais frequentes
        /// </summary>
        public double Gravidade { get; set; }

        /// <summary>
        /// Largura do canvas (área de visualização) da simulação em pixels
        /// Define os limites horizontais do espaço simulado
        /// </summary>
        public int CanvasWidth { get; set; }

        /// <summary>
        /// Altura do canvas (área de visualização) da simulação em pixels
        /// Define os limites verticais do espaço simulado
        /// </summary>
        public int CanvasHeight { get; set; }

        /// <summary>
        /// Data e hora em que a simulação foi salva
        /// Usado para ordenação, filtragem e histórico de simulações
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Lista de todos os corpos celestes presentes na simulação no momento do salvamento
        /// Usa DTOs (Data Transfer Objects) para serialização eficiente e desacoplamento
        /// Cada CorpoDto contém posição, velocidade, massa e outras propriedades físicas
        /// </summary>
        public List<CorpoDto> Corpos { get; set; } = new List<CorpoDto>();
    }
}