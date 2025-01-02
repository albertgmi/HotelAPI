using Bogus;
using HotelAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Seeders
{
    public class HotelSeeder : IHotelSeeder
    {
        readonly Faker faker = new Faker("pl");
        readonly Randomizer randomizer = new Randomizer();
        private readonly IPasswordHasher<User> _passwordHasher;
        public void Seed(HotelDbContext _dbContext)
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
                if (_dbContext.Roles.Any())
                    return;
                var roles = CreateRoles();
                _dbContext.Roles.AddRange(roles);
                _dbContext.SaveChanges();

                var users = CreateUsers(_dbContext.Roles.ToList());
                _dbContext.Users.AddRange(users);
                _dbContext.SaveChanges();

                var addresses = CreateAddresses();
                _dbContext.Addresses.AddRange(addresses);
                _dbContext.SaveChanges();

                var hotels = CreateHotels(_dbContext.Users.ToList(), _dbContext.Addresses.ToList());
                _dbContext.Hotels.AddRange(hotels);
                _dbContext.SaveChanges();

                var rooms = CreateRooms(_dbContext.Hotels.ToList());
                _dbContext.Rooms.AddRange(rooms);
                _dbContext.SaveChanges();

                var reservations = CreateReservations(_dbContext.Users.ToList(), _dbContext.Rooms.ToList());
                _dbContext.Reservations.AddRange(reservations);
                _dbContext.SaveChanges();
            }
        }
        public HotelSeeder(IPasswordHasher<User> passwordHasher, HotelDbContext dbContext)
        {
            _passwordHasher = passwordHasher;
        }
        private List<Role> CreateRoles()
        {
            var roles = new List<Role>();
            roles.Add(new Role()
            {
                Name = "Guest"
            });
            roles.Add(new Role()
            {
                Name = "Manager"
            });
            roles.Add(new Role()
            {
                Name = "Admin"
            });
            return roles;
        }
        private List<Entities.Address> CreateAddresses()
        {
            var addresses = new List<Entities.Address>();
            for (int i = 0; i < 400; i++)
            {
                addresses.Add(new Entities.Address()
                {
                    City = faker.Address.City(),
                    Street = faker.Address.StreetAddress(),
                    PostalCode = faker.Address.ZipCode(),
                });
            }
            return addresses;
        }
        private List<User> CreateUsers(List<Role> roles)
        {
            var users = new List<User>();

            for (int i = 0; i < 400; i++)
            {
                var faker2 = new Faker("pl");
                var passwordBeforeHash = randomizer.String2(length: 8, chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()");
                var user = new User()
                {
                    FirstName = faker2.Person.FirstName,
                    LastName = faker2.Person.LastName,
                    Email = faker2.Person.Email,
                    PasswordHash = passwordBeforeHash,
                    RoleId = roles[faker.Random.Int(0, roles.Count() - 1)].Id,
                    DateOfBirth = faker.Date.Between(DateTime.Parse("1930-01-01"), DateTime.Parse("2015-01-01"))
                };
                var hashed = _passwordHasher.HashPassword(user, user.PasswordHash);
                user.PasswordHash = hashed;
                users.Add(user);
            }
            return users;
        }
        private List<Hotel> CreateHotels(List<User> users, List<Entities.Address> addresses)
        {
            var hotels = new List<Hotel>();
            for (int i = 0; i < 600; i++)
            {
                hotels.Add(new Hotel()
                {
                    Name = faker.Company.CompanyName(),
                    Description = faker.Lorem.Text(),
                    Rating = faker.Random.Decimal() * 5,
                    CreatedById = users[faker.Random.Int(0, users.Count() - 1)].Id,
                    AddressId = addresses[faker.Random.Int(0, addresses.Count() - 1)].Id,
                    ContactNumber = faker.Phone.PhoneNumber("###-###-###"),
                    NumberOfRatings = 0
                });
            }
            return hotels;
        }
        private List<Reservation> CreateReservations(List<User> users, List<Room> rooms)
        {
            var status = new string[] { "Pending", "Confirmed", "Checked-in", "Completed", "Cancelled", "Paid" };
            var reservations = new List<Reservation>();
            for (int i = 0; i < 10000; i++)
            {
                reservations.Add(new Reservation()
                {
                    CheckInDate = faker.Date.Between(DateTime.Parse("2024-01-01"), DateTime.Now),
                    CheckOutDate = faker.Date.Between(DateTime.Now.AddDays(1), DateTime.Now.AddDays(60)),
                    TotalPrice = faker.Random.Decimal() * 10,
                    Status = status[faker.Random.Int(0, status.Length - 1)],
                    MadeById = users[faker.Random.Int(0, users.Count() - 1)].Id,
                    RoomId = rooms[faker.Random.Int(0, rooms.Count() - 1)].Id
                });
            }
            return reservations;
        }
        private List<Room> CreateRooms(List<Hotel> hotels)
        {
            var roomTypes = new string[]
            {
                "Single", "Double", "Twin", "Triple", "Quad", "Suite",
                "King", "Queen", "Family", "Connecting", "Accessible",
                "Penthouse", "Dormitory"
            };

            var rooms = new List<Room>();

            foreach (var hotel in hotels)
            {
                for (int i = 0; i < 45; i++)
                {
                    var room = new Room()
                    {
                        Name = $"{faker.Address.City()} {faker.Random.Word()} Room",
                        Description = faker.Lorem.Text(),
                        Type = roomTypes[faker.Random.Int(0, roomTypes.Length - 1)],
                        Capacity = faker.Random.Int(1, 10),
                        PricePerNight = faker.Random.Decimal() * 25,
                        IsAvailable = faker.Random.Bool(),
                        HotelId = hotel.Id
                    };

                    hotel.Rooms.Add(room);
                    rooms.Add(room);
                }
            }
            for (int i = 0; i < 350; i++)
            {
                var room = new Room()
                {
                    Name = $"{faker.Address.City()} {faker.Random.Word()} Room",
                    Description = faker.Lorem.Text(),
                    Type = roomTypes[faker.Random.Int(0, roomTypes.Length - 1)],
                    Capacity = faker.Random.Int(1, 10),
                    PricePerNight = faker.Random.Decimal() * 25,
                    HotelId = hotels[faker.Random.Int(0, hotels.Count - 1)].Id
                };

                rooms.Add(room);
            }
            return rooms;
        }
    }
}
