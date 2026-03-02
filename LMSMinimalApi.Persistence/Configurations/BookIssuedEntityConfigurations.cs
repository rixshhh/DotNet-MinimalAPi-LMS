using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSMinimalApi.Persistence.Configurations;

public class BookIssuedEntityConfigurations : IEntityTypeConfiguration<BookIssued>
{
    public void Configure(EntityTypeBuilder<BookIssued> builder)
    {
        builder.HasKey(bi => bi.ID);


        builder.Property(bi => bi.BookPrice)
            .HasColumnType("decimal(6,2)")
            .IsRequired();

        builder.Property(bi => bi.IssueDate)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(bi => bi.RenewDate)
            .IsRequired();


        builder.Property(bi => bi.ReturnDate)
            .IsRequired(false);

        builder.HasOne(bi => bi.Book)
            .WithMany(b => b.BookIssueds)
            .HasForeignKey(bi => bi.BookID)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(bi => bi.User)
            .WithMany(u => u.BookIssueds)
            .HasForeignKey(bi => bi.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}