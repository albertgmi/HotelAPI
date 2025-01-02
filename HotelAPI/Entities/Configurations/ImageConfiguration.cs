using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Entities.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasOne(i => i.Hotel)
                .WithMany(h => h.Images)
                .HasForeignKey(ifk => ifk.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Room)
                .WithMany(r => r.Images)
                .HasForeignKey(ifk => ifk.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Url)
                .IsRequired();
            builder.Property(i => i.RoomId)
                .IsRequired(false);
        }
    }
}
