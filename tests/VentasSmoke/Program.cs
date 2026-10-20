using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using TP3Frontend.Pages;
using TP3Frontend.Services;

var handler = new FixtureHandler();
var activator = new CaptureActivator();
var services = new ServiceCollection();
services.AddLogging();
services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("https://test.invalid/") });
services.AddSingleton<IJSRuntime, FakeJs>();
services.AddSingleton<NavigationManager, FakeNavigation>();
services.AddSingleton<AuthService>();
services.AddSingleton<IComponentActivator>(activator);
using var provider = services.BuildServiceProvider();
await provider.GetRequiredService<AuthService>().LoginAsync("fixture", "fixture");
await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
await renderer.Dispatcher.InvokeAsync(async () =>
{
    var root = await renderer.RenderComponentAsync<Ventas>();
    Check(root.ToHtmlString().Contains("Cliente de prueba"), "Initial API data renders");
    Check(handler.LastQuery.Contains("pagina=1"), "Initial page is 1");
    var component = activator.Instance!;
    await Invoke("AlternarDetalle", 7);
    Check(root.ToHtmlString().Contains("Arroz largo fino"), "Expanded detail renders product");
    Set("busqueda", "María & Hijos");
    Set("orden", "total_desc");
    await Invoke("Buscar");
    Check(handler.LastQuery.Contains("buscar=Mar%C3%ADa%20%26%20Hijos"), "Search is URL encoded");
    Check(handler.LastQuery.Contains("ordenarPor=total_desc"), "Selected order is sent");
    await Invoke("CargarVentas", 2);
    Check(handler.LastQuery.Contains("pagina=2"), "Second page is requested");
    Check(root.ToHtmlString().Contains("Página 2 de 2") || root.ToHtmlString().Contains("P&#xE1;gina 2 de 2"), "Pagination is displayed");
    handler.Status = HttpStatusCode.InternalServerError;
    await Invoke("CargarVentas", 1);
    Check(root.ToHtmlString().Contains("Reintentar"), "API failure offers retry");
    handler.Status = HttpStatusCode.OK;
    handler.Empty = true;
    await Invoke("CargarVentas", 1);
    Check(root.ToHtmlString().Contains("No hay ventas"), "Empty state renders");
    handler.Status = HttpStatusCode.Unauthorized;
    await Invoke("CargarVentas", 1);
    Check(root.ToHtmlString().Contains("nuevamente"), "Expired session is explained");
    handler.Status = HttpStatusCode.Forbidden;
    await Invoke("CargarVentas", 1);
    Check(((FakeNavigation)provider.GetRequiredService<NavigationManager>()).Destination == "/acceso-denegado", "Forbidden routes to access denied");

    void Set(string name, object value) => typeof(Ventas).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(component, value);
    async Task Invoke(string name, params object[] args)
    {
        var result = typeof(Ventas).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(component, args);
        if (result is Task task) await task;
        typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(component, null);
    }
});
Console.WriteLine("Sales history checks passed using isolated HTTP fixtures; no real API was called.");
static void Check(bool condition, string message) { if (!condition) throw new Exception(message); Console.WriteLine("PASS " + message); }

class CaptureActivator : IComponentActivator
{
    public Ventas? Instance { get; private set; }
    public IComponent CreateInstance(Type type)
    {
        var component = (IComponent)Activator.CreateInstance(type)!;
        if (component is Ventas ventas) Instance = ventas;
        return component;
    }
}
class FakeNavigation : NavigationManager
{
    public string? Destination;
    public FakeNavigation() => Initialize("https://test.invalid/", "https://test.invalid/ventas");
    protected override void NavigateToCore(string uri, bool forceLoad) => Destination = uri;
}
class FakeJs : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => ValueTask.FromResult(default(TValue)!);
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
}
class FixtureHandler : HttpMessageHandler
{
    public string LastQuery = "";
    public HttpStatusCode Status = HttpStatusCode.OK;
    public bool Empty;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method == HttpMethod.Post)
        {
            var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("{\"exp\":4102444800}"));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new { Id=1, Nombre="Prueba", Username="fixture", Rol="Admin", Token="test."+payload+".fixture" }) });
        }
        LastQuery = request.RequestUri!.Query;
        var data = Empty ? "[]" : """
        [{"id":7,"fecha":"2026-10-19T14:30:00","total":2800,"nombreCliente":"Cliente de prueba","nombreUsuario":"Cajero","detalles":[{"productoId":1,"nombreProducto":"Arroz largo fino","cantidad":1,"precioUnitario":2800,"subtotal":2800}]}]
        """;
        return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent("{\"datos\":"+data+",\"totalRegistros\":21,\"totalPaginas\":2,\"paginaActual\":1,\"tamanoPagina\":20}", System.Text.Encoding.UTF8, "application/json") });
    }
}
