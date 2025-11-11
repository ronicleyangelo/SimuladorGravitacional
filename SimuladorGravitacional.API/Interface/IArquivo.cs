using System.Collections.Generic;
using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Interface;

namespace ProgramacaoAvancada.Interface
{
    public interface IArquivo<T>
    {
        void Salvar(string caminho, List<T> lista, int iteracoes, double tempoEntreIteracoes);
        (List<T> lista, int iteracoes, double tempoEntreIteracoes) Carregar(string caminho);
    }
}
