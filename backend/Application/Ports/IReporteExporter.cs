using ProjectManagement.Application.Reportes;

namespace ProjectManagement.Application.Ports;

public interface IReporteExporter
{
    string Formato { get; }
    string ContentType { get; }
    string ExtensionArchivo { get; }
    byte[] Exportar(ProyectoReporteDto reporte);
}
