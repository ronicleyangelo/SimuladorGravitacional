using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProgramacaoAvancada.Models
{
    public class Universo
    {
        [Key]
        public int Id { get; set; }

        // ✅ ADICIONAR PROPRIEDADES QUE ESTAVAM FALTANDO
        [Required]
        public string Nome { get; set; } = "Novo Universo";

        public string? Descricao { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaModificacao { get; set; }

        [Required]
        public double CanvasWidth { get; set; }

        [Required]
        public double CanvasHeight { get; set; }

        [Required]
        public double FatorSimulacao { get; set; } = 1e5;

        // ✅ MUDAR PARA set público
        public int ColisoesDetectadas { get; set; } = 0;

        [NotMapped]
        public List<Corpo> Corpos { get; set; } = new List<Corpo>();

        // Propriedade para navegação do Entity Framework
        public virtual ICollection<Corpo> CorposNavigation { get; set; } = new List<Corpo>();

        // Construtor para Entity Framework
        public Universo() { }

        public Universo(double canvasWidth, double canvasHeight, double fatorSimulacao = 1e5)
        {
            this.CanvasWidth = canvasWidth;
            this.CanvasHeight = canvasHeight;
            this.FatorSimulacao = fatorSimulacao;
            this.Corpos = new List<Corpo>();
            this.DataCriacao = DateTime.Now;
        }

        // Método para carregar corpos do banco para a simulação
        public void CarregarCorpos(List<Corpo> corpos)
        {
            Corpos = corpos ?? new List<Corpo>();
        }

        // Método para obter corpos para salvar no banco
        public List<Corpo> ObterCorposParaSalvar()
        {
            return Corpos;
        }

        // ✅ MANTENHA TODOS OS SEUS MÉTODOS ORIGINAIS
        public void AdicionarCorpo(Corpo corpo)
        {
            Corpos.Add(corpo);
        }

        public void Simular(double deltaTime)
        {
            // Aplicar gravidade entre pares
            for (int i = 0; i < Corpos.Count; i++)
            {
                for (int j = i + 1; j < Corpos.Count; j++)
                {
                    Corpos[i].AplicarGravidade(Corpos[j], FatorSimulacao, deltaTime);
                }
            }

            // Atualizar posições
            foreach (var c in Corpos)
            {
                c.AtualizarPosicao(CanvasWidth, CanvasHeight);
            }

            // Colisões
            TratarColisoes();
        }

        private void TratarColisoes()
        {
            var novos = new List<Corpo>();
            var removidos = new HashSet<Corpo>();

            for (int i = 0; i < Corpos.Count; i++)
            {
                var a = Corpos[i];
                if (removidos.Contains(a)) continue;

                for (int j = i + 1; j < Corpos.Count; j++)
                {
                    var b = Corpos[j];
                    if (removidos.Contains(b)) continue;

                    double dx = a.PosX - b.PosX;
                    double dy = a.PosY - b.PosY;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist <= a.Raio + b.Raio)
                    {
                        var novo = Corpo.Fundir(a, b);
                        novos.Add(novo);
                        removidos.Add(a);
                        removidos.Add(b);
                        ColisoesDetectadas++;
                        break;
                    }
                }
            }

            Corpos = Corpos.Except(removidos).ToList();
            Corpos.AddRange(novos);
        }

        // Método para criar universo com corpos aleatórios
        public static Universo CriarUniversoAleatorio(double canvasWidth, double canvasHeight, int numeroCorpos, double fatorSimulacao = 1e5)
        {
            var universo = new Universo(canvasWidth, canvasHeight, fatorSimulacao);

            for (int i = 0; i < numeroCorpos; i++)
            {
                var corpo = Corpo.CriarAleatorio(canvasWidth, canvasHeight);
                universo.AdicionarCorpo(corpo);
            }

            return universo;
        }


        // Método para criar universo com distribuição uniforme
        public static Universo CriarUniversoDistribuido(double canvasWidth, double canvasHeight, int numeroCorpos, double fatorSimulacao = 1e5)
        {
            var universo = new Universo(canvasWidth, canvasHeight, fatorSimulacao);

            for (int i = 0; i < numeroCorpos; i++)
            {
                var corpo = Corpo.CriarDistribuido(canvasWidth, canvasHeight, i, numeroCorpos);
                universo.AdicionarCorpo(corpo);
            }

            return universo;
        }

        // Método para limpar universo
        public void Limpar()
        {
            Corpos.Clear();
            ColisoesDetectadas = 0;
        }

        // Método para obter estatísticas
        public string ObterEstatisticas()
        {
            return $"Corpos: {Corpos.Count}, Colisões: {ColisoesDetectadas}, " +
                   $"Massa Total: {Corpos.Sum(c => c.Massa):F2}, " +
                   $"Corpo Mais Massivo: {Corpos.Max(c => c.Massa):F2}";
        }
    }
}