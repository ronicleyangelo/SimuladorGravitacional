using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.DTOs;
using System.Globalization;
using System.Text;

namespace ProgramacaoAvancada.Arquivos
{
    public class Arquivo
    {
        private readonly CultureInfo _cultura = CultureInfo.InvariantCulture;

        // Método para salvar uma simulação completa
        public void SalvarSimulacao(string caminho, SimulacaoDto simulacao)
        {
            var sb = new StringBuilder();
            
            // Cabeçalho com informações da simulação
            sb.AppendLine($"# Simulação: {simulacao.Nome}");
            sb.AppendLine($"# Data: {simulacao.DataCriacao:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"# Iterações: {simulacao.NumeroIteracoes}");
            sb.AppendLine($"# Colisões: {simulacao.NumeroColisoes}");
            sb.AppendLine($"# Corpos: {simulacao.QuantidadeCorpos}");
            sb.AppendLine($"# Universo: {simulacao.Universo.Nome}");
            sb.AppendLine($"# Dimensões: {simulacao.Universo.CanvasWidth}x{simulacao.Universo.CanvasHeight}");
            sb.AppendLine($"# Fator Simulação: {simulacao.Universo.FatorSimulacao}");

            // Dados dos corpos
            sb.AppendLine("NOME;MASSA;DENSIDADE;RAIO;POSX;POSY;VelocidadeX;VelocidadeY;COR");

            foreach (var corpo in simulacao.Universo.Corpos)
            {
                sb.AppendLine(string.Join(";",
                    corpo.Nome,
                    corpo.Massa.ToString(_cultura),
                    corpo.Densidade.ToString(_cultura),
                    corpo.Raio.ToString(_cultura),
                    corpo.PosicaoX.ToString(_cultura),
                    corpo.PosicaoY.ToString(_cultura),
                    corpo.VelocidadeX.ToString(_cultura),
                    corpo.VelocidadeY.ToString(_cultura),
                    corpo.Cor
                ));
            }

            var dir = Path.GetDirectoryName(caminho);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(caminho, sb.ToString());
        }

        // Método para carregar uma simulação do arquivo
        public SimulacaoDto CarregarSimulacao(string caminho)
        {
            if (!File.Exists(caminho))
                throw new FileNotFoundException("Arquivo não encontrado");

            var linhas = File.ReadAllLines(caminho);
            var corpos = new List<CorpoDto>();
            var simulacaoInfo = new Dictionary<string, string>();

            foreach (var linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;

                // Linhas de comentário/metadados
                if (linha.StartsWith("#"))
                {
                    var partesMeta = linha.Substring(1).Split(':');
                    if (partesMeta.Length >= 2)
                    {
                        simulacaoInfo[partesMeta[0].Trim()] = partesMeta[1].Trim();
                    }
                    continue;
                }

                // Pular cabeçalho dos dados
                if (linha.StartsWith("NOME;")) continue;

                // Processar dados dos corpos
                var partes = linha.Split(';');
                if (partes.Length >= 9)
                {
                    try
                    {
                        var corpo = new CorpoDto
                        {
                            Nome = partes[0],
                            Massa = double.Parse(partes[1], _cultura),
                            Densidade = double.Parse(partes[2], _cultura),
                            Raio = double.Parse(partes[3], _cultura),
                            PosicaoX = double.Parse(partes[4], _cultura),
                            PosicaoY = double.Parse(partes[5], _cultura),
                            VelocidadeX = double.Parse(partes[6], _cultura),
                            VelocidadeY = double.Parse(partes[7], _cultura),
                            Cor = partes[8]
                        };
                        corpos.Add(corpo);
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException($"Erro ao processar linha: {linha}", ex);
                    }
                }
            }
            // Criar DTO da simulação
            return new SimulacaoDto
            {
                Nome = simulacaoInfo.GetValueOrDefault("Simulação", "Simulação Carregada"),
                DataCriacao = DateTime.TryParse(simulacaoInfo.GetValueOrDefault("Data"), out var data) 
                    ? data : DateTime.UtcNow,
                NumeroIteracoes = int.TryParse(simulacaoInfo.GetValueOrDefault("Iterações"), out var iteracoes) 
                    ? iteracoes : 0,
                NumeroColisoes = int.TryParse(simulacaoInfo.GetValueOrDefault("Colisões"), out var colisoes) 
                    ? colisoes : 0,
                QuantidadeCorpos = corpos.Count,
                Universo = new UniversoDto
                {
                    Nome = simulacaoInfo.GetValueOrDefault("Universo", "Universo Carregado"),
                    CanvasWidth = double.TryParse(simulacaoInfo.GetValueOrDefault("Dimensões")?.Split('x')[0], out var width) 
                        ? width : 800,
                    CanvasHeight = double.TryParse(simulacaoInfo.GetValueOrDefault("Dimensões")?.Split('x')[1], out var height) 
                        ? height : 600,
                    FatorSimulacao = double.TryParse(simulacaoInfo.GetValueOrDefault("Fator Simulação"), out var fator) 
                        ? fator : 1e5,
                    Corpos = corpos
                }
            };
        }

        // Método para exportar apenas os corpos (formato simples)
        public string ExportarCorpos(List<CorpoDto> corpos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("NOME;MASSA;DENSIDADE;RAIO;POSX;POSY;VelocidadeX;VelocidadeY;COR");

            foreach (var corpo in corpos)
            {
                sb.AppendLine(string.Join(";",
                    corpo.Nome,
                    corpo.Massa.ToString(_cultura),
                    corpo.Densidade.ToString(_cultura),
                    corpo.Raio.ToString(_cultura),
                    corpo.PosicaoX.ToString(_cultura),
                    corpo.PosicaoY.ToString(_cultura),
                    corpo.VelocidadeX.ToString(_cultura),
                    corpo.VelocidadeY.ToString(_cultura),
                    corpo.Cor
                ));
            }

            return sb.ToString();
        }

        // Método para importar corpos do formato simples
        public List<CorpoDto> ImportarCorpos(string conteudo)
        {
            var corpos = new List<CorpoDto>();
            var linhas = conteudo.Split('\n');

            foreach (var linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha) || linha.StartsWith("NOME;"))
                    continue;

                var partes = linha.Split(';');
                if (partes.Length >= 9)
                {
                    try
                    {
                        var corpo = new CorpoDto
                        {
                            Nome = partes[0],
                            Massa = double.Parse(partes[1], _cultura),
                            Densidade = double.Parse(partes[2], _cultura),
                            Raio = double.Parse(partes[3], _cultura),
                            PosicaoX = double.Parse(partes[4], _cultura),
                            PosicaoY = double.Parse(partes[5], _cultura),
                            VelocidadeX = double.Parse(partes[6], _cultura),
                            VelocidadeY = double.Parse(partes[7], _cultura),
                            Cor = partes[8]
                        };
                        corpos.Add(corpo);
                    }
                    catch
                    {
                        // Ignora linhas com erro
                        continue;
                    }
                }
            }

            return corpos;
        }
    }
}