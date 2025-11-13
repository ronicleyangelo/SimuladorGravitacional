using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramacaoAvancada.Models
{
    /// <summary>
    /// Representa um corpo celeste em uma simulação gravitacional
    /// </summary>
    [Table("CorposCelestes")]
    public class Corpo
    {
        /// <summary>
        /// Identificador único do corpo celeste
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo do corpo celeste
        /// </summary>
        [Required(ErrorMessage = "O nome do corpo celeste é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Massa do corpo celeste em unidades de massa solar
        /// </summary>
        [Required(ErrorMessage = "A massa é obrigatória")]
        [Range(0.1, 1000, ErrorMessage = "A massa deve estar entre 0.1 e 1000 unidades")]
        [Column("massa")]
        public double Massa { get; set; }

        /// <summary>
        /// Densidade do corpo celeste em g/cm³
        /// </summary>
        [Required(ErrorMessage = "A densidade é obrigatória")]
        [Range(0.1, 20, ErrorMessage = "A densidade deve estar entre 0.1 e 20 g/cm³")]
        [Column("densidade")]
        public double Densidade { get; set; }

        /// <summary>
        /// Raio do corpo celeste em unidades de raio terrestre
        /// </summary>
        [Required(ErrorMessage = "O raio é obrigatório")]
        [Range(1, 100, ErrorMessage = "O raio deve estar entre 1 e 100 unidades")]
        [Column("raio")]
        public double Raio { get; set; }

        /// <summary>
        /// Cor de representação visual do corpo celeste em formato RGB
        /// </summary>
        [Required(ErrorMessage = "A cor é obrigatória")]
        [MaxLength(50, ErrorMessage = "A cor não pode exceder 50 caracteres")]
        [RegularExpression(@"^rgb\(\d{1,3},\d{1,3},\d{1,3}\)$", ErrorMessage = "Formato de cor inválido. Use: rgb(255,255,255)")]
        [Column("cor")]
        public string Cor { get; set; } = "rgb(255,255,255)";

        /// <summary>
        /// Posição horizontal no canvas de simulação
        /// </summary>
        [Required(ErrorMessage = "A posição X é obrigatória")]
        [Range(0, double.MaxValue, ErrorMessage = "A posição X deve ser não negativa")]
        [Column("posicao_x")]
        public double PosicaoX { get; set; }

        /// <summary>
        /// Posição vertical no canvas de simulação
        /// </summary>
        [Required(ErrorMessage = "A posição Y é obrigatória")]
        [Range(0, double.MaxValue, ErrorMessage = "A posição Y deve ser não negativa")]
        [Column("posicao_y")]
        public double PosicaoY { get; set; }

        /// <summary>
        /// Velocidade horizontal em unidades por iteração
        /// </summary>
        [Required(ErrorMessage = "A velocidade X é obrigatória")]
        [Column("velocidade_x")]
        public double VelocidadeX { get; set; } = 0;

        /// <summary>
        /// Velocidade vertical em unidades por iteração
        /// </summary>
        [Required(ErrorMessage = "A velocidade Y é obrigatória")]
        [Column("velocidade_y")]
        public double VelocidadeY { get; set; } = 0;

        /// <summary>
        /// Aceleração horizontal em unidades por iteração²
        /// </summary>
        [Column("aceleracao_x")]
        public double AceleracaoX { get; set; } = 0;

        /// <summary>
        /// Aceleração vertical em unidades por iteração²
        /// </summary>
        [Column("aceleracao_y")]
        public double AceleracaoY { get; set; } = 0;

        /// <summary>
        /// Tipo do corpo celeste (estrela, planeta, lua, asteroide, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column("tipo_corpo")]
        public TipoCorpoCeleste TipoCorpo { get; set; } = TipoCorpoCeleste.Planeta;

        /// <summary>
        /// Indica se o corpo celeste está ativo na simulação
        /// </summary>
        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Data e hora da última atualização do corpo celeste
        /// </summary>
        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Metadados adicionais do corpo celeste (JSON serializado)
        /// </summary>
        [Column("metadados", TypeName = "jsonb")]
        public string? Metadados { get; set; }

        // Chave estrangeira para Universo
        [Required]
        [Column("universo_id")]
        public int UniversoId { get; set; }
        
        /// <summary>
        /// Universo ao qual este corpo celeste pertence
        /// </summary>
        [ForeignKey("UniversoId")]
        public Universo Universo { get; set; } = null!;

        // Construtores

        /// <summary>
        /// Construtor sem parâmetros para Entity Framework Core
        /// </summary>
        public Corpo() { }

        /// <summary>
        /// Cria um novo corpo celeste com parâmetros básicos
        /// </summary>
        public Corpo(string nome, double massa, double densidade, double raio, string cor, 
                    double posicaoX, double posicaoY, double velocidadeX = 0, double velocidadeY = 0)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Massa = massa;
            Densidade = densidade;
            Raio = raio;
            Cor = cor;
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
            VelocidadeX = velocidadeX;
            VelocidadeY = velocidadeY;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Cria um novo corpo celeste com todos os parâmetros
        /// </summary>
        public Corpo(string nome, double massa, double densidade, double raio, string cor,
                    double posicaoX, double posicaoY, double velocidadeX, double velocidadeY,
                    TipoCorpoCeleste tipoCorpo, bool ativo = true)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Massa = massa;
            Densidade = densidade;
            Raio = raio;
            Cor = cor;
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
            VelocidadeX = velocidadeX;
            VelocidadeY = velocidadeY;
            TipoCorpo = tipoCorpo;
            Ativo = ativo;
            DataAtualizacao = DateTime.UtcNow;
        }

        // Métodos de Negócio

        /// <summary>
        /// Calcula o volume do corpo celeste baseado em sua densidade e massa
        /// </summary>
        /// <returns>Volume em unidades cúbicas</returns>
        public double CalcularVolume()
        {
            return Massa / Densidade;
        }

        /// <summary>
        /// Calcula a área de superfície do corpo celeste
        /// </summary>
        /// <returns>Área de superfície em unidades quadradas</returns>
        public double CalcularAreaSuperficie()
        {
            return 4 * Math.PI * Math.Pow(Raio, 2);
        }

        /// <summary>
        /// Calcula a velocidade total do corpo celeste
        /// </summary>
        /// <returns>Velocidade total em unidades por iteração</returns>
        public double CalcularVelocidadeTotal()
        {
            return Math.Sqrt(Math.Pow(VelocidadeX, 2) + Math.Pow(VelocidadeY, 2));
        }

        /// <summary>
        /// Calcula a energia cinética do corpo celeste
        /// </summary>
        /// <returns>Energia cinética em unidades apropriadas</returns>
        public double CalcularEnergiaCinetica()
        {
            double velocidadeTotal = CalcularVelocidadeTotal();
            return 0.5 * Massa * Math.Pow(velocidadeTotal, 2);
        }

        /// <summary>
        /// Atualiza a posição do corpo celeste baseado em sua velocidade
        /// </summary>
        public void AtualizarPosicao()
        {
            PosicaoX += VelocidadeX;
            PosicaoY += VelocidadeY;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Atualiza a velocidade do corpo celeste baseado em sua aceleração
        /// </summary>
        public void AtualizarVelocidade()
        {
            VelocidadeX += AceleracaoX;
            VelocidadeY += AceleracaoY;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Aplica uma força ao corpo celeste
        /// </summary>
        /// <param name="forcaX">Componente horizontal da força</param>
        /// <param name="forcaY">Componente vertical da força</param>
        public void AplicarForca(double forcaX, double forcaY)
        {
            if (Massa <= 0) return;

            AceleracaoX += forcaX / Massa;
            AceleracaoY += forcaY / Massa;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Reseta as acelerações do corpo celeste
        /// </summary>
        public void ResetarAceleracao()
        {
            AceleracaoX = 0;
            AceleracaoY = 0;
            DataAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se este corpo colidiu com outro corpo celeste
        /// </summary>
        /// <param name="outroCorpo">Outro corpo celeste para verificação de colisão</param>
        /// <returns>True se houve colisão, False caso contrário</returns>
        public bool VerificarColisao(Corpo outroCorpo)
        {
            if (!Ativo || !outroCorpo.Ativo) return false;

            double distanciaX = PosicaoX - outroCorpo.PosicaoX;
            double distanciaY = PosicaoY - outroCorpo.PosicaoY;
            double distancia = Math.Sqrt(Math.Pow(distanciaX, 2) + Math.Pow(distanciaY, 2));

            return distancia <= (Raio + outroCorpo.Raio);
        }

        /// <summary>
        /// Calcula a distância até outro corpo celeste
        /// </summary>
        /// <param name="outroCorpo">Outro corpo celeste</param>
        /// <returns>Distância em unidades</returns>
        public double CalcularDistancia(Corpo outroCorpo)
        {
            double distanciaX = PosicaoX - outroCorpo.PosicaoX;
            double distanciaY = PosicaoY - outroCorpo.PosicaoY;
            return Math.Sqrt(Math.Pow(distanciaX, 2) + Math.Pow(distanciaY, 2));
        }

        /// <summary>
        /// Valida se o corpo celeste está em uma configuração válida
        /// </summary>
        /// <returns>True se válido, False caso contrário</returns>
        public bool Validar()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   Massa >= 0.1 &&
                   Densidade >= 0.1 &&
                   Raio >= 1 &&
                   !string.IsNullOrWhiteSpace(Cor) &&
                   PosicaoX >= 0 &&
                   PosicaoY >= 0;
        }

        /// <summary>
        /// Retorna um resumo descritivo do corpo celeste
        /// </summary>
        /// <returns>String com resumo do corpo celeste</returns>
        public string ObterResumo()
        {
            return $"{Nome} ({TipoCorpo}): Massa={Massa}, Raio={Raio}, Posição=({PosicaoX:F2}, {PosicaoY:F2})";
        }

        /// <summary>
        /// Atualiza a data de modificação do corpo celeste
        /// </summary>
        public void AtualizarDataModificacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Tipos de corpos celestes suportados na simulação
    /// </summary>
    public enum TipoCorpoCeleste
    {
        /// <summary>
        /// Corpo estelar (estrela)
        /// </summary>
        Estrela = 0,

        /// <summary>
        /// Planeta
        /// </summary>
        Planeta = 1,

        /// <summary>
        /// Satélite natural (lua)
        /// </summary>
        Lua = 2,

        /// <summary>
        /// Planeta anão
        /// </summary>
        PlanetaAnao = 3,

        /// <summary>
        /// Asteroide
        /// </summary>
        Asteroide = 4,

        /// <summary>
        /// Cometa
        /// </summary>
        Cometa = 5,

        /// <summary>
        /// Buraco negro
        /// </summary>
        BuracoNegro = 6,

        /// <summary>
        /// Outro tipo de corpo celeste
        /// </summary>
        Outro = 7
    }
}