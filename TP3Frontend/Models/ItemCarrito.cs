namespace TP3Frontend.Models
{
    public class ItemCarrito
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public int StockDisponible { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}