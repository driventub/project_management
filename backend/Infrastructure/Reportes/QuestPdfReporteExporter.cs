using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProjectManagement.Infrastructure.Reportes;

public class QuestPdfReporteExporter : IReporteExporter
{
    public string Formato => "pdf";
    public string ContentType => "application/pdf";
    public string ExtensionArchivo => "pdf";

    public byte[] Exportar(ProyectoReporteDto reporte)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text(reporte.Nombre).FontSize(18).Bold();
                    column.Item().PaddingTop(4).Text(reporte.Descripcion);
                    column.Item().PaddingTop(4).Text(
                        $"Estado: {reporte.Estado}    Inicio: {reporte.FechaInicio:yyyy-MM-dd}    Fin esperado: {reporte.FechaFinEsperada:yyyy-MM-dd}");
                    column.Item().PaddingTop(2).Text($"Generado: {reporte.FechaGeneracion:yyyy-MM-dd HH:mm} UTC")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Tarea").Bold();
                        header.Cell().Text("Columna").Bold();
                        header.Cell().Text("Responsable").Bold();
                        header.Cell().Text("Prioridad").Bold();

                        header.Cell().ColumnSpan(4).PaddingTop(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    });

                    foreach (var tarea in reporte.Tareas)
                    {
                        table.Cell().PaddingVertical(3).Text(tarea.Titulo);
                        table.Cell().PaddingVertical(3).Text(tarea.ColumnaNombre);
                        table.Cell().PaddingVertical(3).Text(tarea.ResponsableNombre ?? "Sin asignar");
                        table.Cell().PaddingVertical(3).Text(tarea.Prioridad);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
