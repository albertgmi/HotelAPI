using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Entities.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasOne(h => h.Address)
                .WithOne(a => a.Hotel)
                .HasForeignKey<Hotel>(a => a.AddressId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(h => h.CreatedBy)
                 .WithMany(u => u.Hotels)
                 .HasForeignKey(hfk => hfk.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Property(h => h.Name)
                .IsRequired();
            builder.Property(h => h.Rating)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();
        }
    }
}
