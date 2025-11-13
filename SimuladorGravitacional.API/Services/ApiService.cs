using System.Net.Http.Json;
using ProgramacaoAvancada.DTOs;

namespace ProgramacaoAvancada.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        // ✅ OBTER TODAS AS SIMULAÇÕES
        public async Task<List<SimulacaoDto>> GetSimulacoesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<SimulacaoDto>>("api/simulacao") ?? new();
            }
            catch
            {
                return new List<SimulacaoDto>();
            }
        }

        // ✅ SALVAR SIMULAÇÃO SIMPLES (gera corpos automaticamente)
        public async Task<SimulacaoDto?> SalvarSimulacaoAsync(CriarSimulacaoDto request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/simulacao", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SimulacaoDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // ✅ SALVAR SIMULAÇÃO COM CORPOS ESPECÍFICOS
        public async Task<SimulacaoDto?> SalvarSimulacaoComCorposAsync(SalvarSimulacaoComCorposDto request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/simulacao/ComCorpos", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SimulacaoDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // ✅ OBTER UMA SIMULAÇÃO POR ID
        public async Task<SimulacaoDto?> GetSimulacaoAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<SimulacaoDto>($"api/simulacao/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ✅ DELETAR SIMULAÇÃO
        public async Task<bool> DeletarSimulacaoAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/simulacao/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ✅ MÉTODO PARA CRIAR UMA NOVA SIMULAÇÃO RÁPIDA
        public async Task<SimulacaoDto?> CriarSimulacaoRapida(string nome, int quantidadeCorpos = 10)
        {
            var request = new CriarSimulacaoDto
            {
                Nome = nome,
                QuantidadeCorpos = quantidadeCorpos,
                LarguraCanvas = 800,
                AlturaCanvas = 600,
                FatorSimulacao = 1e5
            };

            return await SalvarSimulacaoAsync(request);
        }
    }

    // ✅ DTOs para uso no front-end
    public class SalvarSimulacaoComCorposDto
    {
        public string Nome { get; set; } = string.Empty;
        public string NomeUniverso { get; set; } = string.Empty;
        public double CanvasWidth { get; set; } = 800;
        public double CanvasHeight { get; set; } = 600;
        public double FatorSimulacao { get; set; } = 1e5;
        public int NumeroIteracoes { get; set; }
        public int NumeroColisoes { get; set; }
        public List<CreateCorpoDto> Corpos { get; set; } = new List<CreateCorpoDto>();
    }
}