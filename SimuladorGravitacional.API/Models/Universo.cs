using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Representa um universo virtual para simulação de sistemas gravitacionais
    /// </summary>
    [Table("Universos")]
    public class Universo
    {
        /// <summary>
        /// Identificador único do universo
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo do universo
        /// </summary>
        [Required(ErrorMessage = "O nome do universo é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional do universo
        /// </summary>
        [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        [Column("descricao")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Largura do espaço de simulação em unidades virtuais
        /// </summary>
        [Required(ErrorMessage = "A largura do canvas é obrigatória")]
        [Range(100, 10000, ErrorMessage = "A largura deve estar entre 100 e 10.000 unidades")]
        [Column("largura_canvas")]
        public double LarguraCanvas { get; set; } = 800;

        /// <summary>
        /// Altura do espaço de simulação em unidades virtuais
        /// </summary>
        [Required(ErrorMessage = "A altura do canvas é obrigatória")]
        [Range(100, 10000, ErrorMessage = "A altura deve estar entre 100 e 10.000 unidades")]
        [Column("altura_canvas")]
        public double AlturaCanvas { get; set; } = 600;

        /// <summary>
        /// Fator de escala para conversão entre unidades físicas e virtuais
        /// </summary>
        [Required(ErrorMessage = "O fator de simulação é obrigatório")]
        [Range(1, 1e9, ErrorMessage = "O fator de simulação deve estar entre 1 e 1.000.000.000")]
        [Column("fator_simulacao")]
        public double FatorSimulacao { get; set; } = 1e5;

        /// <summary>
        /// Constante gravitacional personalizada para o universo
        /// </summary>
        [Range(1e-11, 1e-9, ErrorMessage = "A constante gravitacional deve estar entre 1e-11 e 1e-9")]
        [Column("constante_gravitacional")]
        public double ConstanteGravitacional { get; set; } = 6.67430e-11;

        /// <summary>
        /// Passo de tempo para cada iteração da simulação (em segundos)
        /// </summary>
        [Range(0.001, 86400, ErrorMessage = "O passo de tempo deve estar entre 0.001 e 86400 segundos")]
        [Column("passo_tempo")]
        public double PassoTempo { get; set; } = 1.0;

        /// <summary>
        /// Indica se as colisões entre corpos estão habilitadas
        /// </summary>
        [Column("colisoes_habilitadas")]
        public bool ColisoesHabilitadas { get; set; } = true;

        /// <summary>
        /// Indica se as bordas do universo são reflexivas
        /// </summary>
        [Column("bordas_reflexivas")]
        public bool BordasReflexivas { get; set; } = false;

        /// <summary>
        /// Cor de fundo do universo em formato RGB
        /// </summary>
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^rgb\(\d{1,3},\d{1,3},\d{1,3}\)$", ErrorMessage = "Formato de cor inválido. Use: rgb(255,255,255)")]
        [Column("cor_fundo")]
        public string CorFundo { get; set; } = "rgb(0,0,0)";

        /// <summary>
        /// Data e hora de criação do universo
        /// </summary>
        [Required]
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data e hora da última atualização do universo
        /// </summary>
        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Configurações adicionais do universo (JSON serializado)
        /// </summary>
        [Column("configuracao", TypeName = "jsonb")]
        public string? Configuracao { get; set; }

        // Propriedades de Navegação

        /// <summary>
        /// Coleção de corpos celestes que compõem este universo
        /// </summary>
        public ICollection<Corpo> Corpos { get; set; } = new List<Corpo>();

        /// <summary>
        /// Simulação à qual este universo pertence
        /// </summary>
        [Required]
        [Column("simulacao_id")]
        public int SimulacaoId { get; set; }
        
        [ForeignKey("SimulacaoId")]
        public Simulacao Simulacao { get; set; } = null!;

        // Construtores

        /// <summary>
        /// Construtor sem parâmetros para Entity Framework Core
        /// </summary>
        public Universo() { }

        /// <summary>
        /// Cria um novo universo com parâmetros básicos
        /// </summary>
        public Universo(string nome, double larguraCanvas, double alturaCanvas, double fatorSimulacao = 1e5)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            LarguraCanvas = larguraCanvas;
            AlturaCanvas = alturaCanvas;
            FatorSimulacao = fatorSimulacao;
            DataCriacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Cria um novo universo com todos os parâmetros
        /// </summary>
        public Universo(string nome, string? descricao, double larguraCanvas, double alturaCanvas, 
                       double fatorSimulacao, double constanteGravitacional, double passoTempo,
                       bool colisoesHabilitadas = true, bool bordasReflexivas = false, 
                       string corFundo = "rgb(0,0,0)")
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Descricao = descricao;
            LarguraCanvas = larguraCanvas;
            AlturaCanvas = alturaCanvas;
            FatorSimulacao = fatorSimulacao;
            ConstanteGravitacional = constanteGravitacional;
            PassoTempo = passoTempo;
            ColisoesHabilitadas = colisoesHabilitadas;
            BordasReflexivas = bordasReflexivas;
            CorFundo = corFundo;
            DataCriacao = DateTime.UtcNow;
        }

        // Métodos de Negócio

        /// <summary>
        /// Calcula a área total do universo
        /// </summary>
        /// <returns>Área em unidades quadradas</returns>
        public double CalcularArea()
        {
            return LarguraCanvas * AlturaCanvas;
        }

        /// <summary>
        /// Calcula o centro do universo
        /// </summary>
        /// <returns>Tupla com coordenadas X e Y do centro</returns>
        public (double CentroX, double CentroY) CalcularCentro()
        {
            return (LarguraCanvas / 2, AlturaCanvas / 2);
        }

        /// <summary>
        /// Verifica se uma posição está dentro dos limites do universo
        /// </summary>
        /// <param name="posicaoX">Posição horizontal</param>
        /// <param name="posicaoY">Posição vertical</param>
        /// <returns>True se a posição está dentro dos limites</returns>
        public bool EstaDentroDosLimites(double posicaoX, double posicaoY)
        {
            return posicaoX >= 0 && posicaoX <= LarguraCanvas && 
                   posicaoY >= 0 && posicaoY <= AlturaCanvas;
        }

        /// <summary>
        /// Aplica as regras de borda a uma posição (reflexão ou teleporte)
        /// </summary>
        /// <param name="posicaoX">Posição horizontal</param>
        /// <param name="posicaoY">Posição vertical</param>
        /// <returns>Nova posição ajustada às bordas</returns>
        public (double NovaPosicaoX, double NovaPosicaoY) AplicarRegrasBorda(double posicaoX, double posicaoY)
        {
            double novaX = posicaoX;
            double novaY = posicaoY;

            if (BordasReflexivas)
            {
                // Reflexão nas bordas
                if (posicaoX < 0) novaX = -posicaoX;
                else if (posicaoX > LarguraCanvas) novaX = 2 * LarguraCanvas - posicaoX;

                if (posicaoY < 0) novaY = -posicaoY;
                else if (posicaoY > AlturaCanvas) novaY = 2 * AlturaCanvas - posicaoY;
            }
            else
            {
                // Teleporte (comportamento padrão)
                if (posicaoX < 0) novaX = LarguraCanvas + posicaoX;
                else if (posicaoX > LarguraCanvas) novaX = posicaoX - LarguraCanvas;

                if (posicaoY < 0) novaY = AlturaCanvas + posicaoY;
                else if (posicaoY > AlturaCanvas) novaY = posicaoY - AlturaCanvas;
            }

            return (novaX, novaY);
        }

        /// <summary>
        /// Calcula a massa total de todos os corpos no universo
        /// </summary>
        /// <returns>Massa total em unidades de massa</returns>
        public double CalcularMassaTotal()
        {
            return Corpos.Where(c => c.Ativo).Sum(c => c.Massa);
        }

        /// <summary>
        /// Calcula o centro de massa do universo
        /// </summary>
        /// <returns>Tupla com coordenadas X e Y do centro de massa</returns>
        public (double CentroMassaX, double CentroMassaY) CalcularCentroDeMassa()
        {
            var corposAtivos = Corpos.Where(c => c.Ativo).ToList();
            if (!corposAtivos.Any())
                return CalcularCentro();

            double massaTotal = corposAtivos.Sum(c => c.Massa);
            double centroMassaX = corposAtivos.Sum(c => c.PosicaoX * c.Massa) / massaTotal;
            double centroMassaY = corposAtivos.Sum(c => c.PosicaoY * c.Massa) / massaTotal;

            return (centroMassaX, centroMassaY);
        }

        /// <summary>
        /// Obtém todos os corpos ativos no universo
        /// </summary>
        /// <returns>Lista de corpos ativos</returns>
        public List<Corpo> ObterCorposAtivos()
        {
            return Corpos.Where(c => c.Ativo).ToList();
        }

        /// <summary>
        /// Obtém corpos por tipo
        /// </summary>
        /// <param name="tipo">Tipo de corpo celeste</param>
        /// <returns>Lista de corpos do tipo especificado</returns>
        public List<Corpo> ObterCorposPorTipo(TipoCorpoCeleste tipo)
        {
            return Corpos.Where(c => c.Ativo && c.TipoCorpo == tipo).ToList();
        }

        /// <summary>
        /// Verifica se há colisões no universo
        /// </summary>
        /// <returns>Lista de pares de corpos em colisão</returns>
        public List<(Corpo, Corpo)> VerificarColisoes()
        {
            var colisoes = new List<(Corpo, Corpo)>();
            var corposAtivos = ObterCorposAtivos();

            for (int i = 0; i < corposAtivos.Count; i++)
            {
                for (int j = i + 1; j < corposAtivos.Count; j++)
                {
                    if (corposAtivos[i].VerificarColisao(corposAtivos[j]))
                    {
                        colisoes.Add((corposAtivos[i], corposAtivos[j]));
                    }
                }
            }

            return colisoes;
        }

        /// <summary>
        /// Adiciona um corpo celeste ao universo
        /// </summary>
        /// <param name="corpo">Corpo celeste a ser adicionado</param>
        public void AdicionarCorpo(Corpo corpo)
        {
            if (corpo == null)
                throw new ArgumentNullException(nameof(corpo));

            corpo.UniversoId = Id;
            Corpos.Add(corpo);
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Remove um corpo celeste do universo
        /// </summary>
        /// <param name="corpo">Corpo celeste a ser removido</param>
        public void RemoverCorpo(Corpo corpo)
        {
            if (corpo == null)
                throw new ArgumentNullException(nameof(corpo));

            Corpos.Remove(corpo);
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Limpa todos os corpos do universo
        /// </summary>
        public void LimparCorpos()
        {
            Corpos.Clear();
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Valida se o universo está em uma configuração válida
        /// </summary>
        /// <returns>True se válido, False caso contrário</returns>
        public bool Validar()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   LarguraCanvas >= 100 &&
                   AlturaCanvas >= 100 &&
                   FatorSimulacao >= 1 &&
                   ConstanteGravitacional >= 1e-11 &&
                   PassoTempo >= 0.001;
        }

        /// <summary>
        /// Retorna um resumo descritivo do universo
        /// </summary>
        /// <returns>String com resumo do universo</returns>
        public string ObterResumo()
        {
            var corposAtivos = ObterCorposAtivos();
            return $"{Nome}: {LarguraCanvas}x{AlturaCanvas}, {corposAtivos.Count} corpos ativos, Massa total: {CalcularMassaTotal():F2}";
        }

        /// <summary>
        /// Atualiza a data de modificação do universo
        /// </summary>
        public void AtualizarDataModificacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}