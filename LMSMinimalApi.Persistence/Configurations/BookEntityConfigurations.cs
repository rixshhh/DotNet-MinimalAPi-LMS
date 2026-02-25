using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSMinimalApi.Persistence.Configurations;

public sealed class BookEntityConfigurations : IEntityTypeConfiguration<Books>
{
    public void Configure(EntityTypeBuilder<Books> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.ID);

        builder
            .Property(b => b.BookName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(b => b.Publisher)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Price)
            .HasPrecision(6, 2)
            .IsRequired();

        // Relationship 
        builder.HasOne(b => b.Categories)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}