using ProgramacaoAvancada.Models;
using System.Text.Json;

namespace ProgramacaoAvancada.Services
{
    public class SimuladorService
    {

        private const double G = 6.674184e-11;
        private readonly Random _rnd = new();

        public List<Corpo> Corpos { get; private set; } = new();
        public int Iteracoes { get; private set; }
        public int Colisoes { get; private set; }
        public bool Rodando { get; private set; }
        public double Velocidade { get; set; } = 1.0;
        public double Gravidade { get; set; } = 0.5;
        public int NumCorpos { get; set; } = 8;

        // logs recentes (mais novo primeiro)
        public List<string> Eventos { get; } = new();

        // canvas dims (para criação inicial)
        public double CanvasWidth { get; set; } = 800;
        public double CanvasHeight { get; set; } = 500;

        public SimuladorService()
        {
            Resetar();
        }

        public void Resetar()
        {
            Corpos.Clear();
            Iteracoes = 0;
            Colisoes = 0;
            Eventos.Clear();
            for (int i = 0; i < NumCorpos; i++)
                Corpos.Add(CriarCorpo());
            AdicionarEvento($"Simulação reiniciada com {NumCorpos} corpos.");
        }

        public void Iniciar()
        {
            Rodando = true;
            AdicionarEvento("Simulação iniciada.");
        }

        public void Parar()
        {
            Rodando = false;
            AdicionarEvento("Simulação pausada.");
        }

        private Corpo CriarCorpo()
        {
            var nomes = new[] {
                "Sol","Terra","Lua","Marte","Júpiter","Saturno","Urano","Netuno",
                "Vênus","Mercúrio","Plutão","Éris","Haumea","Makemake","Ceres"
            };
            var nome = nomes[_rnd.Next(nomes.Length)] + "-" + _rnd.Next(100);
            var massa = _rnd.NextDouble() * 50.0 + 10.0;
            var dens = _rnd.NextDouble() * 5.0 + 1.0;

            return new Corpo
            {
                Nome = nome,
                Massa = massa,
                Densidade = dens,
                PosX = _rnd.NextDouble() * CanvasWidth,
                PosY = _rnd.NextDouble() * CanvasHeight,
                VelX = (_rnd.NextDouble() - 0.5) * 2.0,
                VelY = (_rnd.NextDouble() - 0.5) * 2.0
            };
        }

        public void Atualizar()
        {
            if (!Rodando) return;

            CalcularForcas();
            AtualizarPosicoes();
            DetectarColisoes();
            Iteracoes++;
        }

        private void CalcularForcas()
        {
            // Parecido com o JS: parcimonioso, evitando r muito pequeno
            for (int i = 0; i < Corpos.Count; i++)
            {
                for (int j = i + 1; j < Corpos.Count; j++)
                {
                    var a = Corpos[i];
                    var b = Corpos[j];
                    var dx = b.PosX - a.PosX;
                    var dy = b.PosY - a.PosY;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist > 5.0)
                    {
                        var forca = Gravidade * G * a.Massa * b.Massa / (dist * dist);
                        var fx = forca * dx / dist;
                        var fy = forca * dy / dist;

                        a.VelX += fx / a.Massa;
                        a.VelY += fy / a.Massa;

                        b.VelX -= fx / b.Massa;
                        b.VelY -= fy / b.Massa;
                    }
                }
            }
        }

        private void AtualizarPosicoes()
        {
            foreach (var corpo in Corpos)
            {
                corpo.PosX += corpo.VelX * Velocidade;
                corpo.PosY += corpo.VelY * Velocidade;

                // colisão com borda: "rebote" -0.9
                if (corpo.PosX - corpo.Raio < 0)
                {
                    corpo.PosX = corpo.Raio;
                    corpo.VelX *= -0.9;
                }
                else if (corpo.PosX + corpo.Raio > CanvasWidth)
                {
                    corpo.PosX = CanvasWidth - corpo.Raio;
                    corpo.VelX *= -0.9;
                }

                if (corpo.PosY - corpo.Raio < 0)
                {
                    corpo.PosY = corpo.Raio;
                    corpo.VelY *= -0.9;
                }
                else if (corpo.PosY + corpo.Raio > CanvasHeight)
                {
                    corpo.PosY = CanvasHeight - corpo.Raio;
                    corpo.VelY *= -0.9;
                }
            }
        }

        private void DetectarColisoes()
        {
            // percorre e funde ao detectar interseção (maior "engole" menor)
            for (int i = 0; i < Corpos.Count; i++)
            {
                bool saiu = false;
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

                        Corpo maior, menor;
                        if (a.Massa >= b.Massa) { maior = a; menor = b; }
                        else { maior = b; menor = a; }

                        // Conservação do momento linear (Q = m*v)
                        maior.VelX = (maior.Massa * maior.VelX + menor.Massa * menor.VelX) / (maior.Massa + menor.Massa);
                        maior.VelY = (maior.Massa * maior.VelY + menor.Massa * menor.VelY) / (maior.Massa + menor.Massa);

                        // Aumentar massa do maior
                        maior.Massa += menor.Massa;

                        // Nova densidade (média ponderada correta):
                        // massa_total / (volume_total) -> volume = m/d
                        var volumeMaior = maior.Massa / maior.Densidade;
                        var volumeMenor = menor.Massa / menor.Densidade;
                        var volumeTotal = volumeMaior + volumeMenor;
                        if (volumeTotal > 0)
                            maior.Densidade = (maior.Massa) / volumeTotal;

                        // Reposicionar ligeiramente o maior no centro de massa:
                        maior.PosX = (maior.PosX * (maior.Massa - menor.Massa) + menor.PosX * menor.Massa) / (maior.Massa);
                        maior.PosY = (maior.PosY * (maior.Massa - menor.Massa) + menor.PosY * menor.Massa) / (maior.Massa);

                        // Remover o menor
                        Corpos.Remove(minor: menor);

                        // atualizar estatísticas e lista visual — o caller (UI) lê propriedades
                        saiu = true;
                        break; // porque alteramos a lista
                    }
                }
                if (saiu) { i = -1; /* reinicia para garantir checagem com novo array */ }
            }
        }

        // salva estado como JSON
        public string ObterEstadoJson()
        {
            var dto = new
            {
                corpos = Corpos,
                iteracoes = Iteracoes,
                colisoes = Colisoes,
                timestamp = DateTime.UtcNow.ToString("o")
            };
            return JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        }

        // carregar estado a partir de JSON (supõe formato compatível)
        public void CarregarEstadoJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("corpos", out var corposEl))
                {
                    var novos = JsonSerializer.Deserialize<List<Corpo>>(corposEl.GetRawText());
                    if (novos != null)
                    {
                        Corpos = novos;
                    }
                }

                Iteracoes = root.TryGetProperty("iteracoes", out var itEl) && itEl.TryGetInt32(out var it) ? it : 0;
                Colisoes = root.TryGetProperty("colisoes", out var colEl) && colEl.TryGetInt32(out var col) ? col : 0;
                AdicionarEvento($"Estado carregado. {Corpos.Count} corpos recuperados.");
            }
            catch (Exception ex)
            {
                AdicionarEvento($"Erro ao carregar estado: {ex.Message}");
            }
        }

        public void AdicionarEvento(string mensagem)
        {
            var hora = DateTime.Now.ToLongTimeString();
            Eventos.Insert(0, $"[{hora}] {mensagem}");
            if (Eventos.Count > 20) Eventos.RemoveAt(Eventos.Count - 1);
        }

        // For convenience in UI
        public int StatCorpos => Corpos.Count;
        public int StatIteracoes => Iteracoes;
        public int StatColisoes => Colisoes;
    }
}
