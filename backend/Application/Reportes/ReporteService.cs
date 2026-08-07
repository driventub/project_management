using MapsterMapper;
using ProjectManagement.Application.Ports;

namespace ProjectManagement.Application.Reportes;

public record ReporteArchivo(byte[] Contenido, string ContentType, string NombreArchivo);

public class ReporteService
{
    private readonly IProyectoRepository _proyectoRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly IMapper _mapper;
    private readonly IReadOnlyDictionary<string, IReporteExporter> _exportadores;

    public ReporteService(
        IProyectoRepository proyectoRepository,
        ITareaRepository tareaRepository,
        IMapper mapper,
        IEnumerable<IReporteExporter> exportadores)
    {
        _proyectoRepository = proyectoRepository;
        _tareaRepository = tareaRepository;
        _mapper = mapper;
        _exportadores = exportadores.ToDictionary(e => e.Formato, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<ProyectoReporteDto?> ObtenerReporteAsync(Guid proyectoId, CancellationToken cancellationToken = default)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(proyectoId, cancellationToken);
        if (proyecto is null)
        {
            return null;
        }

        var tareas = await _tareaRepository.GetByProyectoIdAsync(proyectoId, cancellationToken);
        var tareasOrdenadas = tareas.OrderBy(t => t.Columna.Orden).ThenBy(t => t.Orden);

        return new ProyectoReporteDto(
            proyecto.Id,
            proyecto.Nombre,
            proyecto.Descripcion,
            proyecto.FechaInicio,
            proyecto.FechaFinEsperada,
            proyecto.Estado.ToString(),
            DateTime.UtcNow,
            _mapper.Map<List<TareaReporteItemDto>>(tareasOrdenadas));
    }

    public async Task<ReporteArchivo?> ExportarAsync(Guid proyectoId, string formato, CancellationToken cancellationToken = default)
    {
        if (!_exportadores.TryGetValue(formato, out var exportador))
        {
            throw new ArgumentException($"Formato '{formato}' no soportado.");
        }

        var reporte = await ObtenerReporteAsync(proyectoId, cancellationToken);
        if (reporte is null)
        {
            return null;
        }

        var contenido = exportador.Exportar(reporte);
        var nombreArchivo = $"reporte-{Slugify(reporte.Nombre)}-{DateTime.UtcNow:yyyyMMdd}.{exportador.ExtensionArchivo}";

        return new ReporteArchivo(contenido, exportador.ContentType, nombreArchivo);
    }

    private static string Slugify(string valor)
    {
        var normalizado = valor.Trim().ToLowerInvariant();
        var caracteres = normalizado.Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        var slug = new string(caracteres);

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }
}
