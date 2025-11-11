// Services/ApiService.cs
using System.Net.Http.Json;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        // ✅ MÉTODOS EXISTENTES
        public async Task<List<SimulacaoSnapshot>> GetSimulacoesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<SimulacaoSnapshot>>("api/simulacao") ?? new();
            }
            catch
            {
                return new List<SimulacaoSnapshot>();
            }
        }

        public async Task<bool> SalvarSimulacaoAsync(SimulacaoSalvarRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/simulacao", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SimulacaoSalvarRequest?> CarregarSimulacaoAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<SimulacaoSalvarRequest>($"api/simulacao/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ✅ NOVO MÉTODO: Deletar simulação
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
    }
}