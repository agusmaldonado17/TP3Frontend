namespace TP3Frontend.Models
{
    public class CompraRequest
    {
        public int ProveedorId { get; set; }

        public List<DetalleCompraRequest> Detalles { get; set; } = new();
    }

    public class DetalleCompraRequest
    {
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
}