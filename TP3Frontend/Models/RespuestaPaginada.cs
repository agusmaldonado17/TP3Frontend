namespace TP3Frontend.Models
{
    public class RespuestaPaginada<T>
    {
        public List<T> Datos { get; set; } = new();

        public int TotalRegistros { get; set; }

        public int PaginaActual { get; set; }

        public int TamanoPagina { get; set; }

        public int TotalPaginas { get; set; }
    }
}