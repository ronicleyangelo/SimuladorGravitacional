using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Interface
{
    public interface IArquivo<T>
    {
        string GerarConteudoArquivo(List<T> objetos, string cabecalho = "", int iteracoes = 0, double deltaTime = 0.016);
        (List<T> objetos, int iteracoes, double deltaTime) CarregarDeConteudo(string conteudo);
    }
}