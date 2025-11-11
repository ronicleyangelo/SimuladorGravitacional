using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Interface;
using System.Globalization;
using System.Text;

namespace ProgramacaoAvancada.Arquivos
{
    public class Arquivo : IArquivo<Corpo>
    {
        private readonly CultureInfo _cultura = CultureInfo.InvariantCulture;

        public void Salvar(string caminho, List<Corpo> lista, int iteracoes, double tempoEntreIteracoes)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{lista.Count};{iteracoes};{tempoEntreIteracoes.ToString(_cultura)}");

            foreach (var c in lista)
            {
                sb.AppendLine(string.Join(";",
                    c.Nome,
                    c.Massa.ToString(_cultura),
                    c.Raio.ToString(_cultura),
                    c.PosX.ToString(_cultura),
                    c.PosY.ToString(_cultura),
                    c.VelX.ToString(_cultura),
                    c.VelY.ToString(_cultura)
                ));
            }

            var dir = Path.GetDirectoryName(caminho);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(caminho, sb.ToString());
        }

        public (List<Corpo> lista, int iteracoes, double tempoEntreIteracoes) Carregar(string caminho)
        {
            var lista = new List<Corpo>();
            int iteracoes = 0;
            double tempo = 0.016;

            if (!File.Exists(caminho)) return (lista, iteracoes, tempo);

            var linhas = File.ReadAllLines(caminho);

            if (linhas.Length == 0) return (lista, iteracoes, tempo);

            var header = linhas[0].Split(';');
            if (header.Length == 3)
            {
                int.TryParse(header[1], out iteracoes);
                double.TryParse(header[2], NumberStyles.Any, _cultura, out tempo);
            }

            for (int i = 1; i < linhas.Length; i++)
            {
                var partes = linhas[i].Split(';');
                if (partes.Length < 7) continue;

                try
                {
                    var nome = partes[0];
                    var massa = double.Parse(partes[1], _cultura);
                    var raio = double.Parse(partes[2], _cultura);
                    var posX = double.Parse(partes[3], _cultura);
                    var posY = double.Parse(partes[4], _cultura);
                    var velX = double.Parse(partes[5], _cultura);
                    var velY = double.Parse(partes[6], _cultura);

                    var densidade = massa / ((4.0 / 3.0) * Math.PI * Math.Pow(raio, 3));

                    var corpo = new Corpo(nome, massa, densidade, posX, posY, "rgb(255,255,255)")
                    {
                        VelX = velX,
                        VelY = velY
                    };

                    lista.Add(corpo);
                }
                catch
                {
                    continue;
                }
            }

            return (lista, iteracoes, tempo);
        }

        // ✅ (Opcional) Método adicional para gerar conteúdo em string
        // Método melhorado
        // ✅ CORREÇÃO: Método com ordem de parâmetros correta
        public string GerarConteudoArquivo(List<Corpo> lista, string cabecalho, int iteracoes, double tempoEntreIteracoes)
        {
            var sb = new StringBuilder();

            // Adiciona cabeçalho personalizado se fornecido
            if (!string.IsNullOrEmpty(cabecalho))
            {
                sb.AppendLine($"# {cabecalho}");
            }

            sb.AppendLine($"{lista.Count};{iteracoes};{tempoEntreIteracoes.ToString(_cultura)}");

            foreach (var c in lista)
            {
                sb.AppendLine(string.Join(";",
                    c.Nome,
                    c.Massa.ToString(_cultura),
                    c.Raio.ToString(_cultura),
                    c.PosX.ToString(_cultura),
                    c.PosY.ToString(_cultura),
                    c.VelX.ToString(_cultura),
                    c.VelY.ToString(_cultura)
                ));
            }

            return sb.ToString();
        }
        public (List<Corpo>, int, double) CarregarDeConteudo(string conteudo)
        {
            var lista = new List<Corpo>();
            int iteracoes = 0;
            double tempo = 0.016;

            var linhas = conteudo.Split('\n');

            if (linhas.Length == 0) return (lista, iteracoes, tempo);

            var header = linhas[0].Trim().Split(';');
            if (header.Length == 3)
            {
                int.TryParse(header[1], out iteracoes);
                double.TryParse(header[2], NumberStyles.Any, _cultura, out tempo);
            }

            for (int i = 1; i < linhas.Length; i++)
            {
                var partes = linhas[i].Trim().Split(';');
                if (partes.Length < 7) continue;

                try
                {
                    var nome = partes[0];
                    var massa = double.Parse(partes[1], _cultura);
                    var raio = double.Parse(partes[2], _cultura);
                    var posX = double.Parse(partes[3], _cultura);
                    var posY = double.Parse(partes[4], _cultura);
                    var velX = double.Parse(partes[5], _cultura);
                    var velY = double.Parse(partes[6], _cultura);

                    var densidade = massa / ((4.0 / 3.0) * Math.PI * Math.Pow(raio, 3));

                    var corpo = new Corpo(nome, massa, densidade, posX, posY, "rgb(255,255,255)")
                    {
                        VelX = velX,
                        VelY = velY
                    };

                    lista.Add(corpo);
                }
                catch
                {
                    continue;
                }
            }

            return (lista, iteracoes, tempo);
        }
    }
}
