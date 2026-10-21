using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TP3Frontend;
using TP3Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://tp3backend.runasp.net/")
    });

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IReporteService, ReporteService>();

await builder.Build().RunAsync();
