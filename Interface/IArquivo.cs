using System.Collections.Generic;

namespace ProgramacaoAvancada.Interface
{
    public interface IArquivo<T>
    {
        void Salvar(string caminho, List<T> lista, int iteracoes, double tempoEntreIteracoes);
        (List<T> lista, int iteracoes, double tempoEntreIteracoes) Carregar(string caminho);
    }
}
