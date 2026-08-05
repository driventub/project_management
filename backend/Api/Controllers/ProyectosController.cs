using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Proyectos;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/proyectos")]
public class ProyectosController : ControllerBase
{
    private readonly ProyectoService _proyectoService;

    public ProyectosController(ProyectoService proyectoService)
    {
        _proyectoService = proyectoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProyectoDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nombre = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _proyectoService.GetPagedAsync(pageNumber, pageSize, nombre, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProyectoDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoService.GetByIdAsync(id, cancellationToken);
        return proyecto is null ? NotFound() : Ok(proyecto);
    }

    [HttpPost]
    public async Task<ActionResult<ProyectoDto>> Create([FromBody] ProyectoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _proyectoService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProyectoDto>> Update(Guid id, [FromBody] ProyectoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _proyectoService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _proyectoService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
