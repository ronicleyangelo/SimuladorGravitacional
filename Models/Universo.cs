using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Models
{
    public class Universo
    {
        public List<Corpo> Corpos { get; private set; }
        public double Largura { get; set; }
        public double Altura { get; set; }
        public double ConstanteGravitacional { get; set; }
        public int ColisoesDetectadas { get; private set; }

        public Universo(double largura, double altura, double constanteGravitacional)
        {
            Corpos = new List<Corpo>();
            Largura = largura;
            Altura = altura;
            ConstanteGravitacional = constanteGravitacional;
            ColisoesDetectadas = 0;
        }

        public void AdicionarCorpo(Corpo corpo)
        {
            Corpos.Add(corpo);
        }

        public void Simular(double deltaTime)
        {
            // Aplicar gravidade entre todos os corpos
            for (int i = 0; i < Corpos.Count; i++)
            {
                for (int j = i + 1; j < Corpos.Count; j++)
                {
                    Corpos[i].AplicarGravidade(Corpos[j], ConstanteGravitacional, deltaTime);
                }
            }

            // Atualizar posições
            for (int i = Corpos.Count - 1; i >= 0; i--)
            {
                Corpos[i].AtualizarPosicao(Largura, Altura);
            }

            // Detectar e processar colisões
            DetectarColisoes();
        }

        // ✅ MÉTODO AUXILIAR: Calcular distância entre corpos
        private double CalcularDistancia(Corpo a, Corpo b)
        {
            double dx = b.PosX - a.PosX;
            double dy = b.PosY - a.PosY;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void DetectarColisoes()
        {
            var corposParaRemover = new List<Corpo>();
            var corposFundidos = new List<Corpo>();

            for (int i = Corpos.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (i < Corpos.Count && j < Corpos.Count && i != j)
                    {
                        var corpoA = Corpos[i];
                        var corpoB = Corpos[j];

                        double distancia = CalcularDistancia(corpoA, corpoB);
                        if (distancia < (corpoA.Raio + corpoB.Raio))
                        {
                            var corpoFundido = Corpo.Fundir(corpoA, corpoB);
                            corposFundidos.Add(corpoFundido);
                            corposParaRemover.Add(corpoA);
                            corposParaRemover.Add(corpoB);
                            ColisoesDetectadas++;
                        }
                    }
                }
            }

            // Remove corpos colididos e adiciona os fundidos
            foreach (var corpo in corposParaRemover)
            {
                Corpos.Remove(corpo);
            }
            
            foreach (var corpo in corposFundidos)
            {
                Corpos.Add(corpo);
            }
        }
    }
}