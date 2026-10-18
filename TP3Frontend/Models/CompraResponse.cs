namespace TP3Frontend.Models
{
    public class CompraResponse
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Total { get; set; }

        public int ProveedorId { get; set; }

        public string ProveedorNombre { get; set; } = string.Empty;

        public int UsuarioId { get; set; }

        public string UsuarioNombre { get; set; } = string.Empty;

        public List<DetalleCompraResponse> Detalles { get; set; } = new();
    }

    public class DetalleCompraResponse
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}