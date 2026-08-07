using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Tareas;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/proyectos/{proyectoId:guid}/tareas")]
public class TareasController : ControllerBase
{
    private readonly TareaService _tareaService;

    public TareasController(TareaService tareaService)
    {
        _tareaService = tareaService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TareaDto>>> GetByProyecto(Guid proyectoId, CancellationToken cancellationToken)
    {
        var tareas = await _tareaService.GetByProyectoIdAsync(proyectoId, cancellationToken);
        return Ok(tareas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TareaDto>> GetById(Guid proyectoId, Guid id, CancellationToken cancellationToken)
    {
        var tarea = await _tareaService.GetByIdAsync(id, cancellationToken);
        return tarea is null || tarea.ProyectoId != proyectoId ? NotFound() : Ok(tarea);
    }

    [HttpPost]
    public async Task<ActionResult<TareaDto>> Create(Guid proyectoId, [FromBody] TareaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _tareaService.CreateAsync(proyectoId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { proyectoId, id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TareaDto>> Update(Guid proyectoId, Guid id, [FromBody] TareaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _tareaService.UpdateAsync(proyectoId, id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid proyectoId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _tareaService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/mover")]
    public async Task<ActionResult<TareaDto>> Mover(Guid proyectoId, Guid id, [FromBody] MoverTareaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tarea = await _tareaService.MoverAsync(id, request, cancellationToken);
            return tarea is null ? NotFound() : Ok(tarea);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
