using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Columnas;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/proyectos/{proyectoId:guid}/columnas")]
public class ColumnasController : ControllerBase
{
    private readonly ColumnaService _columnaService;

    public ColumnasController(ColumnaService columnaService)
    {
        _columnaService = columnaService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ColumnaDto>>> GetByProyecto(Guid proyectoId, CancellationToken cancellationToken)
    {
        var columnas = await _columnaService.GetByProyectoIdAsync(proyectoId, cancellationToken);
        return Ok(columnas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ColumnaDto>> GetById(Guid proyectoId, Guid id, CancellationToken cancellationToken)
    {
        var columna = await _columnaService.GetByIdAsync(id, cancellationToken);
        return columna is null || columna.ProyectoId != proyectoId ? NotFound() : Ok(columna);
    }

    [HttpPost]
    public async Task<ActionResult<ColumnaDto>> Create(Guid proyectoId, [FromBody] ColumnaRequest request, CancellationToken cancellationToken)
    {
        var created = await _columnaService.CreateAsync(proyectoId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ColumnaDto>> Update(Guid proyectoId, Guid id, [FromBody] ColumnaRequest request, CancellationToken cancellationToken)
    {
        var updated = await _columnaService.UpdateAsync(id, request, cancellationToken);
        return updated is null || updated.ProyectoId != proyectoId ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid proyectoId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _columnaService.DeleteAsync(id, cancellationToken);
        return result switch
        {
            ColumnaDeleteResult.NotFound => NotFound(),
            ColumnaDeleteResult.TieneTareas => Conflict(new { message = "No se puede eliminar una columna que contiene tareas." }),
            _ => NoContent()
        };
    }

    [HttpPut("reorder")]
    public async Task<ActionResult<IReadOnlyList<ColumnaDto>>> Reorder(Guid proyectoId, [FromBody] ReorderColumnasRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var columnas = await _columnaService.ReorderAsync(proyectoId, request, cancellationToken);
            return Ok(columnas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
