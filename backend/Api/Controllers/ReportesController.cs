using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Reportes;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/proyectos/{proyectoId:guid}/reportes")]
public class ReportesController : ControllerBase
{
    private readonly ReporteService _reporteService;

    public ReportesController(ReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet("{formato}")]
    public async Task<IActionResult> Exportar(Guid proyectoId, string formato, CancellationToken cancellationToken)
    {
        try
        {
            var archivo = await _reporteService.ExportarAsync(proyectoId, formato, cancellationToken);
            return archivo is null ? NotFound() : File(archivo.Contenido, archivo.ContentType, archivo.NombreArchivo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
