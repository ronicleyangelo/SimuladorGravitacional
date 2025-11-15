using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Data;
using ProgramacaoAvancada.Models;
using System;
using System.Threading.Tasks;

namespace ProgramacaoAvancada.Services
{
    public class DatabaseService
    {
        private readonly SimulacaoDbContext _context;

        public DatabaseService(SimulacaoDbContext context)
        {
            _context = context;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                Console.WriteLine("Banco de dados inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        // ✅ REMOVER ESTE MÉTODO (DbPath não existe mais)
        // public string GetDatabasePath()
        // {
        //     return _context.DbPath;
        // }
    }
}