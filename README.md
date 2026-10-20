# TP3Frontend

Frontend de punto de venta desarrollado con Blazor WebAssembly y .NET 10.

## Ejecutar

```sh
dotnet restore
dotnet run --project TP3Frontend --launch-profile http
```

La aplicación se abre en `http://localhost:5101`. Las operaciones autenticadas requieren el backend ya configurado por el proyecto.

## Organización de estilos

- `TP3Frontend/wwwroot/css/theme.css`: variables de diseño, tipografía, controles, tablas, tarjetas y patrones compartidos entre pantallas. Se carga después de Bootstrap a través de `app.css`.
- `TP3Frontend/wwwroot/css/app.css`: estilos de carga y errores de Blazor.
- `TP3Frontend/Layout/MainLayout.razor.css`: estructura general, cabecera y adaptación a móvil.
- `TP3Frontend/Layout/NavMenu.razor.css`: navegación, selección activa y panel de usuario.
- `TP3Frontend/Pages/Login.razor.css`: composición e ilustración decorativa exclusivas del acceso.

La paleta está centralizada en las variables de `:root`. Para ajustar colores, modificar esas variables; para una nueva pantalla, reutilizar `page-heading`, `surface-panel`, `workspace-grid` y `table-responsive`. Los estilos exclusivos de una página deben ir en su archivo `.razor.css`.

El diseño toma como referencia [Sales Analytics Dashboard, de Mahid](https://dribbble.com/shots/24613273-Sales-Analytics-Dashboard): superficies claras, acento lima y tarjetas lavanda, azul y menta. No requiere fuentes, imágenes ni paquetes adicionales.

## Verificación

```sh
dotnet build --no-restore
git diff --check
```

Para una revisión visual completa con el backend disponible: comprobar inicio, venta con carrito vacío y cargado, ingreso de mercadería, historial expandido y reportes; revisar también el menú móvil y el desplazamiento horizontal de tablas. Mantener las condiciones de roles, los enlaces, los eventos y los bloques `@code` al modificar la presentación.

## Historial de ventas y reportes

El historial está en `/ventas`, disponible para Admin y Vendedor desde el menú y el inicio. Usa `GET api/Ventas` con paginación de 20 registros, búsqueda por cliente y ordenamiento del servidor. El detalle usa los campos reales `NombreCliente`, `NombreUsuario` y `Detalles.NombreProducto` del backend. Los filtros se aplican al pulsar Buscar y se conservan al cambiar de página.

Los PDF se generan en `TP5Backend/TP5ProgramacionMovil/Services/ReportePdfService.cs`. Ese generador es compartido por descarga y correo; reiniciar el backend después de modificarlo. `wwwroot/js/downloads.js` verifica la firma PDF, conserva los bytes binarios y retrasa la liberación de la URL temporal para permitir que el navegador termine de iniciar la descarga.

Pruebas sin base de datos ni envíos de correo:

```sh
node --test tests/downloads.test.cjs
dotnet run --project tests/VentasSmoke/VentasSmoke.csproj
dotnet run --project tests/ReportesSmoke/ReportesSmoke.csproj
```

`VentasSmoke` verifica renderizado, detalle, búsqueda con caracteres especiales, ordenamiento, paginación y estados de error con HTTP simulado. `ReportesSmoke` necesita `TP5Backend` como carpeta hermana y genera seis PDF de prueba en `tmp/pdfs/updated`: stock, ventas y entradas con listas vacías y 65 registros de nombres largos. Son datos ficticios, exclusivamente para validación visual.
