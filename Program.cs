using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProgramacaoAvancada;
using ProgramacaoAvancada.Services;
using ProgramacaoAvancada.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ HttpClient apontando para API
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("https://localhost:7056/") 
});

// ✅ Registrar serviços (sem IArquivo)
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<SimuladorService>();

await builder.Build().RunAsync();