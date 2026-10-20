namespace TP3Frontend.Models;

public class VentaResponse
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public List<DetalleVentaResponse> Detalles { get; set; } = new();
}

public class DetalleVentaResponse
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
