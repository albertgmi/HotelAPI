using HotelAPI.Entities;
using HotelAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Services.FileServiceFolder
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly HotelDbContext _dbContext;
        public FileService(IWebHostEnvironment environment, HotelDbContext dbContext)
        {
            _environment = environment;
            _dbContext = dbContext;
        }
        public string UploadImage(int hotelId, int? roomId, IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException("Invalid file type.");

            var rootPath = _environment.WebRootPath;
            var folderPath = roomId == null
                ? Path.Combine(rootPath, "images", "hotels", hotelId.ToString())
                : Path.Combine(rootPath, "images", "hotels", hotelId.ToString(), roomId.ToString());
            var fileName = file.FileName;
            Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}" +
                $"_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}_{fileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return roomId == null
                ? $"/images/hotels/{hotelId}/{uniqueFileName}"
                : $"/images/hotels/{hotelId}/{roomId}/{uniqueFileName}";
        }
        public void AddImageToHotel(int hotelId, string url)
        {
            var hotel = _dbContext
                .Hotels
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");

            var image = new Image
            {
                HotelId = hotelId,
                Url = url,
                RoomId = null
            };
            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();
        }
        public void AddImageToRoom(int hotelId, int roomId, string url)
        {
            var hotel = _dbContext
                .Hotels
                .Include(x => x.Rooms)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");

            var room = hotel
                .Rooms
                .FirstOrDefault(x => x.Id == roomId);
            if (room is null)
                throw new NotFoundException("Room not found");

            var image = new Image
            {
                RoomId = roomId,
                Url = url
            };
            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();
        }
        public void DeleteImage(string url)
        {
            var rootPath = _environment.WebRootPath;
            var filePath = Path.Combine(rootPath, url.TrimStart('/'));

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        public void DeleteImageFromDb(int imageId)
        {
            var image = _dbContext
                .Images
                .FirstOrDefault(x => x.Id == imageId);
            if (image is null)
                throw new NotFoundException("Image not found");
            _dbContext.Images.Remove(image);
            _dbContext.SaveChanges();
        }
    }
}
