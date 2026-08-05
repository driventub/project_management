using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Usuarios;

namespace ProjectManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> GetAll(CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.GetAllAsync(cancellationToken);
        return Ok(usuarios);
    }
}
