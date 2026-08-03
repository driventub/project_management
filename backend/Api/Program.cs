using ProjectManagement.Application.Ports;
using ProjectManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.MapGet("/api/usuarios", async (IUsuarioRepository usuarioRepository, CancellationToken cancellationToken) =>
// {
//     var usuarios = await usuarioRepository.GetAllAsync(cancellationToken);
//     return usuarios.Select(u => new { u.Id, u.Nombre, u.Email });
// })
// .WithName("GetUsuarios")
// .WithOpenApi();
