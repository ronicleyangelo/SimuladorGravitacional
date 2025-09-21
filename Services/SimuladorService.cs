using ProgramacaoAvancada.Models;
using System.Text.Json;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {
        private const double G = 6.674184e-11;

        public List<Corpo> Corpos { get; private set; } = new();
        public int Iteracoes { get; private set; }
        public int Colisoes { get; private set; }
        public bool Rodando { get; private set; }
        public double Velocidade { get; set; } = 1.0;
        public double Gravidade { get; set; } = 0.5;
        public int NumCorpos { get; set; } = 8;

        public double CanvasWidth { get; set; } = 800;
        public double CanvasHeight { get; set; } = 500;

        public List<string> Eventos { get; } = new();

        public int StatCorpos => Corpos.Count;
        public int StatIteracoes => Iteracoes;
        public int StatColisoes => Colisoes;

        public SimuladorService() => Resetar();

        public void Resetar()
        {
            Corpos = Enumerable.Range(0, NumCorpos)
                .Select(_ => Corpo.CriarAleatorio(CanvasWidth, CanvasHeight))
                .ToList();

            Iteracoes = 0;
            Colisoes = 0;
            Eventos.Clear();
            AdicionarEvento($"Simulação reiniciada com {NumCorpos} corpos.");
        }

        public void Iniciar() { Rodando = true; AdicionarEvento("Simulação iniciada."); }
        public void Parar() { Rodando = false; AdicionarEvento("Simulação pausada."); }

        public void Atualizar()
        {
            if (!Rodando) return;

            // Calcular forças gravitacionais
            for (int i = 0; i < Corpos.Count; i++)
                for (int j = i + 1; j < Corpos.Count; j++)
                    Corpos[i].AplicarGravidade(Corpos[j], Gravidade, G);

            // Atualizar posições
            foreach (var c in Corpos)
                c.AtualizarPosicao(Velocidade, CanvasWidth, CanvasHeight);

            // Detectar colisões
            DetectarColisoes();

            Iteracoes++;
        }

        private void DetectarColisoes()
        {
            for (int i = 0; i < Corpos.Count; i++)
            {
                for (int j = i + 1; j < Corpos.Count; j++)
                {
                    var a = Corpos[i];
                    var b = Corpos[j];
                    var dx = b.PosX - a.PosX;
                    var dy = b.PosY - a.PosY;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < a.Raio + b.Raio)
                    {
                        Colisoes++;
                        AdicionarEvento($"Colisão: {a.Nome} com {b.Nome}");

                        // Fusão
                        var maior = Corpo.Fundir(a, b);
                        var menor = a == maior ? b : a;
                        Corpos.Remove(menor);

                        // Reinicia loop externo
                        i = -1;
                        break;
                    }
                }
            }
        }

        public void AdicionarEvento(string mensagem)
        {
            var hora = DateTime.Now.ToLongTimeString();
            Eventos.Insert(0, $"[{hora}] {mensagem}");
            if (Eventos.Count > 20) Eventos.RemoveAt(Eventos.Count - 1);
        }

        // Salvar estado em JSON
        public string ObterEstadoJson() =>
            JsonSerializer.Serialize(new
            {
                corpos = Corpos,
                iteracoes = Iteracoes,
                colisoes = Colisoes,
                timestamp = DateTime.UtcNow.ToString("o")
            }, new JsonSerializerOptions { WriteIndented = true });

        // Carregar estado a partir de JSON
        public void CarregarEstadoJson(string json)
        {
            try
            {
                var estado = JsonSerializer.Deserialize<SimulacaoEstado>(json);
                if (estado == null) throw new Exception("Arquivo inválido");

                Corpos = estado.Corpos ?? new List<Corpo>();
                Iteracoes = estado.Iteracoes;
                Colisoes = estado.Colisoes;

                AdicionarEvento($"Estado carregado com {Corpos.Count} corpos.");
            }
            catch (Exception ex)
            {
                AdicionarEvento($"Erro ao carregar estado: {ex.Message}");
            }
        }

        private class SimulacaoEstado
        {
            public List<Corpo>? Corpos { get; set; }
            public int Iteracoes { get; set; }
            public int Colisoes { get; set; }
        }
    }
}
