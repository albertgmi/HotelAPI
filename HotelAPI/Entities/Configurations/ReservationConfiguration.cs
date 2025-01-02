using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Entities.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasOne(r => r.MadeBy)
                .WithMany(u => u.Reservations)
                .HasForeignKey(fk => fk.MadeById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.CheckInDate)
                .IsRequired();
            builder.Property(r => r.CheckOutDate)
                .IsRequired();
            builder.Property(r => r.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();
        }
    }
}
