namespace HotelAPI.Services.FileServiceFolder
{
    public interface IFileService
    {
        string UploadImage(int hotelId, int? roomId, IFormFile file);
        void AddImageToRoom(int hotelId, int roomId, string url);
        public void AddImageToHotel(int hotelId, string url);
        void DeleteImage(string url);
        void DeleteImageFromDb(int imageId);
    }
}
