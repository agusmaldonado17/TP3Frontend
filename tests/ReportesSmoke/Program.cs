using QuestPDF.Infrastructure;
using TP5ProgramacionMovil.Models;
using TP5ProgramacionMovil.Services;
QuestPDF.Settings.License = LicenseType.Community;
var service = new ReportePdfService();
var output = Path.GetFullPath("tmp/pdfs/updated");
Directory.CreateDirectory(output);
var productos = Enumerable.Range(1, 65).Select(i => new Producto {
    Id = i, Nombre = i % 3 == 0 ? "Aceite de girasol primera prensada presentación familiar 1,5 litros" : "Producto de prueba " + i,
    CategoriaProducto = new CategoriaProducto { Nombre = "Almacén y productos regionales" },
    PrecioVenta = 1234567.89m, StockActual = i, PuntoReposicion = 10
}).ToArray();
var ventas = Enumerable.Range(1, 65).Select(i => new Venta {
    Id = i, Fecha = new DateTime(2026, 10, 19, 14, 30, 0), Total = 1234567.89m,
    Cliente = new Cliente { Nombre = "Cliente de prueba con nombre extenso" },
    Usuario = new Usuario { Nombre = "Administrador del Sistema" }
}).ToArray();
var compras = Enumerable.Range(1, 65).Select(i => new Compra {
    Id = i, Fecha = new DateTime(2026, 10, 19, 14, 30, 0), Total = 1234567.89m,
    Proveedor = new Proveedor { RazonSocial = "Proveedor de prueba con razón social extensa S.A." },
    Usuario = new Usuario { Nombre = "Administrador del Sistema" }
}).ToArray();
File.WriteAllBytes(Path.Combine(output,"stock.pdf"),service.GenerarReporteStock(productos));
File.WriteAllBytes(Path.Combine(output,"ventas.pdf"),service.GenerarReporteVentas(ventas));
File.WriteAllBytes(Path.Combine(output,"entradas.pdf"),service.GenerarReporteEntradas(compras));
File.WriteAllBytes(Path.Combine(output,"stock-vacio.pdf"),service.GenerarReporteStock([]));
File.WriteAllBytes(Path.Combine(output,"ventas-vacio.pdf"),service.GenerarReporteVentas([]));
File.WriteAllBytes(Path.Combine(output,"entradas-vacio.pdf"),service.GenerarReporteEntradas([]));
Console.WriteLine("Generated 6 test reports with empty and multi-page fixtures.");
