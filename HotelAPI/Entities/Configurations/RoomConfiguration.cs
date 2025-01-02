using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Entities.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasMany(r => r.Reservations)
                .WithOne(r => r.Room)
                .HasForeignKey(fk => fk.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.Name)
                .IsRequired();
            builder.Property(r => r.Description)
                .IsRequired();
            builder.Property(r => r.Capacity)
                .IsRequired();
            builder.Property(r => r.PricePerNight)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();
        }
    }
}
