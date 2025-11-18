using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProgramacaoAvancada;
using ProgramacaoAvancada.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ HttpClient para API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5001") // URL da sua API
});

// ✅ Serviços
builder.Services.AddScoped<SimulacaoApiService>();
builder.Services.AddScoped<SimuladorService>();

await builder.Build().RunAsync();