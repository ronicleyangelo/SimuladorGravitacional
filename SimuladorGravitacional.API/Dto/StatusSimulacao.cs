namespace ProgramacaoAvancada.DTOs
{
    /// <summary>
    /// Representa os possíveis estados de uma simulação
    /// </summary>
    public enum StatusSimulacao
    {
        /// <summary>
        /// Simulação está em rascunho/editável
        /// </summary>
        Rascunho = 0,

        /// <summary>
        /// Simulação está executando
        /// </summary>
        Executando = 1,

        /// <summary>
        /// Simulação está pausada
        /// </summary>
        Pausada = 2,

        /// <summary>
        /// Simulação foi concluída com sucesso
        /// </summary>
        Concluida = 3,

        /// <summary>
        /// Simulação foi parada devido a um erro
        /// </summary>
        Erro = 4,

        /// <summary>
        /// Simulação foi cancelada pelo usuário
        /// </summary>
        Cancelada = 5
    }

    /// <summary>
    /// Tipos de corpos celestes suportados na simulação
    /// </summary>
    public enum TipoCorpoCeleste
    {
        /// <summary>
        /// Corpo estelar (estrela)
        /// </summary>
        Estrela = 0,

        /// <summary>
        /// Planeta
        /// </summary>
        Planeta = 1,

        /// <summary>
        /// Satélite natural (lua)
        /// </summary>
        Lua = 2,

        /// <summary>
        /// Planeta anão
        /// </summary>
        PlanetaAnao = 3,

        /// <summary>
        /// Asteroide
        /// </summary>
        Asteroide = 4,

        /// <summary>
        /// Cometa
        /// </summary>
        Cometa = 5,

        /// <summary>
        /// Buraco negro
        /// </summary>
        BuracoNegro = 6,

        /// <summary>
        /// Outro tipo de corpo celeste
        /// </summary>
        Outro = 7
    }

    /// <summary>
    /// Níveis de severidade para eventos da simulação
    /// </summary>
    public enum NivelEvento
    {
        /// <summary>
        /// Evento informativo
        /// </summary>
        Informacao = 0,

        /// <summary>
        /// Aviso
        /// </summary>
        Aviso = 1,

        /// <summary>
        /// Erro
        /// </summary>
        Erro = 2,

        /// <summary>
        /// Evento crítico
        /// </summary>
        Critico = 3
    }
}