using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class ColumnaConfiguration : IEntityTypeConfiguration<Columna>
{
    public void Configure(EntityTypeBuilder<Columna> builder)
    {
        builder.ToTable("columnas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Orden)
            .IsRequired();

        builder.HasMany(c => c.Tareas)
            .WithOne(t => t.Columna)
            .HasForeignKey(t => t.ColumnaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
