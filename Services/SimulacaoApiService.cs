using System.Net.Http.Json;
using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.DTOs;

namespace ProgramacaoAvancada.Services
{
    public class SimulacaoApiService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5001/api/simulacoes"; // ⚠️ Ajuste a porta

        public SimulacaoApiService(HttpClient http)
        {
            _http = http;
        }

        // Salvar simulação
        public async Task<SimulacaoSalva?> SalvarSimulacaoAsync(SalvarSimulacaoRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(BaseUrl, request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SimulacaoSalva>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao salvar simulação: {ex.Message}");
                return null;
            }
        }

        // Buscar por ID
        public async Task<SimulacaoSalva?> GetSimulacaoPorIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<SimulacaoSalva>($"{BaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar simulação {id}: {ex.Message}");
                return null;
            }
        }

        // Buscar recentes
        public async Task<List<SimulacaoSalva>?> GetSimulacoesRecentesAsync(int limit = 10)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<SimulacaoSalva>>($"{BaseUrl}/recentes/{limit}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao listar simulações: {ex.Message}");
                return new List<SimulacaoSalva>();
            }
        }

        // Deletar
        public async Task<bool> DeletarSimulacaoAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao deletar simulação {id}: {ex.Message}");
                return false;
            }
        }
    }

    // ✅ Classes DTO para comunicação com API
    public class SalvarSimulacaoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public int NumeroCorpos { get; set; }
        public int Iteracoes { get; set; }
        public int Colisoes { get; set; }
        public double Gravidade { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        public List<CorpoDto> Corpos { get; set; } = new();
    }

    // public class CorpoDto
    // {
    //     public string Nome { get; set; } = string.Empty;
    //     public double Massa { get; set; }
    //     public double Raio { get; set; }
    //     public double PosX { get; set; }
    //     public double PosY { get; set; }
    //     public double VelX { get; set; }
    //     public double VelY { get; set; }
    // }
}