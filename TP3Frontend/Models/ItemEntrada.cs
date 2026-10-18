namespace TP3Frontend.Models
{
    public class ItemEntrada
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Cantidad { get; set; } = 1;

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}