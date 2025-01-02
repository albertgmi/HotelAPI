namespace HotelAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Relations
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
