using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;

namespace ToDo.Infrastructure.DAL.Configurations;

internal sealed class ToDoTaskConfiguration : IEntityTypeConfiguration<ToDoTask>
{
    public void Configure(EntityTypeBuilder<ToDoTask> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                v => v.Value,
                v => new ToDoTaskId(v))
            .IsRequired();
        builder.Property(x => x.ExpirationDate)
            .HasConversion(
                v => v.Value,
                v => new DateAndTime(v))
            .IsRequired();
        builder.Property(x => x.Title)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Title(v))
            .HasMaxLength(50);
        builder.Property(x => x.Description)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Description(v))
            .HasMaxLength(500);
        builder.Property(x => x.PercentComplete)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new PercentComplete(v));
    }
}