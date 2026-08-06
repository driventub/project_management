using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Ports;
using ProjectManagement.Infrastructure.Persistence;
using ProjectManagement.Infrastructure.Realtime;
using ProjectManagement.Infrastructure.Reportes;
using ProjectManagement.Infrastructure.Repositories;
using ProjectManagement.Infrastructure.Security;
using QuestPDF.Infrastructure;

namespace ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddDbContext<ProjectManagementDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProjectManagementDbContext>());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProyectoRepository, ProyectoRepository>();
        services.AddScoped<IColumnaRepository, ColumnaRepository>();
        services.AddScoped<ITareaRepository, TareaRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddSignalR();
        services.AddScoped<ITableroNotifier, TableroNotifier>();

        services.AddScoped<IReporteExporter, QuestPdfReporteExporter>();
        services.AddScoped<IReporteExporter, ClosedXmlReporteExporter>();

        return services;
    }
}
