using ClosedXML.Excel;
using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Reportes;

namespace ProjectManagement.Infrastructure.Reportes;

public class ClosedXmlReporteExporter : IReporteExporter
{
    public string Formato => "xlsx";
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string ExtensionArchivo => "xlsx";

    public byte[] Exportar(ProyectoReporteDto reporte)
    {
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Reporte");

        hoja.Cell(1, 1).Value = reporte.Nombre;
        hoja.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);

        hoja.Cell(2, 1).Value = reporte.Descripcion;

        hoja.Cell(3, 1).Value = $"Estado: {reporte.Estado}";
        hoja.Cell(3, 2).Value = $"Inicio: {reporte.FechaInicio}";
        hoja.Cell(3, 3).Value = $"Fin esperado: {reporte.FechaFinEsperada}";

        hoja.Cell(4, 1).Value = $"Generado: {reporte.FechaGeneracion:yyyy-MM-dd HH:mm} UTC";
        hoja.Cell(4, 1).Style.Font.FontColor = XLColor.Gray;
        hoja.Cell(4, 1).Style.Font.FontSize = 9;

        const int filaEncabezado = 6;
        var encabezados = new[] { "Tarea", "Columna", "Responsable", "Prioridad" };
        for (var i = 0; i < encabezados.Length; i++)
        {
            var celda = hoja.Cell(filaEncabezado, i + 1);
            celda.Value = encabezados[i];
            celda.Style.Font.SetBold();
            celda.Style.Fill.SetBackgroundColor(XLColor.LightGray);
        }

        var fila = filaEncabezado + 1;
        foreach (var tarea in reporte.Tareas)
        {
            hoja.Cell(fila, 1).Value = tarea.Titulo;
            hoja.Cell(fila, 2).Value = tarea.ColumnaNombre;
            hoja.Cell(fila, 3).Value = tarea.ResponsableNombre ?? "Sin asignar";
            hoja.Cell(fila, 4).Value = tarea.Prioridad;
            fila++;
        }

        hoja.Columns(1, encabezados.Length).AdjustToContents();
        foreach (var columna in hoja.Columns(1, encabezados.Length))
        {
            if (columna.Width < 12)
            {
                columna.Width = 12;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
