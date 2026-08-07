using System.Text;
using System.Text.Json.Serialization;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Application.Auth;
using ProjectManagement.Application.Columnas;
using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Proyectos;
using ProjectManagement.Application.Reportes;
using ProjectManagement.Application.Tareas;
using ProjectManagement.Application.Usuarios;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.Persistence;
using ProjectManagement.Infrastructure.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMapster();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<ColumnaService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ReporteService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key configuration value is required.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer configuration value is required.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience configuration value is required.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };

        // Browsers can't set custom headers on a WebSocket upgrade request, so the
        // SignalR JS client falls back to an access_token query string param for that
        // transport. Only honor it on the hub path, never on regular API requests.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments(TableroHub.Ruta))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

const string FrontendCorsPolicy = "FrontendCorsPolicy";
var corsAllowedOrigins = (builder.Configuration["Cors:AllowedOrigins"]
    ?? throw new InvalidOperationException("Cors:AllowedOrigins configuration value is required."))
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(corsAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

// Applies any pending migrations on startup (idempotent — EF tracks what's
// already applied via __EFMigrationsHistory), so `docker compose up` against
// a brand-new Postgres volume creates the schema and seeds the two users
// without a separate manual `dotnet ef database update` step.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TableroHub>(TableroHub.Ruta);

// app.MapGet("/api/usuarios", async (IUsuarioRepository usuarioRepository, CancellationToken cancellationToken) =>
// {
//     var usuarios = await usuarioRepository.GetAllAsync(cancellationToken);
//     return usuarios.Select(u => new { u.Id, u.Nombre, u.Email });
// })
// .WithName("GetUsuarios")
// .WithOpenApi();

app.Run();
