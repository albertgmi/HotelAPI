using HotelAPI.Models.UserModels;

namespace HotelAPI.Services.UserServiceFolder
{
    public interface IUserService
    {
        void RegisterUser(RegisterUserDto registerUserDto);
        string GenerateJwt(LoginUserDto loginUserDto);
        void MakeAdmin(int userId);
        void MakeManager(int userId);
    }
}
