using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProgramacaoAvancada;
using ProgramacaoAvancada.Services;
using ProgramacaoAvancada.Interface;
using ProgramacaoAvancada.Models;
using ProgramacaoAvancada.Arquivos;
using ProgramacaoAvancada.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddScoped<IArquivo<Corpo>, Arquivo>();

// ✅ CONFIGURAR O DbContextFactory
builder.Services.AddDbContextFactory<SimulacaoDbContext>(options =>
{
    // A connection string será usada do OnConfiguring no DbContext
    // Não precisa repetir aqui, o EF vai usar a do seu DbContext
    options.UseNpgsql(); // Vazio - pega do OnConfiguring
});

builder.Services.AddScoped<SimuladorService>();

var app = builder.Build();

// ✅ INICIALIZAR E CRIAR TABELAS AUTOMATICAMENTE
try
{
    // Criar scope para acessar serviços
    await using var scope = app.Services.CreateAsyncScope();

    // Obter factory do DbContext
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SimulacaoDbContext>>();

    // Criar contexto
    await using var context = await dbFactory.CreateDbContextAsync();

    // ✅ ESTE COMANDO CRIA AS TABELAS NO NEON!
    var created = await context.Database.EnsureCreatedAsync();

    if (created)
    {
        Console.WriteLine("✅ TABELAS CRIADAS COM SUCESSO NO NEON DATABASE!");
    }
    else
    {
        Console.WriteLine("ℹ️  As tabelas já existiam no banco.");
    }

    // Testar conexão
    var canConnect = await context.Database.CanConnectAsync();
    if (canConnect)
    {
        Console.WriteLine("✅ CONEXÃO COM NEON ESTABELECIDA!");

        // Contar universos existentes
        var universoCount = await context.Universos.CountAsync();
        Console.WriteLine($"📊 Universos no banco: {universoCount}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERRO AO CRIAR TABELAS: {ex.Message}");
    Console.WriteLine($"🔍 Detalhes: {ex.InnerException?.Message}");
}

await app.RunAsync();