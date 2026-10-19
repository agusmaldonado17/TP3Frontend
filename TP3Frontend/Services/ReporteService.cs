using System.Net.Http.Json;
using TP3Frontend.Models;

namespace TP3Frontend.Services
{
    public interface IReporteService
    {
        Task<byte[]?> GetStockPdfAsync();

        Task<byte[]?> GetVentasPdfAsync();

        Task<byte[]?> GetEntradasPdfAsync();

        Task<bool> EnviarReporteAsync(
            string email,
            string tipoReporte);
    }


    public class ReporteService : IReporteService
    {
        private readonly ILogger<ReporteService> _logger;
        private readonly HttpClient _http;

        public ReporteService(
            ILogger<ReporteService> logger,
            HttpClient http)
        {
            _logger = logger;
            _http = http;
        }


        public Task<byte[]?> GetStockPdfAsync()
        {
            return DescargarAsync(
                "api/Reportes/stock"
            );
        }


        public Task<byte[]?> GetVentasPdfAsync()
        {
            return DescargarAsync(
                "api/Reportes/ventas"
            );
        }


        public Task<byte[]?> GetEntradasPdfAsync()
        {
            return DescargarAsync(
                "api/Reportes/entradas"
            );
        }


        public async Task<bool> EnviarReporteAsync(
            string email,
            string tipoReporte)
        {
            try
            {
                var request = new EnviarReporteRequest
                {
                    Email = email,
                    TipoReporte = tipoReporte
                };

                var response =
                    await _http.PostAsJsonAsync(
                        "api/Reportes/enviar",
                        request
                    );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al enviar el reporte {TipoReporte}",
                    tipoReporte
                );

                return false;
            }
        }


        private async Task<byte[]?> DescargarAsync(
            string url)
        {
            try
            {
                using var response =
                    await _http.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return await response.Content
                    .ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al descargar el reporte {Url}",
                    url
                );

                return null;
            }
        }
    }
}