using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramacaoAvancada.Models
{
    public class Universo
    {
        public List<Corpo> Corpos { get; private set; } = new List<Corpo>();

        private double canvasWidth;
        private double canvasHeight;
        private double fatorSimulacao;

        public int ColisoesDetectadas { get; private set; } = 0;

        public Universo(double canvasWidth, double canvasHeight, double fatorSimulacao = 1e5)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.fatorSimulacao = fatorSimulacao;
        }

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
                    Corpos[i].AplicarGravidade(Corpos[j], fatorSimulacao, deltaTime);
                }
            }

            // Atualizar posições
            foreach (var c in Corpos)
            {
                c.AtualizarPosicao(canvasWidth, canvasHeight);
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
    }
}
