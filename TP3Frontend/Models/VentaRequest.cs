namespace TP3Frontend.Models
{
    public class VentaRequest
    {
        public int ClienteId { get; set; }

        public List<DetalleVentaRequest> Detalles { get; set; } = new();
    }

    public class DetalleVentaRequest
    {
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }
    }
}