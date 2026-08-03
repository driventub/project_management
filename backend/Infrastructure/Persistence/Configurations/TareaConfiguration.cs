using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.ToTable("tareas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Descripcion)
            .HasMaxLength(2000);

        builder.Property(t => t.Prioridad)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.FechaCreacion)
            .IsRequired();

        builder.Property(t => t.Orden)
            .IsRequired();

        builder.HasOne(t => t.Responsable)
            .WithMany(u => u.TareasAsignadas)
            .HasForeignKey(t => t.ResponsableId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
